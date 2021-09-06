using EngineCoreProject.DTOs.EPOSMachineDto;
using EngineCoreProject.DTOs.FeesDto;
using EngineCoreProject.DTOs.Payment;
using EngineCoreProject.Models;

using PaymentServicePro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IPaymentRepository
{
    public interface IPaymentRepository
    {    
        /// <summary>
        /// Generate URL for first unpaid payment related to one application. 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="lang"></param>
        /// <param name="description"></param>
        /// <param name="UserId"></param>
        /// <param name="ActionId"></param>
        /// <returns>The URL to redirect to the auto payment gate, throw Exception if no application or no payment for this application or no unpaid payment for the application</returns>
        Task<string> GenerateURL(int applicationId, string lang, string description, int UserId,int ActionId);
        Task<PaymentGetStatusResponse> GetPaymentStatus(GetPaymentStatusDto GetPaymentStatusDto);  
        Task<List<PaymentDto>> GetMyPayment(int UserId, string lang);
        Task<PaymentDto> GetPaymentInfo(int PaymentId, string lang);
        string RedirectMethod(string Host, Dictionary<string, string> DictionaryURL);
        string RedirectMethodWithPathPara(string Host,List<string> DictionaryURL);   
        string GenerateInvoiceNo();      
        string GetValueOfResponseKey(List<PaymentResponseDtoList> myDeserializedClass, string key);
        string getResponseHash(List<PaymentResponseDtoList> myDeserializedClass);
       // Task<PaymentDetialsDto> GetPaymentInfoByAppId(int applicationId, string lang);

        Task<PaymentDetialsForAPPDto> GetPaymentsInfoByAppId(int applicationId, string lang);
        /// <summary>
        /// Add a new payment with details to pay later, replace if exist before,
        ///
        /// </summary>
        /// <param name="PaymentDetailsDto"></param>
        /// <param name="addNewIfPaidBefore">if true add the new payment even it paid before, else throw exception if paid before.</param>
        /// <param name="lang"></param>
        /// <returns></returns>
        Task AddApplicationFeesAsync(PaymentDetailsDto PaymentDetailsDto, string lang);
        Task<List<GetFees>> GetApplicationFeesByIdAsync(int Id, string lang);
        Task<List<GetFees>> GetApplicationFeesAsync();
        Task<bool> DeleteApplicationFeesAsync(int Id);
        Task<string> ManualPay(ManualPaymentDetialsDto manualPaymentDetials, string lang);
        Task<string> AutoPay(List<PaymentResponseDtoList> paymentResponseDtoList, string lang);
        Task<List<PaymentDetails>> GetApplicationFeesByAppIdAsync(int AppId);

        ////////crons
        public Task UpdatePaymentTableCrons();
    
       // public Task UpdatePaymentTableAfterAskStatus(PaymentGetStatusResponse PaymentGetStatusResponse, int PaymentId);

        public Task<List<PaymentDto>> GetPaymentsBetweenTwoDates(DateTime startDate, DateTime endDate, string paymentType);
        public Task<PaymentDetailsGetWithAttempts> GetPaymentDetailsById(int paymentId);

        public Task<ResQueryURNDto> QueryURNAsync(int appId, string lang);
    }
}
