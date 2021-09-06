using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.FeesDto;
using EngineCoreProject.DTOs.Payment;
using EngineCoreProject.DTOs.PDFGenerator;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.IGeneratorRepository;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using EngineCoreProject.IRepository.IPaymentRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services.ApplicationSet;
using EngineCoreProject.Services.GeneratorServices;
using EngineCoreProject.Services.Job;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.DTOs.ApplicationDtos.RelatedContent;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.TemplateSetRepository;
using DinkToPdf.Contracts;
using EngineCoreProject.DTOs.ConfigureWritableDto;
using PaymentServicePro;
using EngineCoreProject.DTOs.EPOSMachineDto;
using System.Security.Cryptography;
using System.Net.Http;
using EngineCoreProject.IRepository.IEPOSMachineRepository;

namespace EngineCoreProject.Services.Payment
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ILogger<PaymentRepository> _logger;
        private readonly ILogger<ApplicationRepositiory> _loggerForApplication;
        private readonly PaymentSettings _paymentSettings;
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly INotificationSettingRepository _iNotificationSettingRepository;
        private readonly ISendNotificationRepository _ISendNotificationRepository;
        private readonly ITransactionRepository _ITransactionRepository;
        private readonly IApplicationPartyRepository _IApplicationPartyRepository;
        private readonly IConfiguration _IConfiguration;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IAdmServiceRepository _IAdmServiceRepository;
        private readonly ISysValueRepository _ISysValueRepository;
        private readonly IApplicationTrackRepository _IApplicationTrackRepository;
        private readonly ITemplateRepository _ITemplateRepository;
        private readonly IOptions<FileNaming> _FileNaming;
        private readonly IOptions<Pdfdocumentsetting> _Pdfdocumentsetting;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepositiory;
        private readonly ApiEposUrl ApiEposUrl;
        private IConverter _IConverter;
        private readonly IBlockChain _IBlockChain;

        private IEPOSMachine _IEPOSMachine;

        ValidatorException _exception;
        public PaymentRepository(IBlockChain iBlockChain,ILogger<PaymentRepository> logger, IGeneralRepository iGeneralRepository, IConfiguration iConfiguration, EngineCoreDBContext EngineCoreDBContext, INotificationSettingRepository iNotificationSettingRepository,
            ITransactionRepository iTransactionRepository, IFilesUploaderRepositiory iFilesUploaderRepository,
            IApplicationPartyRepository iApplicationPartyRepository, IUserRepository iUserRepository,
            ISendNotificationRepository iSendNotificationRepository, IAdmServiceRepository iAdmServiceRepository,
            ISysValueRepository iSysValueRepository, IApplicationTrackRepository iApplicationTrackRepository,
            ITemplateRepository iTemplateRepository, IOptions<FileNaming> fileNaming, IConverter converter,
            IOptions<PaymentSettings> paymentSettings, IOptions<Pdfdocumentsetting> pDFDocumentSetting,
            IFilesUploaderRepositiory iFilesUploaderRepositiory, ILogger<ApplicationRepositiory> loggerForApplication,

             IOptions<ApiEposUrl> apiEposUrl, IEPOSMachine iEPOSMachine)
        {
            _paymentSettings = paymentSettings.Value;
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _iNotificationSettingRepository = iNotificationSettingRepository;
            _IConfiguration = iConfiguration;
            _exception = new ValidatorException();
            _logger = logger;
            _ITransactionRepository = iTransactionRepository;
            _IFilesUploaderRepository = iFilesUploaderRepository;
            _IApplicationPartyRepository = iApplicationPartyRepository;
            _IUserRepository = iUserRepository;
            _ISendNotificationRepository = iSendNotificationRepository;
            _IAdmServiceRepository = iAdmServiceRepository;
            _ISysValueRepository = iSysValueRepository;
            _IApplicationTrackRepository = iApplicationTrackRepository;
            _ITemplateRepository = iTemplateRepository;
            _FileNaming = fileNaming;
            _IConverter = converter;
            _Pdfdocumentsetting = pDFDocumentSetting;
            _IFilesUploaderRepositiory = iFilesUploaderRepositiory;
            ApiEposUrl = apiEposUrl.Value;
            _IEPOSMachine = iEPOSMachine;
            _IBlockChain = iBlockChain;
            _loggerForApplication = loggerForApplication;
        }

        public async Task<string> GenerateURL(int applicationId, string lang, string description, int UserId, int actionId)
        {
            if (!await _EngineCoreDBContext.User.AnyAsync(x => x.Id == UserId))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            string URlString = null;

            var application = await _EngineCoreDBContext.Application.Where(x => x.Id == Convert.ToInt32(applicationId)).FirstOrDefaultAsync();
            if (application == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ApplicationNotFound"));
                throw _exception;
            }

            var payments = await _EngineCoreDBContext.Payment.Include(x => x.PaymentDetails).Where(x => x.ApplicationId == applicationId).ToListAsync();
            if (payments == null || payments.Count == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "nopaymentsforAPP"));
                throw _exception;
            }

            var unPaidPayments = payments.Where(x => x.Status != Constants.SuccessfulCodeStatus && x.Status != Constants.SuccessfulCodeStatusWALLET).ToList();

            if (unPaidPayments == null || unPaidPayments.Count == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "paidBefore"));
                throw _exception;
            }


            var unPaidPayment = unPaidPayments[0];
            if (unPaidPayment == null || unPaidPayment.PaymentDetails.Count == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "nopaymentDeatilsforAPP"));
                throw _exception;
            }

            foreach (var PaymentFee in unPaidPayment.PaymentDetails)
            {
                if (!await _EngineCoreDBContext.TransactionFee.AnyAsync(x => x.PrimeClass == PaymentFee.ServiceMainCode && x.SubClass == PaymentFee.ServiceSubCode))
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "unUsedFee"));
                    throw _exception;
                }

                if (PaymentFee.Price <= 0 || PaymentFee.Quantity <= 0)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "zeroNotAllowed"));
                    throw _exception;
                }

                if (unPaidPayment.PaymentDetails.Where(x => x.ServiceSubCode == PaymentFee.ServiceSubCode).Count() > 1)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "repeatedRecord") + "  " + PaymentFee.ServiceSubCode);
                    throw _exception;
                }
            }


            if (unPaidPayment.InvoiceNo == null || unPaidPayment.InvoiceNo == "")
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "noInvoiceNumberforAPP"));
                throw _exception;
            }

            var paymentGateAttempts = await _EngineCoreDBContext.PaymentGateAttempt.Where(x => x.PaymentId == unPaidPayment.Id).ToListAsync();
            foreach (var payAttempt in paymentGateAttempts)
            {
                GetPaymentStatusDto GetPaymentStatusDto = new GetPaymentStatusDto()
                {
                    PurchaseId = payAttempt.PaymentAttemptInvoiceNo
                };

                // check from the unpaid invoice number from the pay gate.
                PaymentGetStatusResponse PaymentStatus = await GetPaymentStatus(GetPaymentStatusDto);
                if (PaymentStatus.Status == Constants.SuccessfulCodeStatus || PaymentStatus.Status == Constants.SuccessfulCodeStatusWALLET)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "paidBefore"));
                    throw _exception;
                }
            }

            try
            {

                var attemptInvoiceNo = unPaidPayment.InvoiceNo + (paymentGateAttempts.Count + 1).ToString();
                while (await _EngineCoreDBContext.PaymentGateAttempt.AnyAsync(x => x.PaymentAttemptInvoiceNo == attemptInvoiceNo))
                {
                    attemptInvoiceNo += "1";
                }

                Dictionary<string, string> DictionaryURL = new Dictionary<string, string>
                {
                    { "description", description },
                    { "id", attemptInvoiceNo },
                    { "languageCode", lang },
                    { "paymentType", _paymentSettings.paymentType },
                    { "returnUrl", _paymentSettings.returnURL },
                    { "entityID", _paymentSettings.EntityID.ToString() }
                };
                string StringForHash = _paymentSettings.secretKey + DictionaryURL["description"] + DictionaryURL["id"] + DictionaryURL["languageCode"] + DictionaryURL["paymentType"] + DictionaryURL["returnUrl"] + DictionaryURL["entityID"];
                string secureHashh = _iGeneralRepository.SecureHashGenerationHMACSHA256(StringForHash, _paymentSettings.secretKey);
                DictionaryURL.Add("secureHash", secureHashh);
                unPaidPayment.UserId = UserId;
                unPaidPayment.PaymentType = _paymentSettings.paymentType;
                unPaidPayment.PaymentStatus = "0";
                _EngineCoreDBContext.Update(unPaidPayment);

                if (await _EngineCoreDBContext.SaveChangesAsync() > 0)
                {
                    PaymentGateAttempt paymentGateAttempt = new PaymentGateAttempt
                    {
                        PaidAttemptDate = DateTime.Now,
                        PaymentId = unPaidPayment.Id,
                        SecureHash = secureHashh,
                        PaymentAttemptInvoiceNo = attemptInvoiceNo
                    };
                    _EngineCoreDBContext.PaymentGateAttempt.Add(paymentGateAttempt);
                    await _EngineCoreDBContext.SaveChangesAsync();

                    URlString = _iGeneralRepository.GenerateURL(DictionaryURL, _paymentSettings.URL);
                }

                foreach (PaymentDetails paymentDetail in unPaidPayment.PaymentDetails)
                {
                    URlString += "&price=" + paymentDetail.Price.ToString().Trim();
                    URlString += "&quantity=" + paymentDetail.Quantity.ToString().Trim();
                    URlString += "&serviceCode=" + paymentDetail.ServiceMainCode.Trim() + "-" + paymentDetail.ServiceSubCode.Trim();
                }

                return URlString;
            }

            catch (Exception e)
            {
                _exception.AttributeMessages.Add("Error in the setting of the generate payment URL " + e.Message);
                throw _exception;
            }
        }


        private async Task<int> UpdateAutoPaymentWithDetails(List<PaymentResponseDtoList> myDeserializedClass)
        {
            string InvoiceNo = GetValueOfResponseKey(myDeserializedClass, Constants.ResponseInvoiceNo);
            if (InvoiceNo != null)
            {
                using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                var payAttemptId = await UpdateAutoPaymentTable(myDeserializedClass);
                if (payAttemptId > 0)
                {
                    await UpdateAutoPaymentDetialsTableAsync(GetValueOfResponseKey(myDeserializedClass, Constants.ResponseEServiceData), payAttemptId);
                }
                scope.Complete();
                return payAttemptId;
            }
            return -1;
        }

        private async Task UpdateAutoPaymentDetialsTableAsync(string ResponseEServiceData, int paymentAttemptId)
        {
            var paymentId = await _EngineCoreDBContext.PaymentGateAttempt.Where(x => x.Id == paymentAttemptId).FirstOrDefaultAsync();

            ResponseEServiceData[] ResponseEServiceDataList = JsonConvert.DeserializeObject<ResponseEServiceData[]>(ResponseEServiceData);
            if (paymentId != null && ResponseEServiceDataList != null)
            {
                foreach (ResponseEServiceData responseEServiceData in ResponseEServiceDataList)
                {
                    int indexOfDash = responseEServiceData.mainSubCode.IndexOf("-");
                    int Length = responseEServiceData.mainSubCode.Length;
                    string MainCodeService = responseEServiceData.mainSubCode.Substring(0, indexOfDash);
                    string SubCodeService = responseEServiceData.mainSubCode.Substring(indexOfDash + 1, Length - indexOfDash - 1);

                    double? price = Convert.ToDouble(responseEServiceData.price);
                    int? quantity = Convert.ToInt32(responseEServiceData.quantity);
                    double? ownerFees = Convert.ToDouble(responseEServiceData.ownerFees);
                    double? amountWithFees = Convert.ToDouble(responseEServiceData.amountWithFees);
                    double? amountWithoutFees = Convert.ToDouble(responseEServiceData.amountWithoutFees);

                    PaymentDetails PaymentDetails = await _EngineCoreDBContext.PaymentDetails.Where(x => x.ServiceMainCode == MainCodeService && x.ServiceSubCode == SubCodeService && x.PaymentId == paymentId.PaymentId).FirstOrDefaultAsync();
                    if (PaymentDetails != null)
                    {
                        try
                        {
                            PaymentDetails.OwnerFees = Convert.ToDouble(ownerFees);
                            PaymentDetails.AmountWithFees = Convert.ToDouble(amountWithFees);
                            PaymentDetails.AmountWithoutFees = Convert.ToDouble(amountWithoutFees);
                        }
                        catch
                        {

                        }
                    }
                    _iGeneralRepository.Update(PaymentDetails);
                    await _iGeneralRepository.Save();

                }
            }
        }

        private async Task<int> UpdateAutoPaymentTable(List<PaymentResponseDtoList> myDeserializedClass)
        {
            if (myDeserializedClass == null)
            {
                return 0;
            }

            string IncomingStatus = GetValueOfResponseKey(myDeserializedClass, Constants.ResponseStatus);
            string InvoiceNo = GetValueOfResponseKey(myDeserializedClass, Constants.ResponseInvoiceNo);

            var attemptPay = await _EngineCoreDBContext.PaymentGateAttempt.Where(x => x.PaymentAttemptInvoiceNo == InvoiceNo).FirstOrDefaultAsync();
            if (attemptPay == null)
            {
                return 0;
            }

            var currentPay = await _EngineCoreDBContext.Payment.Where(x => x.Id == attemptPay.PaymentId).FirstOrDefaultAsync();
            if (currentPay == null)
            {
                return 0;
            }

            string SavedStatus = currentPay.Status;

            if (SavedStatus == Constants.SuccessfulCodeStatus || SavedStatus == Constants.SuccessfulCodeStatusWALLET)
            {
                return 0;
            }

            currentPay.Status = GetValueOfResponseKey(myDeserializedClass, Constants.ResponseStatus);
            currentPay.StatusMessage = HttpUtility.UrlDecode(GetValueOfResponseKey(myDeserializedClass, Constants.ResponseStatusMessage));
            currentPay.PaymentMethodType = GetValueOfResponseKey(myDeserializedClass, Constants.ResponsePaymentMethodType);
            currentPay.PaymentStatus = GetPaymentStatus(GetValueOfResponseKey(myDeserializedClass, Constants.ResponseConfirmationID), GetValueOfResponseKey(myDeserializedClass, Constants.ResponseStatus));
            currentPay.TransactionResponseDate = returnResponseDate(myDeserializedClass, Constants.ResponseTransactionResponseDate);

            attemptPay.ConfirmationId = GetValueOfResponseKey(myDeserializedClass, Constants.ResponseConfirmationID);
            attemptPay.Pun = GetValueOfResponseKey(myDeserializedClass, Constants.ResponsePUN);
            attemptPay.CollectionCenterFee = GetValueOfResponseKey(myDeserializedClass, Constants.ResponseCollectionCenterFees);
            attemptPay.EDirhamFee = GetValueOfResponseKey(myDeserializedClass, Constants.ResponseEDirhamFees);
            attemptPay.SecureHash = GetValueOfResponseKey(myDeserializedClass, Constants.ResponseSecureHash);
            attemptPay.EServiceData = "{ \"response Time\": \"" + DateTime.Now.ToString() + "\",{" + JsonConvert.SerializeObject(myDeserializedClass) + "}}";

            try
            {
                if (GetValueOfResponseKey(myDeserializedClass, Constants.ResponseStatus) == Constants.SuccessfulCodeStatus)
                {
                    currentPay.ActualPaid = Convert.ToDouble(GetValueOfResponseKey(myDeserializedClass, Constants.ResponseAmount)) / 100;
                }
            }
            catch
            {
            }

            _EngineCoreDBContext.Payment.Update(currentPay);
            await _EngineCoreDBContext.SaveChangesAsync();

            _EngineCoreDBContext.PaymentGateAttempt.Update(attemptPay);
            await _EngineCoreDBContext.SaveChangesAsync();

            return attemptPay.Id;
        }


        private string GetPaymentStatus(string confirmationId, string status)
        {
            if (status == Constants.SuccessfulCodeStatus || status == Constants.SuccessfulCodeStatusWALLET)
            {
                return Constants.SuccessfullPaymentStatus;
            }
            if (status != Constants.SuccessfulCodeStatus && status != Constants.SuccessfulCodeStatusWALLET && confirmationId == "")
            {
                return Constants.FailedPaymentStatusWithOutConfirmationId;
            }
            if (status != Constants.SuccessfulCodeStatus && status != Constants.SuccessfulCodeStatusWALLET && confirmationId != "")
            {
                return Constants.FailedPaymentStatusWithConfirmationId;
            }
            if (status == Constants.PendingBankCodeStatus)
            {
                return Constants.PandingBankPaymentStatus;
            }
            else return Constants.InitialPaymentStatus;
        }


        public async Task<PaymentGetStatusResponse> GetPaymentStatus(GetPaymentStatusDto GetPaymentStatusDto)
        {
            PaymentServiceClient client = new PaymentServiceClient(PaymentServiceClient.EndpointConfiguration.SOAPEndPoint);
            PaymentGetStatusResponse response = await client.GetPaymentStatusAsync(GetPaymentStatusDto.PurchaseId, null, Convert.ToInt32((_paymentSettings.EntityID)));
            return response;
        }

        public string GetValueOfResponseKey(List<PaymentResponseDtoList> myDeserializedClass, string key)
        {
            PaymentResponseDtoList resul = myDeserializedClass.Where(x => x.key == key).FirstOrDefault();
            if (resul == null || resul.value == null)
            {
                return null;
            }
            return resul.value[0].Trim();
        }

        DateTime? returnResponseDate(List<PaymentResponseDtoList> myDeserializedClass, string key)
        {
            string datePay = GetValueOfResponseKey(myDeserializedClass, Constants.ResponseTransactionResponseDate);
            if (datePay == "") return null;
            DateTime dt = DateTime.ParseExact(datePay, "ddMMyyyyHHmmss", CultureInfo.CurrentCulture);
            DateTime TransactionResponseDate = Convert.ToDateTime(dt.ToString("yyyy-MM-dd HH:mm:ss tt"));
            return TransactionResponseDate;
        }

        public async Task<List<PaymentDto>> GetMyPayment(int id, string lang)
        {
            List<PaymentDto> PaymentResult = await _EngineCoreDBContext.Payment.Where(x => x.UserId == id).Select(y => new PaymentDto
            {
                Id = y.Id,
                PaymentStatus = y.PaymentStatus == Constants.SuccessfullPaymentStatus,
                ServiceName = _iGeneralRepository.getServiceNameTranslateAsync(lang, y.Id).Result,
                InvoiceNo = y.InvoiceNo,
                DatePayment = y.TransactionResponseDate,
                Total = y.TotalAmount,
                Message = y.StatusMessage,
                Status = y.Status,
                ApplicationNo = (int)y.ApplicationId

            }).ToListAsync();
            return PaymentResult.OrderByDescending(x => x.DatePayment).ToList(); ;
        }

        public async Task<PaymentDto> GetPaymentInfo(int PaymentId, string lang)
        {
            PaymentDto PaymentResult = await _EngineCoreDBContext.Payment.Where(x => x.Id == PaymentId).Select(y => new PaymentDto
            {
                Id = y.Id,
                PaymentStatus = y.PaymentStatus == Constants.SuccessfullPaymentStatus,
                ServiceName = _iGeneralRepository.getServiceNameTranslateAsync(lang, y.Id).Result,
                InvoiceNo = y.InvoiceNo,
                DatePayment = y.TransactionResponseDate,
                Total = y.TotalAmount,
                Message = y.StatusMessage,
                ApplicationNo = (int)y.ApplicationId,
                Status = y.Status,
                UserName = (from k in _EngineCoreDBContext.User.Where(z => z.Id == y.UserId)
                            select new { UserName = k.FullName }
                                ).Select(v => v.UserName).FirstOrDefault(),
            }).FirstOrDefaultAsync();
            return PaymentResult;
        }

        public async Task<PaymentDetialsForAPPDto> GetPaymentsInfoByAppId(int applicationId, string lang)
        {
            PaymentDetialsForAPPDto result = new PaymentDetialsForAPPDto();

            if (lang == null || lang == "")
            {
                lang = "ar";
            }

            var application = await _EngineCoreDBContext.Application.Where(x => x.Id == applicationId).FirstOrDefaultAsync();
            if (application == null)
            {
                result.PaymentStatus = Constants.PAYMENT_STATUS_ENUM.NOPAYMENT;
                result.PaymentStatusName = (lang == "ar") ? Constants.PAYMENT_STATUS_NOPAYMENT_AR : Constants.PAYMENT_STATUS_NOPAYMENT_EN;
                return result;
            }

            var payments = await _EngineCoreDBContext.Payment.Include(x => x.PaymentDetails).Include(x => x.PaymentGateAttempt).Where(x => x.ApplicationId == applicationId).ToListAsync();
            if (payments == null || payments.Count == 0)
            {
                result.PaymentStatus = Constants.PAYMENT_STATUS_ENUM.NOPAYMENT;
                result.PaymentStatusName = (lang == "ar") ? Constants.PAYMENT_STATUS_NOPAYMENT_AR : Constants.PAYMENT_STATUS_NOPAYMENT_EN;
                return result;
            }

            try
            {
                // Recheck the payment web attempts if paid, TODO Check EPOS currently all attempts are web..
                var unPaidstatusWeb = payments.Where(x => x.Status != Constants.SuccessfulCodeStatus /* && x.PaymentType == _paymentSettings.paymentType*/).ToList();
                foreach (var checkWebGatePayment in unPaidstatusWeb)
                {
                    foreach (var payementWebAttempt in checkWebGatePayment.PaymentGateAttempt)
                    {
                        GetPaymentStatusDto GetPaymentStatusDto = new GetPaymentStatusDto()
                        {
                            PurchaseId = payementWebAttempt.PaymentAttemptInvoiceNo
                        };

                        PaymentGetStatusResponse PaymentGetStatusResponse = await GetPaymentStatus(GetPaymentStatusDto);
                        // update payment attempt record.
                        payementWebAttempt.CollectionCenterFee = PaymentGetStatusResponse.CollectionCenterFees;
                        payementWebAttempt.EDirhamFee = PaymentGetStatusResponse.EDirhamFees;
                        payementWebAttempt.EServiceData = "{ \"response Time\": \"" + DateTime.Now.ToString() + "\",{" + JsonConvert.SerializeObject(PaymentGetStatusResponse) + "}}";
                        payementWebAttempt.Pun = PaymentGetStatusResponse.PUN;
                        payementWebAttempt.ConfirmationId = PaymentGetStatusResponse.ConfirmationID;
                        _EngineCoreDBContext.PaymentGateAttempt.Update(payementWebAttempt);
                        await _EngineCoreDBContext.SaveChangesAsync();

                        // success payment.
                        bool updateOnlyOneScuessPay = true;
                        if (updateOnlyOneScuessPay && PaymentGetStatusResponse != null && PaymentGetStatusResponse.InvoiceID != null && PaymentGetStatusResponse.InvoiceID == GetPaymentStatusDto.PurchaseId && PaymentGetStatusResponse.Status == Constants.SuccessfulCodeStatus)
                        {
                            //await UpdateSuccessPayment(PaymentGetStatusResponse, unPaid.Id);
                            updateOnlyOneScuessPay = false;
                            checkWebGatePayment.ActualPaid = Convert.ToDouble(PaymentGetStatusResponse.Amount) / 100;
                            checkWebGatePayment.InvoiceNo = PaymentGetStatusResponse.InvoiceID;
                            string datePay = PaymentGetStatusResponse.TransactionResponseDate;
                            DateTime dt = DateTime.ParseExact(datePay, "ddMMyyyyHHmmss", CultureInfo.CurrentCulture);
                            checkWebGatePayment.TransactionResponseDate = Convert.ToDateTime(dt.ToString("yyyy-MM-dd HH:mm:ss tt"));

                            checkWebGatePayment.Status = PaymentGetStatusResponse.Status;
                            checkWebGatePayment.PaymentStatus = Constants.SuccessfullPaymentStatus;
                            checkWebGatePayment.StatusMessage = PaymentGetStatusResponse.StatusMessage;

                            _EngineCoreDBContext.Payment.Update(checkWebGatePayment);
                            await _EngineCoreDBContext.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("error in updating payments for checking web payment attempts " + ex.Message);
            }
            //Finish the payments is paid.

            var unPaidstatus = payments.Where(x => x.Status == "0" || x.Status == "-1" || x.Status == Constants.InitialPaymentStatus || x.Status == Constants.InitialPaymentStatusBeforPay || x.Status == Constants.InitialStatus || x.Status == Constants.InitialStatusBeforPay).ToList().Count;
            if (unPaidstatus == payments.Count)
            {
                result.PaymentStatus = Constants.PAYMENT_STATUS_ENUM.UNPAID;
                result.PaymentStatusName = (lang == "ar") ? Constants.PAYMENT_STATUS_READY_FOR_PAID_AR : Constants.PAYMENT_STATUS_READY_FOR_PAID_EN;
            }
            else
            {
                var paidstatus = payments.Where(x => x.Status == "1" || x.Status == Constants.SuccessfullPaymentStatus || x.Status == Constants.SuccessfulCodeStatus).ToList().Count;
                if (paidstatus == payments.Count)
                {
                    result.PaymentStatus = Constants.PAYMENT_STATUS_ENUM.PAID;
                    result.PaymentStatusName = (lang == "ar") ? Constants.PAYMENT_STATUS_PAID_AR : Constants.PAYMENT_STATUS_PAID_EN;

                }
                else
                {
                    result.PaymentStatus = Constants.PAYMENT_STATUS_ENUM.PartialPaid;
                    result.PaymentStatusName = (lang == "ar") ? Constants.PAYMENT_STATUS_PARTIAL_AR : Constants.PAYMENT_STATUS_PARTIAL_EN;
                }
            }

            DateTime payDate = DateTime.Now;
            double total = 0;
            foreach (var payment in payments)
            {
                total += (double)payment.TotalAmount;
                PaymentsDates paymentsDates = new PaymentsDates()
                {
                    InvoicesDates = payment.PaymentDate.ToString(),
                    InvoicesNumbers = payment.InvoiceNo
                };

                result.PaymentsDates.Add(paymentsDates);

                var paymentDetails = payment.PaymentDetails;
                foreach (var paymentDetail in paymentDetails)
                {
                    var existBefore = result.ApplicationFees.Where(x => x.SubCalss == paymentDetail.ServiceSubCode).FirstOrDefault();

                    if (existBefore != null)
                    {
                        existBefore.Quantity += (int)paymentDetail.Quantity;
                        existBefore.FeeValue += (int)paymentDetail.Price;
                    }
                    else
                    {
                        var fee = await _EngineCoreDBContext.TransactionFee.Where(x => x.SubClass == paymentDetail.ServiceSubCode).FirstOrDefaultAsync();
                        var feeName = await _iGeneralRepository.GetTranslateByShortCut(lang, fee.TransactionNameShortcut);
                        result.ApplicationFees.Add(new DTOs.TransactionFeeDto.TransactionFeeOutput
                        {
                            Quantity = (int)paymentDetail.Quantity,
                            FeeNo = fee.Id,
                            FeeName = await _iGeneralRepository.GetTranslateByShortCut(lang, fee.TransactionNameShortcut),
                            FeeValue = (double)paymentDetail.Price,
                            SubCalss = paymentDetail.ServiceSubCode,
                            PrimeClass = paymentDetail.ServiceMainCode
                        });

                    }
                }
            }

            result.Total = total;
            return result;
        }

        public string RedirectMethod(string Host, Dictionary<string, string> DictionaryURL) //build URL with query string
        {
            string URlString = null;
            var array = (
              from key in DictionaryURL.Keys
              select string.Format(
                  "{0}={1}",
                  key,
                 DictionaryURL[key])
              ).ToArray();
            URlString = string.Join("&", array);
            URlString = Host + "?" + HttpUtility.UrlPathEncode(URlString);
            URlString = HttpUtility.UrlPathEncode(URlString);
            return URlString;
        }
        public string RedirectMethodWithPathPara(string Host, List<string> DictionaryURL)
        {
            string URL = Host;
            foreach (string str in DictionaryURL)
            {
                URL += "/" + HttpUtility.UrlPathEncode(str);
            }
            return URL;
        }

        public string GenerateInvoiceNo()
        {
            string InvoiceNo = _paymentSettings.PreInvoiceNo + _iGeneralRepository.GetNextSecForPayment();
            return InvoiceNo;
        }

        public string getResponseHash(List<PaymentResponseDtoList> myDeserializedClass)
        {
            string StringToHash = getStringToHash(myDeserializedClass);
            string key = "76a0ed27b643bc6652273f29df66e522";
            string ResponseHash = _iGeneralRepository.SecureHashGenerationHMACSHA256(StringToHash, key);
            return ResponseHash;
        }

        public string getStringToHash(List<PaymentResponseDtoList> myDeserializedClass)
        {
            string key = "76a0ed27b643bc6652273f29df66e522";
            StringBuilder StringToHash = new StringBuilder();
            StringToHash.Append(key);
            PaymentResponseDtoList para = myDeserializedClass.Where(x => x.key == "Response.EServiceData").FirstOrDefault();

            if (para != null) para.value = new string[] { GetEServiceString(para) };

            foreach (PaymentResponseDtoList kvp in myDeserializedClass.OrderBy(key => key.key, StringComparer.Ordinal).ToList())
            {
                if (kvp.value[0] != null && kvp.value[0] != "")
                    StringToHash.Append(kvp.value[0]);
            }

            return StringToHash.ToString();
        }

        private string GetEServiceString(PaymentResponseDtoList para)
        {
            EServiceSubDataAfterOrder EServiceSubDataAfterOrder = new EServiceSubDataAfterOrder();
            EServiceSubData[] json = JsonConvert.DeserializeObject<EServiceSubData[]>(para.value[0]);
            string EServiceString = null;
            foreach (EServiceSubData Subjson in json)
            {
                EServiceSubDataAfterOrder.ownerFees = new string[] { Subjson.ownerFees };
                EServiceSubDataAfterOrder.mainSubCode = new string[] { Subjson.mainSubCode };
                EServiceSubDataAfterOrder.price = new string[] { Subjson.price };
                EServiceSubDataAfterOrder.amountWithFees = new Int32[] { Convert.ToInt32(Subjson.amountWithFees) };
                EServiceSubDataAfterOrder.quantity = new string[] { Subjson.quantity };
                EServiceSubDataAfterOrder.amountWithoutFees = new Int32[] { Convert.ToInt32(Subjson.amountWithoutFees) };
                EServiceString += new JavaScriptSerializer().Serialize(new EServiceSubDataAfterOrder[] { EServiceSubDataAfterOrder }).Replace("\"", "'");
            }

            return EServiceString;
        }


        private async Task CkeckPaymentForAppIfValid(PaymentDetailsDto paymentDetailsDto, int serviceNo, string lang)
        {
            var allServiceFee = await _EngineCoreDBContext.ServiceFee.Where(x => x.ServiceNo == serviceNo).ToListAsync();
            if (allServiceFee == null || allServiceFee.Count == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "FeesServiceNotFound"));
                throw _exception;
            }

            var requiredFees = allServiceFee.Where(x => x.Required == true).Select(x => x.FeeNo).ToList();
            if (requiredFees == null || requiredFees.Count == 0)
            {
                return;
            }

            var requiredSubServiceNumbers = await _EngineCoreDBContext.TransactionFee.Where(x => requiredFees.Contains(x.Id)).Select(x => x.SubClass).Distinct().ToListAsync();

            if (requiredSubServiceNumbers != null && requiredSubServiceNumbers.Count > 0)
            {
                foreach (var requiredSubServiceNo in requiredSubServiceNumbers)
                {
                    if (!paymentDetailsDto.FeeList.Any(x => x.ServiceSubCode == requiredSubServiceNo))
                    {
                        _exception.AttributeMessages.Add(Constants.getMessage(lang, "missedFeeForService"));
                        throw _exception;
                    }
                }
            }
        }

        public async Task AddApplicationFeesAsync(PaymentDetailsDto PaymentDetailsDto, string lang)
        {
            var application = await _EngineCoreDBContext.Application.Where(x => x.Id == Convert.ToInt32(PaymentDetailsDto.ApplicationId)).FirstOrDefaultAsync();
            if (application == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ApplicationNotFound"));
                throw _exception;
            }

            if (application.ServiceId == null)
            {
                _exception.AttributeMessages.Add("NoServiceAttachedTOTheApplicationAskTheAdmin");
                throw _exception;
            }

            if (PaymentDetailsDto.FeeList == null || PaymentDetailsDto.FeeList.Count() == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "FeesListNotFound"));
                throw _exception;
            }


            foreach (var PaymentFee in PaymentDetailsDto.FeeList)
            {
                if (!await _EngineCoreDBContext.TransactionFee.AnyAsync(x => x.PrimeClass == PaymentFee.ServiceMainCode && x.SubClass == PaymentFee.ServiceSubCode))
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "unUsedFee"));
                    throw _exception;
                }

                if (PaymentFee.Price <= 0 || PaymentFee.Quantity <= 0)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "zeroNotAllowed"));
                    throw _exception;
                }

                if (PaymentDetailsDto.FeeList.Where(x => x.ServiceSubCode == PaymentFee.ServiceSubCode).Count() > 1)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "repeatedRecord") + "  " + PaymentFee.ServiceSubCode);
                    throw _exception;
                }
            }

            if (!await _EngineCoreDBContext.User.AnyAsync(x => x.Id == Convert.ToInt32(PaymentDetailsDto.UserId)))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }


            // check if the final list of paid and to paid fees to the application are correct.
            await CkeckPaymentForAppIfValid(PaymentDetailsDto, (int)application.ServiceId, lang);

            var oldPayments = await _EngineCoreDBContext.Payment.Where(x => x.ApplicationId == PaymentDetailsDto.ApplicationId).ToListAsync();

            List<PaymentGateAttempt> PrevPaymentGateAttempts = new List<PaymentGateAttempt>();

            if (oldPayments != null)
            {
                // remove unpaid payments.
                var unpaidPayment = oldPayments.Where(x => x.PaymentStatus != Constants.SuccessfullPaymentStatus && x.Status != Constants.SuccessfulCodeStatus).ToList();
                if (unpaidPayment.Count > 0)
                {
                    foreach (var unPaid in unpaidPayment)
                    {
                        var paymentAttempts = await _EngineCoreDBContext.PaymentGateAttempt.Where(x => x.PaymentId == unPaid.Id).ToListAsync();
                        foreach (var paymentAttempt in paymentAttempts)
                        {
                            PaymentGateAttempt paymentGateAttempt = new PaymentGateAttempt
                            {
                                PaidAttemptDate = paymentAttempt.PaidAttemptDate,
                                PaymentAttemptInvoiceNo = paymentAttempt.PaymentAttemptInvoiceNo
                            };
                            PrevPaymentGateAttempts.Add(paymentGateAttempt);
                        }

                        _EngineCoreDBContext.PaymentGateAttempt.RemoveRange(paymentAttempts);
                        await _EngineCoreDBContext.SaveChangesAsync();


                        var unpaidDetails = await _EngineCoreDBContext.PaymentDetails.Where(x => x.PaymentId == unPaid.Id).ToListAsync();
                        _EngineCoreDBContext.PaymentDetails.RemoveRange(unpaidDetails);
                        await _EngineCoreDBContext.SaveChangesAsync();

                        _EngineCoreDBContext.Payment.Remove(unPaid);
                        await _EngineCoreDBContext.SaveChangesAsync();
                    }
                }

                var paidPayments = oldPayments.Where(x => x.Status == Constants.SuccessfullPaymentStatus || x.Status == Constants.SuccessfulCodeStatus || x.Status == Constants.SuccessfulCodeStatusWALLET).ToList();
                if (paidPayments != null && paidPayments.Count > 0)
                {
                    // get Paid, remove extra.
                    List<FeesDto> oldPaid = new List<FeesDto>();
                    foreach (var oldPaidPay in paidPayments)
                    {
                        var oldPaidPayDetails = await _EngineCoreDBContext.PaymentDetails.Where(x => x.PaymentId == oldPaidPay.Id).ToListAsync();
                        foreach (var oldPaidPayDetail in oldPaidPayDetails)
                        {
                            var det = oldPaid.Where(x => x.ServiceSubCode == oldPaidPayDetail.ServiceSubCode).FirstOrDefault();
                            if (det != null)
                            {
                                det.Quantity += (int)oldPaidPayDetail.Quantity;
                                det.Price += (int)oldPaidPayDetail.Price;
                            }
                            else
                            {
                                oldPaid.Add(new FeesDto
                                {
                                    Price = oldPaidPayDetail.Price,
                                    Quantity = (int)oldPaidPayDetail.Quantity,
                                    ServiceSubCode = oldPaidPayDetail.ServiceSubCode,
                                    ServiceMainCode = oldPaidPayDetail.ServiceMainCode
                                });
                            }
                        }
                    }

                    if (oldPaid.Count > 0)
                    {
                        foreach (var newFee in PaymentDetailsDto.FeeList)
                        {
                            var det = oldPaid.Where(x => x.ServiceSubCode == newFee.ServiceSubCode).FirstOrDefault();
                            if (det != null)
                            {
                                newFee.Quantity -= det.Quantity;
                                newFee.Price -= det.Price;
                            }
                        }

                        PaymentDetailsDto.FeeList.RemoveAll(x => x.Quantity <= 0 || x.Price <= 0);

                        if (PaymentDetailsDto.FeeList.Count == 0)
                        {
                            return;
                        }
                    }

                }
            }

            double totalAmount = 0;
            foreach (FeesDto fees in PaymentDetailsDto.FeeList)
            {
                totalAmount += (double)fees.Price;
            }
            int payID = 0;

            Models.Payment CurrentPayment = new Models.Payment()
            {
                ActionId = PaymentDetailsDto.ActionId,
                ActualPaid = 0,
                ApplicationId = application.Id,
                CreatedDate = DateTime.Now,
                CreatedBy = _IUserRepository.GetUserID(),
                InvoiceNo = GenerateInvoiceNo(),
                PaymentDate = DateTime.Now,
                PaymentStatus = Constants.InitialPaymentStatusBeforPay,
                PaymentSource = 0.ToString(),
                Printed = false,
                ReceiptNo = "PAY_RECEIPTNO" + _iGeneralRepository.GetNextSecForPayment(),
                ServiceId = application.ServiceId,
                Status = Constants.InitialPaymentStatusBeforPay,
                TotalAmount = totalAmount,
                UserId = PaymentDetailsDto.UserId
            };

            await _EngineCoreDBContext.AddAsync(CurrentPayment);
            if (await _EngineCoreDBContext.SaveChangesAsync() <= 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "failedAddPayment"));
                throw _exception;
            }

            payID = CurrentPayment.Id;

            // add previous payment attempts.
            if (PrevPaymentGateAttempts != null && PrevPaymentGateAttempts.Count > 0)
            {
                foreach (var payAttempt in PrevPaymentGateAttempts)
                {
                    payAttempt.PaymentId = payID;
                }
                await _EngineCoreDBContext.PaymentGateAttempt.AddRangeAsync(PrevPaymentGateAttempts);
                await _EngineCoreDBContext.SaveChangesAsync();
            }


            // add payment details.
            if (PaymentDetailsDto.FeeList.Count() > 0)
            {
                List<PaymentDetails> ApplicationFees = new List<PaymentDetails>();
                foreach (var PaymentFee in PaymentDetailsDto.FeeList)
                {
                    PaymentDetails payment = new PaymentDetails()
                    {
                        ServiceMainCode = PaymentFee.ServiceMainCode,
                        ServiceSubCode = PaymentFee.ServiceSubCode,
                        Quantity = PaymentFee.Quantity,
                        Price = PaymentFee.Price,
                        PaymentId = payID
                    };
                    ApplicationFees.Add(payment);
                }

                await _EngineCoreDBContext.PaymentDetails.AddRangeAsync(ApplicationFees);
                if (await _EngineCoreDBContext.SaveChangesAsync() <= 0)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "failedAddPayment"));
                    throw _exception;
                }
            }
        }


        public async Task<bool> DeleteApplicationFeesAsync(int Id)
        {
            List<PaymentDetails> ApplicationFeesList = await _EngineCoreDBContext.PaymentDetails.Where(x => x.Id == Id).ToListAsync();
            if (ApplicationFeesList.Count == 0) throw new InvalidOperationException(" Id not found !!!!");
            _EngineCoreDBContext.PaymentDetails.RemoveRange(ApplicationFeesList);
            _EngineCoreDBContext.SaveChanges();
            return true;
        }

        public async Task<List<GetFees>> GetApplicationFeesByIdAsync(int Id, string lang)
        {
            List<GetFees> result = new List<GetFees>();
            var pay = await _EngineCoreDBContext.Payment.Include(x => x.PaymentDetails).Where(x => x.ApplicationId == Id).FirstOrDefaultAsync();
            if (pay != null)
            {
                foreach (var detail in pay.PaymentDetails)
                {
                    var feeName = await _EngineCoreDBContext.TransactionFee.Where(x => x.SubClass == detail.ServiceSubCode).FirstOrDefaultAsync();
                    GetFees getFees = new GetFees
                    {
                        Id = detail.Id,
                        ServiceMainCode = detail.ServiceMainCode,
                        ServiceSubCode = detail.ServiceSubCode,
                        Price = detail.Price,
                        Quantity = (int)detail.Quantity,
                        AmountWithFees = detail.AmountWithFees,
                        AmountWithoutFees = detail.AmountWithoutFees,
                        OwnerFees = detail.OwnerFees,
                        FeeName = await _iGeneralRepository.GetTranslateByShortCut(lang, feeName.TransactionNameShortcut)

                    };
                    result.Add(getFees);
                }
            }
            return result;
        }


        public async Task<List<GetFees>> GetApplicationFeesAsync()
        {
            List<GetFees> result = await _EngineCoreDBContext.PaymentDetails.Select(x =>
                 new GetFees
                 {
                     Id = x.Id,
                     ServiceMainCode = x.ServiceMainCode,
                     ServiceSubCode = x.ServiceSubCode,
                     Price = x.Price,
                     Quantity = (int)x.Quantity,
                     AmountWithFees = x.AmountWithFees,
                     AmountWithoutFees = x.AmountWithoutFees,
                     OwnerFees = x.OwnerFees
                 }
                ).ToListAsync();

            return result;
        }


        public async Task<List<PaymentDetails>> GetApplicationFeesByAppIdAsync(int AppId)
        {
            List<PaymentDetails> res = new List<PaymentDetails>();
            var pay = await _EngineCoreDBContext.Payment.Include(x => x.PaymentDetails).Where(x => x.ApplicationId == AppId).FirstOrDefaultAsync();
            if (pay != null)
            {
                res = pay.PaymentDetails.ToList();
            }

            return res;
        }

        public async Task<string> AutoPay(List<PaymentResponseDtoList> paymentResponseDtoList, string lang)
        {
            var PaymentAttemptId = await UpdateAutoPaymentWithDetails(paymentResponseDtoList);
            List<string> DictionaryURL = new List<string>();
            if (PaymentAttemptId == -1)
            {
                string ResponseStatus = GetValueOfResponseKey(paymentResponseDtoList, Constants.ResponseStatus);
                string ResponseStatusMessage = GetValueOfResponseKey(paymentResponseDtoList, Constants.ResponseStatusMessage);

                _exception.AttributeMessages.Add("Response Status :" + ResponseStatus + " ,Response Status Message:" + ResponseStatusMessage);
                throw _exception;
            }
            else
            {

                var paymentAttempt = await _EngineCoreDBContext.PaymentGateAttempt.Where(x => x.Id == PaymentAttemptId).FirstOrDefaultAsync();
                if (paymentAttempt == null)
                {
                    _exception.AttributeMessages.Add("payment invoice not fount in payments details gate attempts :" + PaymentAttemptId.ToString());
                    throw _exception;
                }

                var Payment = await _EngineCoreDBContext.Payment.Where(x => x.Id == paymentAttempt.PaymentId).FirstOrDefaultAsync();

                // DictionaryURL.Add(Payment.ServiceId.ToString());
                DictionaryURL.Add(Payment.ApplicationId.ToString() + "$");
                int AppId = (int)Payment.ApplicationId;

                string Status = GetValueOfResponseKey(paymentResponseDtoList, Constants.ResponseStatus);

                if (Status == Constants.SuccessfulCodeStatus)
                {
                    await RedirectAfterPay(AppId, Convert.ToInt32(Payment.UserId));
                }

                return RedirectMethodWithPathPara(_paymentSettings.redirectToURL, DictionaryURL);
            }
        }

        public async Task<string> ManualPay(ManualPaymentDetialsDto manualPaymentDetials, string lang)
        {
            if (manualPaymentDetials.PayDate == null)
            {
                _exception.AttributeMessages.Add("Please add valid payment date.");
                throw _exception;
            }

            if (!await _EngineCoreDBContext.User.AnyAsync(x => x.Id == manualPaymentDetials.UserId))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            var application = await _EngineCoreDBContext.Application.Where(x => x.Id == manualPaymentDetials.AppId).FirstOrDefaultAsync();
            if (application == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ApplicationNotFound"));
                throw _exception;
            }

            var payment = await _EngineCoreDBContext.Payment.Where(x => x.ApplicationId == manualPaymentDetials.AppId).FirstOrDefaultAsync();
            if (payment == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "nopaymentsforAPP"));
                throw _exception;
            }

            List<Models.PaymentDetails> PaymentDetails = await _EngineCoreDBContext.PaymentDetails.Where(x => x.PaymentId == payment.Id).ToListAsync();

            if (PaymentDetails == null || PaymentDetails.Count == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "nopaymentsforAPP"));
                throw _exception;
            }

            payment.ActualPaid = manualPaymentDetials.TotalAmount;
            payment.TransactionResponseDate = manualPaymentDetials.PayDate;
            payment.ReceiptNo = "PAY_RECEIPTNO_MANUAL_" + manualPaymentDetials.ReceiptNo;

            if (await _EngineCoreDBContext.Payment.AnyAsync(x => x.ReceiptNo == payment.ReceiptNo))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ExistingReceiptNumber")); //"Existed before PAY_RECEIPTNO_MANUAL_" + payment.ReceiptNo An existing receipt number
                throw _exception;
            }

            payment.InvoiceNo = "PAY_MANUAL_" + manualPaymentDetials.ReceiptNo;

            if (await _EngineCoreDBContext.Payment.AnyAsync(x => x.InvoiceNo == payment.InvoiceNo))
            {
                _exception.AttributeMessages.Add("Existed before PAY_INVOICE_MANUAL_" + payment.InvoiceNo);
                throw _exception;
            }

            payment.Status = Constants.SuccessfulCodeStatus;
            payment.StatusMessage = Constants.getMessage(lang, "PaidSuccess"); ;
            payment.PaymentStatus = Constants.SuccessfullPaymentStatus;
            payment.PaymentType = "MANUAL_PAY";
            payment.PaymentSource = "MANUAL_PAY";
            payment.PaymentMethodType = "MANUAL_PAY";
            payment.UserId = manualPaymentDetials.UserId;
            payment.LastUpdatedDate = DateTime.Now;
            payment.EndEffectiveDate = DateTime.Now;

            try
            {
                _EngineCoreDBContext.Update(payment);
                await _EngineCoreDBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _exception.AttributeMessages.Add("Failed in Manual payment for " + ex.Message);
                throw _exception;
            }

            await RedirectAfterPay(manualPaymentDetials.AppId, manualPaymentDetials.UserId);
            return payment.ReceiptNo;
        }

        private async Task RedirectAfterPay(int appId, int userId)
        {
            ApplicationRepositiory applicationRepositiory = new ApplicationRepositiory(_EngineCoreDBContext, _IBlockChain, _iGeneralRepository, _iNotificationSettingRepository,
                                                                                       _IFilesUploaderRepository, _IConfiguration, _ITransactionRepository, 
                                                                                       _IApplicationPartyRepository, _IUserRepository, _ISendNotificationRepository, 
                                                                                       _IAdmServiceRepository, this, _ISysValueRepository, _IApplicationTrackRepository,
                                                                                       _ITemplateRepository, _loggerForApplication);

            if (await applicationRepositiory.MakeItDone(appId, userId) == Constants.ERROR)
            {
                _exception.AttributeMessages.Add(String.Format("failed in changing the application {0} to performed, please ask the admin to fix.", appId));
                throw _exception;
            }

            var transactions = await _ITransactionRepository.GetAll(appId);
            if (transactions.Count > 0)
            {
                string path = _IConfiguration["TransactionFolder"];
                int transactionId = transactions[0].Id;
                if (transactions[0].TransactionNo != null && transactions[0].TransactionNo.Length > 0)
                {
                    TransactionOldVersionDto dto = new TransactionOldVersionDto
                    {
                        TransactionId = transactions[0].Id,
                        TransactionCreatedDate = transactions[0].TransactionCreatedDate != null ? (DateTime)transactions[0].TransactionCreatedDate : System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                        DocumentUrl = transactions[0].DocumentUrl == null ? "" : transactions[0].DocumentUrl,
                        TransactionNo = transactions[0].TransactionNo
                    };

                    int i = await _ITransactionRepository.AddOldVersion(dto, userId);
                }

                TransactionDto transactionDto = _ITransactionRepository.FromObjectToDto(transactions[0]);
                transactionDto.TransactionNo = _ITransactionRepository.GenerateTransactionNo();

                if (await _ITransactionRepository.Update(transactionId, userId, transactionDto) == Constants.ERROR)
                {
                    _exception.AttributeMessages.Add(String.Format("failed in changing the transaction for the application {0}, please ask the admin to fix.", appId));
                    throw _exception;
                }


                try
                {
                    GeneratorRepository generator = new GeneratorRepository(_EngineCoreDBContext, applicationRepositiory, this, _iGeneralRepository, _FileNaming, _IConverter, _Pdfdocumentsetting, _IFilesUploaderRepositiory);
                    AutoCreatePdfPaths paths = await generator.autoCreatePDFAsync("en", appId, path);

                    if (paths.TransactionDoc != null && paths.TransactionDoc.Length > 0)
                    {
                        var app = await _EngineCoreDBContext.Application.Where(x => x.Id == appId).FirstOrDefaultAsync();
                        string destination = Path.Combine(_IConfiguration["BaseFolder"], app.ServiceId.ToString(), appId.ToString(), paths.TransactionDoc);
                        bool m = applicationRepositiory.MoveAppAttachment(paths.TransactionDoc, destination);
                        transactionDto.DocumentUrl = destination;
                        transactionDto.TransactionCreatedDate = DateTime.Now;
                        await _ITransactionRepository.Update(transactionId, userId, transactionDto);
                    }

                    if (paths.RecordIdPaths.Count > 0)
                    {
                        foreach (KeyValuePair<int, string> entry in paths.RecordIdPaths)
                        {
                            AppRelatedContentDto appdto = new AppRelatedContentDto();
                            AppRelatedContent a = await applicationRepositiory.GetOneRelatedContent(entry.Key);
                            appdto.AppId = a.AppId;
                            appdto.Content = a.Content;
                            appdto.TitleShortcut = a.TitleShortcut;
                            appdto.ContentUrl = entry.Value;
                            await applicationRepositiory.UpdateRContent(entry.Key, appdto);
                        }
                    }
                 
                }
                catch (Exception ex)
                {
                    _exception.AttributeMessages.Add(String.Format("failed in generating PDF for the application {0}, ", appId));
                    _exception.AttributeMessages.Add(ex.Message);
                    throw _exception;
                }

                try
                {
                    var documentType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "document_type").FirstOrDefault();
                    int documentTypeId = documentType.Id;
                    int pOAId = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == documentTypeId)
                                 join tr in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                                 where tr.Value.Contains("Power of Attorney")
                                 select new { id = lv.Id }
                        ).FirstOrDefault().id;


                    var appt = _EngineCoreDBContext.Application.Where(x => x.Id == appId).FirstOrDefault();
                    if (appt != null)
                    {
                        Template temp = _EngineCoreDBContext.Template.Where(x => x.Id == appt.TemplateId).FirstOrDefault();
                        if (temp != null)
                        {
                            if (temp.DocumentTypeId == pOAId)
                            {
                                var appTransaction = _EngineCoreDBContext.AppTransaction.Where(x => x.ApplicationId == appId).FirstOrDefault();
                                appTransaction.TransactionRefNo = "1234567890hh115688914015890" + appId.ToString();
                                _iGeneralRepository.Update(appTransaction);
                                if (await _iGeneralRepository.Save())
                                {
                                    _IBlockChain.GetVcID(appId);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

        }



        ////////////////////crons
        private async Task UpdateSuccessPayment(PaymentGetStatusResponse PaymentGetStatusResponse, int PaymentId)
        {
            Models.Payment CurrentPay = await _EngineCoreDBContext.Payment.Where(x => x.Id == PaymentId).FirstOrDefaultAsync();
            if (CurrentPay != null)
            {
                try
                {
                    try
                    {
                        if (PaymentGetStatusResponse.Status == Constants.SuccessfulCodeStatus)
                        {
                            CurrentPay.ActualPaid = Convert.ToDouble(PaymentGetStatusResponse.Amount) / 100;
                        }
                    }
                    catch
                    {
                    }

                    CurrentPay.InvoiceNo = PaymentGetStatusResponse.InvoiceID;

                    string datePay = PaymentGetStatusResponse.TransactionResponseDate;
                    DateTime dt = DateTime.ParseExact(datePay, "ddMMyyyyHHmmss", CultureInfo.CurrentCulture);
                    CurrentPay.TransactionResponseDate = Convert.ToDateTime(dt.ToString("yyyy-MM-dd HH:mm:ss tt"));

                    CurrentPay.Status = PaymentGetStatusResponse.Status;
                    CurrentPay.PaymentStatus = Constants.SuccessfullPaymentStatus;
                    CurrentPay.StatusMessage = PaymentGetStatusResponse.StatusMessage;

                    _iGeneralRepository.Update(CurrentPay);
                    if (await _iGeneralRepository.Save() && PaymentGetStatusResponse.Status == Constants.SuccessfulCodeStatus)
                    {
                        await RedirectAfterPay((int)CurrentPay.ApplicationId, (int)CurrentPay.UserId);
                    }

                }
                catch
                {


                }
            }
        }

        public async Task UpdatePaymentTableCrons()
        {
            var result = await _EngineCoreDBContext.Payment.Where(x => x.Status != Constants.SuccessfulCodeStatus && x.Status != Constants.SuccessfulCodeStatusWALLET && x.PaymentType == _paymentSettings.paymentType).ToListAsync();

            foreach (var unPaid in result)
            {
                try
                {
                    if (unPaid.PaymentDate == null || DateTime.Compare((DateTime)unPaid.PaymentDate, DateTime.Now.AddMinutes(-2)) >= 0)
                    {
                        continue;
                    }
                    DateTime payDate = (DateTime)unPaid.PaymentDate;

                    if ((DateTime.Now > payDate.AddDays(7)) && Constants.PandingBankPaymentStatus == unPaid.Status)
                    {
                        continue;
                    }

                    var paymentAttempts = await _EngineCoreDBContext.PaymentGateAttempt.Where(x => x.PaymentId == unPaid.Id).ToListAsync();

                    if (paymentAttempts != null && paymentAttempts.Count > 0)
                    {
                        foreach (var payAttempt in paymentAttempts)
                        {
                            if (payAttempt.Pun != null || payAttempt.ConfirmationId != null || payAttempt.CollectionCenterFee != null)
                            {
                                continue;
                            }

                            GetPaymentStatusDto GetPaymentStatusDto = new GetPaymentStatusDto()
                            {
                                PurchaseId = payAttempt.PaymentAttemptInvoiceNo
                            };

                            PaymentGetStatusResponse PaymentGetStatusResponse = await GetPaymentStatus(GetPaymentStatusDto);

                            // update payment attempt record.
                            payAttempt.CollectionCenterFee = PaymentGetStatusResponse.CollectionCenterFees;
                            payAttempt.EDirhamFee = PaymentGetStatusResponse.EDirhamFees;
                            payAttempt.EServiceData = "{ \"response Time\": \"" + DateTime.Now.ToString() + "\",{" + JsonConvert.SerializeObject(PaymentGetStatusResponse) + "}}";
                            payAttempt.Pun = PaymentGetStatusResponse.PUN;
                            payAttempt.ConfirmationId = PaymentGetStatusResponse.ConfirmationID;
                            _EngineCoreDBContext.PaymentGateAttempt.Update(payAttempt);
                            await _EngineCoreDBContext.SaveChangesAsync();

                            bool updateOnlyOneScuessPay = true;
                            // success payment.
                            if (updateOnlyOneScuessPay && PaymentGetStatusResponse != null && PaymentGetStatusResponse.InvoiceID != null && PaymentGetStatusResponse.InvoiceID == GetPaymentStatusDto.PurchaseId && PaymentGetStatusResponse.Status == Constants.SuccessfulCodeStatus)
                            {
                                await UpdateSuccessPayment(PaymentGetStatusResponse, unPaid.Id);
                                updateOnlyOneScuessPay = false;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogInformation(" error exception from Cron auto pay in updating payments for Web Payment " + ex.Message);
                }
            }

            var unPaidEpos = await _EngineCoreDBContext.Payment.Where(x => x.Status != Constants.SuccessfulCodeStatus && x.Status != Constants.SuccessfulCodeStatusWALLET && x.Status != Constants.SuccessfulEPOSCodeStatus && x.PaymentType == Constants.EPOS_PAYMENT).ToListAsync();
            try
            {
                foreach (var unPaid in unPaidEpos)
                {
                   await QueryURNAsync((int)unPaid.ApplicationId, "en");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("error in updating payments for Epos " + ex.Message);
            }
        }

        public async Task<List<PaymentDto>> GetPaymentsBetweenTwoDates(DateTime startDate, DateTime endDate, string paymentType)
        {
            List<PaymentDto> result = new List<PaymentDto>();
            var payments = await _EngineCoreDBContext.Payment.Where(x => x.PaymentDate >= startDate && x.PaymentDate <= endDate).ToListAsync();

            if (paymentType != null && paymentType.Length > 0)
            {
                payments = payments.Where(x => x.PaymentType != null && x.PaymentType.Length > 0 && x.PaymentType.ToLower().Contains(paymentType.ToLower().Trim())).ToList();
            }
            foreach (var payment in payments)
            {

                // TODO  later join
                PaymentDto paymentDto = new PaymentDto
                {
                    Id = payment.Id,
                    ApplicationNo = (int)payment.ApplicationId,
                    PaymentStatus = payment.PaymentStatus == Constants.SuccessfullPaymentStatus,
                    InvoiceNo = payment.InvoiceNo,
                    Status = payment.Status,
                    Total = payment.TotalAmount,
                    ActualPaid = payment.ActualPaid,
                    DatePayment = payment.PaymentDate,
                    UserName = await _EngineCoreDBContext.User.Where(x => x.Id == payment.UserId).Select(x => x.FullName).FirstOrDefaultAsync(),
                    Message = payment.StatusMessage,
                    PaymentType = payment.PaymentType,
                    UpdatedName = await _EngineCoreDBContext.User.Where(x => x.Id == payment.LastUpdatedBy).Select(x => x.FullName).FirstOrDefaultAsync(),
                    CreatedName = await _EngineCoreDBContext.User.Where(x => x.Id == payment.CreatedBy).Select(x => x.FullName).FirstOrDefaultAsync(),

                };
                result.Add(paymentDto);
            }

            return result;
        }

        public async Task<PaymentDetailsGetWithAttempts> GetPaymentDetailsById(int paymentId)
        {
            PaymentDetailsGetWithAttempts result = new PaymentDetailsGetWithAttempts();
            var paymentDetails = await _EngineCoreDBContext.PaymentDetails.Where(x => x.PaymentId == paymentId).ToListAsync();

            if (paymentDetails != null && paymentDetails.Count > 0)
            {
                result.PaymentDetailsGet = new List<PaymentDetailsGetDto>();
            }

            foreach (var paymentDetail in paymentDetails)
            {
                PaymentDetailsGetDto paymentDto = new PaymentDetailsGetDto
                {
                    Id = paymentDetail.Id,
                    Quantity = paymentDetail.Quantity,
                    PaymentId = paymentDetail.PaymentId,
                    AmountWithFees = paymentDetail.AmountWithFees,
                    AmountWithoutFees = paymentDetail.AmountWithoutFees,
                    Price = paymentDetail.Price,
                    OwnerFees = paymentDetail.OwnerFees,
                    ServiceMainCode = paymentDetail.ServiceMainCode,
                    ServiceSubCode = paymentDetail.ServiceSubCode,
                };
                result.PaymentDetailsGet.Add(paymentDto);
            }


            var paymentAttemptDetails = await _EngineCoreDBContext.PaymentGateAttempt.Where(x => x.PaymentId == paymentId).ToListAsync();

            if (paymentAttemptDetails != null && paymentAttemptDetails.Count > 0)
            {
                result.PaymentGateAttempts = new List<PaymentGateAttemptsGet>();
            }


            foreach (var paymentAttemptDetail in paymentAttemptDetails)
            {
                PaymentGateAttemptsGet payGateAttemptsGet = new PaymentGateAttemptsGet
                {
                    Id = paymentAttemptDetail.Id,
                    CollectionCenterFee = paymentAttemptDetail.CollectionCenterFee,
                    ConfirmationId = paymentAttemptDetail.ConfirmationId,
                    EDirhamFee = paymentAttemptDetail.EDirhamFee,
                    EServiceData = paymentAttemptDetail.EServiceData,
                    PaidAttemptDate = paymentAttemptDetail.PaidAttemptDate,
                    PaymentAttemptInvoiceNo = paymentAttemptDetail.PaymentAttemptInvoiceNo,
                    Pun = paymentAttemptDetail.Pun,
                    SecureHash = paymentAttemptDetail.SecureHash
                };

                result.PaymentGateAttempts.Add(payGateAttemptsGet);
            }

            return result;

        }

        public async Task<ResQueryURNDto> QueryURNAsync(int appId, string lang)
        {
            ResQueryURNDto res = new ResQueryURNDto();
            var currentPay = await _EngineCoreDBContext.Payment.Include(x => x.PaymentGateAttempt).Where(x => x.ApplicationId == appId).FirstOrDefaultAsync();
            if (currentPay == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "nopaymentsforAPP"));
                throw _exception;
            }
            if (currentPay.Status == Constants.SuccessfulCodeStatus/* || currentPay.Status == Constants.SuccessfulEPOSCodeStatus*/)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "paidBefore"));
                throw _exception;
            }

            if (currentPay.PaymentGateAttempt.Count == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "PaymentNotDefinedAsEpos"));
                throw _exception;
            }

            foreach (var payAttempt in currentPay.PaymentGateAttempt)
            {
                try
                {
                    int number = Int32.Parse(payAttempt.ConfirmationId);
                    if (!await _EngineCoreDBContext.EmployeeSetting.AnyAsync( x=> x.EnotaryId == number))
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                var resQueryURN = await _IEPOSMachine.CheckUniqueReferenceNumberStatus(lang, Int32.Parse(payAttempt.ConfirmationId), payAttempt.PaymentAttemptInvoiceNo);

                if (resQueryURN != null && resQueryURN.paymentStatus != null)
                {
                    if (resQueryURN.paymentStatus.responseCode == "000" && resQueryURN.paymentStatus.paymentDone == true)
                    {
                        currentPay.Status = Constants.SuccessfulCodeStatus;
                        currentPay.ActualPaid = resQueryURN.totalAmount;
                        currentPay.PaymentStatus = 1.ToString();
                        currentPay.PaymentType = Constants.EPOS_PAYMENT;
                        currentPay.PaymentSource = resQueryURN.channel;

                        // notary id.
                        currentPay.CreatedBy = Int32.Parse(payAttempt.ConfirmationId);

                        try
                        {
                            currentPay.PaymentDate = Convert.ToDateTime(resQueryURN.responseDateTime);
                        }
                        catch
                        {
                            currentPay.PaymentDate = DateTime.Now;
                        }
                        _EngineCoreDBContext.Payment.Update(currentPay);
                        if (await _EngineCoreDBContext.SaveChangesAsync() > 0)
                        {
                            await UpdatePaymentDetialsAfterEposSuccessPay(resQueryURN.services, currentPay.Id, lang);
                            await RedirectAfterPay(Convert.ToInt32(currentPay.ApplicationId), Convert.ToInt32(currentPay.UserId));

                            res.PaymentDate = currentPay.PaymentDate;
                            res.responseCode = currentPay.Status;
                            res.responseDescription = currentPay.StatusMessage;
                            res.totalAmount = currentPay.TotalAmount;
                            res.uniqueReferenceNumber = payAttempt.PaymentAttemptInvoiceNo;
                            return res;

                        }
                        else
                        {
                            _exception.AttributeMessages.Add(Constants.getMessage(lang, "UpdatePaymentEposStatus"));
                            throw _exception;
                        }
                    }
                }
            }

            return res;
        }


        private string GetQueryURNSecureHash(ReqQueryURN ReqQueryURN)
        {
            string message = ReqQueryURN.entityCode + ReqQueryURN.uniqueReferenceNumber + ReqQueryURN.transactionReference;
            return HmacSha256Digest(message, "c6cbacff23f341a0");
        }

        public string HmacSha256Digest(string message, string secret)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            HMACSHA256 cryptographer = new HMACSHA256(keyBytes);
            byte[] bytes = cryptographer.ComputeHash(messageBytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private async Task UpdatePaymentDetialsAfterEposSuccessPay(ServiceQueryURN[] services, int payId, string lang)
        {
            var payDetails = await _EngineCoreDBContext.PaymentDetails.Where(x => x.PaymentId == payId).ToListAsync();
            if (payDetails == null || payDetails.Count == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "nopaymentsforAPP"));
                throw _exception;
            }

            foreach (var serviceQueryURN in services)
            {
                var transactionFee = await _EngineCoreDBContext.TransactionFee.Where(x => x.ServiceCodeEpos == serviceQueryURN.serviceCode).FirstOrDefaultAsync();
                if (transactionFee == null)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "UnknownError"));
                    throw _exception;
                }

                var paymentDetailsRowToUpapdate = payDetails.Where(x => x.PaymentId == payId && x.ServiceSubCode == transactionFee.SubClass && x.ServiceMainCode == transactionFee.PrimeClass).FirstOrDefault();
                if (paymentDetailsRowToUpapdate != null)
                {
                    paymentDetailsRowToUpapdate.Price = serviceQueryURN.unitPrice;
                    paymentDetailsRowToUpapdate.Quantity = serviceQueryURN.quantity;
                    paymentDetailsRowToUpapdate.AmountWithoutFees = serviceQueryURN.serviceAmount;
                    paymentDetailsRowToUpapdate.AmountWithFees = serviceQueryURN.totalAmount;
                }

            }
            _EngineCoreDBContext.UpdateRange(payDetails);
            if (await _EngineCoreDBContext.SaveChangesAsync() <= 0)
            {
                _exception.AttributeMessages.Add("Failed in updating payment details.");
                throw _exception;
            }
        }

    }
}
