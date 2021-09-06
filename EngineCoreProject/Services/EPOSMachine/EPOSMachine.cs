using EngineCoreProject.DTOs.EPOSMachineDto;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.IRepository.IEPOSMachineRepository;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.IRepository.ISysLookUpRepository;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.EPOSMachine
{
    public class EPOSMachine : IEPOSMachine
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly ApiEposUrl ApiEposUrl;
        private readonly ILogger<EPOSMachine> _logger;

        ValidatorException _exception;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepositiory;
        public EPOSMachine(IOptions<ApiEposUrl> apiEposUrl, IUserRepository iUserRepository,
                           IGeneralRepository iGeneralRepository, EngineCoreDBContext EngineCoreDBContext,
                           IFilesUploaderRepositiory iFilesUploaderRepositiory, ILogger<EPOSMachine> logger)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _IUserRepository = iUserRepository;
            _exception = new ValidatorException();
            _IFilesUploaderRepositiory = iFilesUploaderRepositiory;
            _logger = logger;
            ApiEposUrl = apiEposUrl.Value;
        }

        public async Task<ResLoginDto> MerchantLoginAsync(string lang, int EnotaryId)
        {
            var user = await _EngineCoreDBContext.User.Where(x => x.Id == EnotaryId).FirstOrDefaultAsync();
            if (user == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }
            var employeeSetting = await _EngineCoreDBContext.EmployeeSetting.Where(x => x.EnotaryId == EnotaryId).FirstOrDefaultAsync();
            if (employeeSetting == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            var secureHash = GetLogInSecureHash(new ReqLogin { entityCode = employeeSetting.EntityCode, terminalID = employeeSetting.TerminalId, userID = employeeSetting.UserId });
            ReqLogin loginReq = new ReqLogin { entityCode = employeeSetting.EntityCode, userID = employeeSetting.UserId, terminalID = employeeSetting.TerminalId, secureHash = secureHash };
            using var client = new HttpClient();

            var json = JsonConvert.SerializeObject(loginReq);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = ApiEposUrl.loginURL;

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            try
            {
                httpResponseMessage = client.PostAsync(url, data).Result;

                if (httpResponseMessage == null)
                {
                    var message = "EPOS: Empty result in SerializeObject " + loginReq;
                    _exception.AttributeMessages.Add(message);
                    _logger.LogInformation(message);
                    throw _exception;
                }
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                var message = "EPOS: The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout " + url + " the error is " + ex.Message;
                _exception.AttributeMessages.Add(message);
                _logger.LogInformation(message);
                throw _exception;
            }

            catch (Exception ex)
            {
                var message = "EPOS: error in the request " + url + " the error is " + ex.Message;
                _exception.AttributeMessages.Add(message);
                _logger.LogInformation(message);
                throw _exception;
            }

            string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            ResLogin merchantLoginRes = new ResLogin();
            try
            {
                merchantLoginRes = JsonConvert.DeserializeObject<ResLogin>(result);
                if (merchantLoginRes == null)
                {
                    var message = "EPOS: Empty result in DeserializeObject " + result;
                    _exception.AttributeMessages.Add(message);
                    _logger.LogInformation(message);
                    throw _exception;
                }

                if (merchantLoginRes.responseCode != Constants.SuccessfulEPOSCodeStatus)
                {
                    var message = "EPOS: Failed response, error is  " + merchantLoginRes.responseCode + " description is " + merchantLoginRes.responseDescription + " entityCode is " + merchantLoginRes.entityCode + " sessionToken is " + merchantLoginRes.sessionToken + " tokenExpiryDate is " + merchantLoginRes.tokenExpiryDate.ToString();
                    _exception.AttributeMessages.Add(message);
                    _logger.LogInformation(message);
                    throw _exception;
                }
            }
            catch (Exception ex)
            {
                _exception.AttributeMessages.Add(" Error in DeserializeObject " + result + " the error is " + ex.Message);
                throw _exception;
            }


            employeeSetting.SessionToken = merchantLoginRes.sessionToken;
            employeeSetting.ExpiredSessionToken = merchantLoginRes.tokenExpiryDate;
            _EngineCoreDBContext.Update(employeeSetting);
            if (await _EngineCoreDBContext.SaveChangesAsync() > 0)
            {
                return new ResLoginDto
                {
                    sessionToken = employeeSetting.SessionToken,
                    tokenExpiryDate = employeeSetting.ExpiredSessionToken,
                    responseCode = Constants.SuccessfulEPOSCodeStatus
                };
            }
            else
            {
                _exception.AttributeMessages.Add("failed in generating new token for user EPOS.");
                throw _exception;
            }
        }


        public async Task<ResLoginDto> QueryTokenAsync(string lang)
        {

            ResLoginDto resLoginDto = new ResLoginDto();
            var employeeSetting = await _EngineCoreDBContext.EmployeeSetting.Where(x => x.EnotaryId == _IUserRepository.GetUserID()).FirstOrDefaultAsync();
            if (employeeSetting == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            if (employeeSetting.SessionToken == null || employeeSetting.ExpiredSessionToken < DateTime.Now)
            {
                return await MerchantLoginAsync(lang, _IUserRepository.GetUserID());

            }
            else
            {
                return new ResLoginDto { sessionToken = employeeSetting.SessionToken, tokenExpiryDate = employeeSetting.ExpiredSessionToken };
            }
        }
        


        public async Task<ResQueryPriceDto> QueryServicePriceAsync(ReqQueryPriceDto reqQueryPriceDto, int notaryId, string lang)
        {
            ResQueryPriceDto resQueryPriceDto = new ResQueryPriceDto();

            // validations.
            var notary = await _EngineCoreDBContext.User.Where(x => x.Id == notaryId).FirstOrDefaultAsync();
            if (notary == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            var app = await _EngineCoreDBContext.Application.Where(x => x.Id == reqQueryPriceDto.AppId).FirstOrDefaultAsync();
            if (app == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "AppNotFound"));
                throw _exception;
            }

            var employeeSetting = await _EngineCoreDBContext.EmployeeSetting.Where(x => x.EnotaryId == notaryId).FirstOrDefaultAsync();
            if (employeeSetting == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            var payment = await _EngineCoreDBContext.Payment.Include(x => x.PaymentGateAttempt).Where(x => x.ApplicationId == reqQueryPriceDto.AppId).FirstOrDefaultAsync();
            if (payment == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "nopaymentsforAPP"));
                throw _exception;
            }

            if (payment.Status == Constants.SuccessfulCodeStatus)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "paidBefore"));
                throw _exception;
            }

            // Check old EPOS attempts.
            var oldAttempts = await _EngineCoreDBContext.PaymentGateAttempt.Where(x => x.PaymentId == payment.Id).ToListAsync();
            foreach (var oldAttempt in oldAttempts)
            {
                ResQueryURN resultResQueryURN = new ResQueryURN();
                try
                {
                    resultResQueryURN = await CheckUniqueReferenceNumberStatus(lang, notaryId, oldAttempt.PaymentAttemptInvoiceNo);
                }
                catch (ValidatorException ex)
                {
                    _exception.AttributeMessages = ex.AttributeMessages;
                    throw _exception;
                }
                catch (Exception ex)
                {
                    _exception.AttributeMessages.Add("Unknown error in calling old attempts " + ex.Message);
                    throw _exception;
                }

                // check if the response means that the payment is paid or the request itself is succeeded.
                if (resultResQueryURN.responseCode == Constants.SuccessfulEPOSCodeStatus && resultResQueryURN.paymentStatus.paymentDone == true)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "paidBefore") + " from query Epos machine.");
                    throw _exception;
                }
            }

            var paymentDetailsList = await _EngineCoreDBContext.PaymentDetails.Where(x => x.PaymentId == payment.Id).ToListAsync();
            List<ServiceReq> serviceList = new List<ServiceReq>();
            foreach (PaymentDetails paymentDetails in paymentDetailsList)
            {
                ServiceReq serviceReq = new ServiceReq()
                {
                    serviceCode = await GetEposServiceCodeByMainSubCodeAsync(lang, paymentDetails.ServiceSubCode, paymentDetails.ServiceMainCode),
                    quantity = paymentDetails.Quantity.ToString(),
                    price = paymentDetails.Price.ToString(),
                };

                serviceList.Add(serviceReq);
            }

            if (employeeSetting.SessionToken == null || employeeSetting.ExpiredSessionToken < DateTime.Now)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "SessionTimeExpired"));
                throw _exception;
            }

            ReqQueryPrice reqQueryPrice = new ReqQueryPrice
            {
                sessionToken = employeeSetting.SessionToken,
                userID = employeeSetting.UserId,
                entityCode = employeeSetting.EntityCode,
                channel = employeeSetting.Channel,
                instrument = reqQueryPriceDto.Instrument,
                locationCode = employeeSetting.LocationCode,
                transactionReference = employeeSetting.TransactionReference,
                sourceReference = employeeSetting.SourceReference,
                services = serviceList
            };

            reqQueryPrice.secureHash = GetQueryServicePriceSecureHash(reqQueryPrice);
            ResQueryPrice resQueryPrice = new ResQueryPrice();

            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(reqQueryPrice);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = ApiEposUrl.QueryServicePriceURL;

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            try
            {
                httpResponseMessage = client.PostAsync(url, data).Result;

                if (httpResponseMessage == null)
                {
                    _exception.AttributeMessages.Add("empty result in the request " + url);
                    throw _exception;
                }
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                _exception.AttributeMessages.Add("The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout " + url + " the error is " + ex.Message);
                throw _exception;

            }

            catch (Exception ex)
            {
                _exception.AttributeMessages.Add("error in the request " + url + " the error is " + ex.Message);
                throw _exception;
            }

            string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            try
            {
                resQueryPrice = JsonConvert.DeserializeObject<ResQueryPrice>(result);

                if (resQueryPrice == null)
                {
                    var message = "EPOS: empty result in DeserializeObject " + result;
                    _exception.AttributeMessages.Add(message);
                    _logger.LogInformation(message);
                    throw _exception;
                }

                if (resQueryPrice.responseCode != Constants.SuccessfulEPOSCodeStatus)
                {
                    var message = "EPOS: Failed QueryServicePrice response, error is  " + resQueryPrice.responseCode + " description is " + resQueryPrice.responseDescription + " entityCode is " + resQueryPrice.entityCode;
                    _exception.AttributeMessages.Add(message);
                    _logger.LogInformation(message);
                    throw _exception;
                }
            }
            catch (Exception ex)
            {
                var message = "EPOS: Error in DeserializeObject " + result + " the error is " + ex.Message;
                _exception.AttributeMessages.Add(message);
                _logger.LogInformation(message);
                throw _exception;
            }

            // update after success pick a reference number response. 
            payment.Status = resQueryPrice.responseCode;
            payment.StatusMessage = " Waiting for pay after pick a ref number ";
            payment.PaymentStatus = 0.ToString();
            payment.PaymentType = Constants.EPOS_PAYMENT;
            payment.PaymentSource = Constants.EPOS_PAYMENT;
            payment.PaymentMethodType = Constants.EPOS_PAYMENT;
            payment.UserId = reqQueryPriceDto.UserId;
            payment.PaymentDate = DateTime.Now;
            payment.ReceiptNo = "PAY_RECEIPTNO_EPOS_" + resQueryPrice.uniqueReferenceNumber;

            
            _EngineCoreDBContext.Payment.Update(payment);
            if (await _EngineCoreDBContext.SaveChangesAsync() > 0)
            {
                // add new attempt.
                PaymentGateAttempt paymentAttempt = new PaymentGateAttempt
                {
                    PaymentId = payment.Id,
                    ConfirmationId = notaryId.ToString(),
                    Pun = "Epos",
                    PaidAttemptDate = DateTime.Now,
                    PaymentAttemptInvoiceNo = resQueryPrice.uniqueReferenceNumber
                };
                _EngineCoreDBContext.PaymentGateAttempt.Update(paymentAttempt);
                if (await _EngineCoreDBContext.SaveChangesAsync() > 0)
                {
                    resQueryPriceDto.message = Constants.getMessage(lang, "AskPaymentEposSuccess");
                    resQueryPriceDto.responseCode = Constants.SuccessfulEPOSCodeStatus;
                    resQueryPriceDto.totalAmount = resQueryPrice.totalAmount;
                    resQueryPriceDto.urnReferenceNumber = resQueryPrice.uniqueReferenceNumber;
                }
                else
                {
                    _exception.AttributeMessages.Add("Failed in save payment attempt EPOS.");
                    throw _exception;
                }
            }

            return resQueryPriceDto;
        }


        public async Task<EmployeeSetting> GetEposSetting(string lang, int EnotaryId)
        {
            User user = await _EngineCoreDBContext.User.Where(x => x.Id == EnotaryId).FirstOrDefaultAsync();
            if (user == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }
            var employeeSetting = await _EngineCoreDBContext.EmployeeSetting.Where(x => x.EnotaryId == EnotaryId).FirstOrDefaultAsync();
            if (employeeSetting == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            return employeeSetting;
        }

        public async Task UpdateEposSetting(string lang, EPOSMachineSettingDto EPOSMachineSettingDto)
        {
            try
            {
                int? UserId = EPOSMachineSettingDto.EnotaryId;
                User user = await _EngineCoreDBContext.User.Where(x => x.Id == UserId).FirstOrDefaultAsync();
                if (user == null)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                    throw _exception;
                }

                EmployeeSetting UpdatedEPOSMachineSetting = new EmployeeSetting
                {
                    Channel = EPOSMachineSettingDto.channel,
                    EntityCode = EPOSMachineSettingDto.entityCode,
                    SourceReference = EPOSMachineSettingDto.sourceReference,
                    TransactionReference = EPOSMachineSettingDto.transactionReference,
                    TerminalId = EPOSMachineSettingDto.terminalID,
                    LocationCode = EPOSMachineSettingDto.locationCode,
                    UserId = EPOSMachineSettingDto.userID,
                    EnotaryId = EPOSMachineSettingDto.EnotaryId           
                };

                var employeeSetting = await _EngineCoreDBContext.EmployeeSetting.Where(x => x.EnotaryId == UserId).FirstOrDefaultAsync();
                if (employeeSetting == null)
                {
                    await _EngineCoreDBContext.EmployeeSetting.AddAsync(UpdatedEPOSMachineSetting);
                }
                else
                {
                    employeeSetting.Channel = UpdatedEPOSMachineSetting.Channel;
                    employeeSetting.EntityCode = UpdatedEPOSMachineSetting.EntityCode;
                    employeeSetting.SourceReference = UpdatedEPOSMachineSetting.SourceReference;
                    employeeSetting.TransactionReference = UpdatedEPOSMachineSetting.TransactionReference;
                    employeeSetting.TerminalId = UpdatedEPOSMachineSetting.TerminalId;
                    employeeSetting.LocationCode = UpdatedEPOSMachineSetting.LocationCode;
                    employeeSetting.UserId = UpdatedEPOSMachineSetting.UserId;
                    _EngineCoreDBContext.Update(employeeSetting);
                }
                await _EngineCoreDBContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "faildUpdate") + "  "  + e.Message);
                throw _exception;
            }
        }

        public async Task<ResQueryURN> CheckUniqueReferenceNumberStatus(string lang, int NotaryId, string UniqueReferenceNumber)
        {
            User notary = await _EngineCoreDBContext.User.Where(x => x.Id == NotaryId).FirstOrDefaultAsync();
            var employeeSetting = await _EngineCoreDBContext.EmployeeSetting.Where(x => x.EnotaryId == NotaryId).FirstOrDefaultAsync();
            if (employeeSetting == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            ReqQueryURN reqQueryURN = new ReqQueryURN
            {
                entityCode = employeeSetting.EntityCode,
                uniqueReferenceNumber = UniqueReferenceNumber,
                sourceReference = employeeSetting.SourceReference,
                transactionReference = employeeSetting.TransactionReference,
                userID = employeeSetting.UserId,
                sessionToken = employeeSetting.SessionToken,
                optionalParameter1 = "optionalParameter1",
                optionalParameter2 = "optionalParameter2"
            };

            reqQueryURN.secureHash = GetQueryURNSecureHash(reqQueryURN);
            using var client = new HttpClient();

            string json = "";
            try
            {
                json = JsonConvert.SerializeObject(reqQueryURN);

                if (json == "")
                {
                    _exception.AttributeMessages.Add("empty result in SerializeObject " + reqQueryURN);
                    throw _exception;
                }
            }
            catch (Exception ex)
            {
                _exception.AttributeMessages.Add("error in SerializeObject " + reqQueryURN + " the error is " + ex.Message);
                throw _exception;
            }

            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = ApiEposUrl.QueryUrnURL;
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            try
            {
                httpResponseMessage = client.PostAsync(url, data).Result;
                if (httpResponseMessage == null)
                {
                    _exception.AttributeMessages.Add("empty result in SerializeObject " + reqQueryURN);
                    throw _exception;
                }
            }

            catch (System.Net.Http.HttpRequestException ex)
            {
                var message = "EPOS: The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout " + url + " the error is " + ex.Message;
                _exception.AttributeMessages.Add(message);
                _logger.LogInformation(message);
                throw _exception;

            }
            catch (Exception ex)
            {
                var message = "EPOS: error in the request " + url + " the error is " + ex.Message;
                _exception.AttributeMessages.Add(message);
                _logger.LogInformation(message);
                throw _exception;
            }

            ResQueryURN resQueryURN = new ResQueryURN();
            string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            try
            {
                resQueryURN = JsonConvert.DeserializeObject<ResQueryURN>(result);

                if (resQueryURN == null)
                {
                    _exception.AttributeMessages.Add("empty result in DeserializeObject " + result);
                    throw _exception;
                }
            }

            catch (Exception ex)
            {
                _exception.AttributeMessages.Add(" Error in DeserializeObject " + result + " the error is " + ex.Message);
                throw _exception;
            }

            return resQueryURN;
        }


        private string HmacSha256Digest(string message, string secret)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            HMACSHA256 cryptographer = new HMACSHA256(keyBytes);
            byte[] bytes = cryptographer.ComputeHash(messageBytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private string GetLogInSecureHash(ReqLogin ReqLogin)
        {
            string message = ReqLogin.entityCode + ReqLogin.terminalID + ReqLogin.userID;
            return HmacSha256Digest(message, "c6cbacff23f341a0");
        }
        private string GetQueryServicePriceSecureHash(ReqQueryPrice ReqQueryPrice)
        {
            string message = ReqQueryPrice.channel + ReqQueryPrice.instrument + ReqQueryPrice.transactionReference + ReqQueryPrice.userID + ReqQueryPrice.sessionToken;
            return HmacSha256Digest(message, "c6cbacff23f341a0");
        }
        private string GetQueryURNSecureHash(ReqQueryURN ReqQueryURN)
        {
            string message = ReqQueryURN.entityCode + ReqQueryURN.uniqueReferenceNumber + ReqQueryURN.transactionReference;

            return HmacSha256Digest(message, "c6cbacff23f341a0");
        }
        private async Task<string> GetEposServiceCodeByMainSubCodeAsync(string lang, string SubClass, string PrimeClass)
        {
            string EposServiceCode = await _EngineCoreDBContext.TransactionFee.Where(x => x.SubClass == SubClass && x.PrimeClass == PrimeClass).Select(y => y.ServiceCodeEpos).FirstOrDefaultAsync();
            if (EposServiceCode == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "EposServiceCode"));
                throw _exception;
            }
            return EposServiceCode.Trim();
        }
    }
}
