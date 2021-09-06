using EngineCoreProject.DTOs.SysLookUpDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.EPOSMachineDto
{
    public class ResQueryPrice
    {
        public string uniqueReferenceNumber { get; set; }
        public int totalTaxAmount { get; set; }
        public float totalAmount { get; set; }
        public string sourceReference { get; set; }
        public string hashReference { get; set; }
        public Service[] services { get; set; }
        public Fee[] fees { get; set; }
        public string entityCode { get; set; }
        public string channel { get; set; }
        public string instrument { get; set; }
        public string locationCode { get; set; }
        public string transactionReference { get; set; }
        public float totalFees { get; set; }
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
        public string responseDateTime { get; set; }
    }
    public class Service
    {
        public string serviceCode { get; set; }
        public string englishName { get; set; }
        public string arabicName { get; set; }
        public string englishDescription { get; set; }
        public string arabicDescription { get; set; }
        public int unitPrice { get; set; }
        public string priceType { get; set; }
        public int transactionAmount { get; set; }
        public int numberOfUnits { get; set; }
        public int quantity { get; set; }
        public bool taxApplicable { get; set; }
        public int serviceAmount { get; set; }
        public int serviceTaxAmount { get; set; }
        public int totalTaxAmount { get; set; }
        public int totalAmount { get; set; }
        public int serviceRefundTaxAmount { get; set; }
        public int serviceRefundAmount { get; set; }
        public Taxdetail[] taxDetails { get; set; }
    }
    public class ServiceReq
    {
        public string serviceCode { get; set; }

        public string price { get; set; }

        public string quantity { get; set; }

    }
    public class Taxdetail
    {
        public int taxableAmount { get; set; }
        public int taxAmount { get; set; }
        public string taxRegisterationNumber { get; set; }
        public string taxCode { get; set; }
    }
    public class Fee
    {
        public string feeCode { get; set; }
        public string englishName { get; set; }
        public string arabicName { get; set; }
        public string englishDescription { get; set; }
        public string arabicDescription { get; set; }
        public string type { get; set; }
        public string subType { get; set; }
        public int unitPrice { get; set; }
        public string priceType { get; set; }
        public int transactionAmount { get; set; }
        public int numberOfUnits { get; set; }
        public int quantity { get; set; }
        public bool taxApplicable { get; set; }
        public float feeAmount { get; set; }
        public int feeTaxAmount { get; set; }
        public int feeRefundTaxAmount { get; set; }
        public int feeRefundAmount { get; set; }
        public bool isDebit { get; set; }
    }
    public class ApiEposUrl
    {
        public string loginURL { get; set; }
        public string TokenInquiryURL { get; set; }
        public string QueryServicePriceURL { get; set; }
        public string QueryUrnURL { get; set; }

    }
    public class EPOSMachineSettingDto
    {
        [Required]
        public string locationCode { get; set; }
        [Required]
        public string transactionReference { get; set; }
        [Required]
        public string sourceReference { get; set; }
        [Required]
        public string entityCode { get; set; }
        [Required]
        public string terminalID { get; set; }
        [Required]
        public string userID { get; set; }
        [Required]
        public string channel { get; set; }
        public int EnotaryId { get; set; }
    }

    public class ReqQueryPriceDto
    {
        public string Instrument { get; set; }
        public int AppId { get; set; }
        public int UserId { get; set; }
    }
   
    public class ReqLogin
    {
        public string entityCode { get; set; }
        public string terminalID { get; set; }
        public string userID { get; set; }
        public string secureHash { get; set; }
    }
    public class ResLogin
    {
        public string entityCode { get; set; }
        public string sessionToken { get; set; }
        public DateTime? tokenExpiryDate { get; set; }
        public string tokenStatus { get; set; }
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
        public string responseDateTime { get; set; }
    }
    public class ResTokenInquiry
    {
        public string entityCode { get; set; }
        public string terminalID { get; set; }
        public string userID { get; set; }
        public string expiryDate { get; set; }
        public string tokenStatus { get; set; }
        public string sessionToken { get; set; }
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
        public string responseDateTime { get; set; }
    }
    public class ResLoginDto
    {
        public string sessionToken { get; set; }
        public DateTime? tokenExpiryDate { get; set; }

        public string responseCode { get; set; }

    }
    public class ReqQueryPrice
    {
        public string sessionToken { get; set; }
        public string userID { get; set; }
        public string entityCode { get; set; }
        public string channel { get; set; }
        public string instrument { get; set; }
        public string locationCode { get; set; }
        public string transactionReference { get; set; }
        public string sourceReference { get; set; }
        public string secureHash { get; set; }
        public List<ServiceReq> services { get; set; }
    }
    public class ServiceQueryPrice
    {
        public string serviceCode { get; set; }
        public string englishName { get; set; }
        public string arabicName { get; set; }
        public string englishDescription { get; set; }
        public string arabicDescription { get; set; }
        public int unitPrice { get; set; }
        public string priceType { get; set; }
        public int transactionAmount { get; set; }
        public int numberOfUnits { get; set; }
        public int quantity { get; set; }
        public bool taxApplicable { get; set; }
        public int serviceAmount { get; set; }
        public int serviceTaxAmount { get; set; }
        public int totalTaxAmount { get; set; }
        public int totalAmount { get; set; }
        public int serviceRefundTaxAmount { get; set; }
        public int serviceRefundAmount { get; set; }
        public Taxdetail[] taxDetails { get; set; }
    }
    public class ResQueryPriceDto
    {
        public float totalAmount { get; set; }
        public string message { get; set; }
        public string responseCode { get; set; }
        public string urnReferenceNumber { get; set; }
    }

    public class ReqQueryURN
    {
        public string entityCode { get; set; }
        public string uniqueReferenceNumber { get; set; }
        public string sourceReference { get; set; }
        public string transactionReference { get; set; }
        public string userID { get; set; }
        public string sessionToken { get; set; }
        public string optionalParameter1 { get; set; }
        public string optionalParameter2 { get; set; }
        public string secureHash { get; set; }
    }
    public class ResQueryURN
    {
        public string uniqueReferenceNumber { get; set; }
        public string entityCode { get; set; }
        public string channel { get; set; }
        public string instrument { get; set; }
        public string transactionType { get; set; }
        public string locationCode { get; set; }
        public string sourceReference { get; set; }
        public int totalTaxAmount { get; set; }
        public int totalAmount { get; set; }
        public ServiceQueryURN[] services { get; set; }
        public FeeQueryURN[] fees { get; set; }
        public string transactionReference { get; set; }
        public string optionalParameter1 { get; set; }
        public string optionalParameter2 { get; set; }
        public PaymentstatusQueryURN paymentStatus { get; set; }
        public int totalFees { get; set; }
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
        public string responseDateTime { get; set; }
    }
    public class ResQueryURNDto
    {
        public string uniqueReferenceNumber { get; set; }

        public double? totalAmount { get; set; }

        public string responseCode { get; set; }
        public string responseDescription { get; set; }

        public string URN { get; set; }
        public DateTime? PaymentDate { get; set; }

    }
    public class PaymentstatusQueryURN
    {
        public bool paymentDone { get; set; }
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
        public PaymentdetailsQueryURN paymentDetails { get; set; }
    }
    public class PaymentdetailsQueryURN
    {
        public string transactionRRN { get; set; }
        public string approvalNumber { get; set; }
        public string urn { get; set; }
        public string cardNumber { get; set; }
        public string transactionDateTime { get; set; }
    }
    public class ServiceQueryURN
    {
        public string serviceCode { get; set; }
        public string englishName { get; set; }
        public string arabicName { get; set; }
        public string englishDescription { get; set; }
        public string arabicDescription { get; set; }
        public int unitPrice { get; set; }
        public string priceType { get; set; }
        public int transactionAmount { get; set; }
        public int numberOfUnits { get; set; }
        public int quantity { get; set; }
        public bool taxApplicable { get; set; }
        public int serviceAmount { get; set; }
        public int serviceTaxAmount { get; set; }
        public int totalTaxAmount { get; set; }
        public int totalAmount { get; set; }
        public int serviceRefundTaxAmount { get; set; }
        public int serviceRefundAmount { get; set; }
        public TaxdetailQueryURN[] taxDetails { get; set; }
    }
    public class TaxdetailQueryURN
    {
        public int taxableAmount { get; set; }
        public int taxAmount { get; set; }
        public string taxRegisterationNumber { get; set; }
        public string taxCode { get; set; }
    }
    public class FeeQueryURN
    {
        public string feeCode { get; set; }
        public string englishName { get; set; }
        public string arabicName { get; set; }
        public string englishDescription { get; set; }
        public string arabicDescription { get; set; }
        public string type { get; set; }
        public string subType { get; set; }
        public int unitPrice { get; set; }
        public string priceType { get; set; }
        public int transactionAmount { get; set; }
        public int numberOfUnits { get; set; }
        public int quantity { get; set; }
        public bool taxApplicable { get; set; }
        public int feeAmount { get; set; }
        public int feeTaxAmount { get; set; }
        public int feeRefundTaxAmount { get; set; }
        public int feeRefundAmount { get; set; }
        public bool isDebit { get; set; }
    }

    public class EPOSCardsEmployeeToken
    {
        public List<TranslationValueDtoGet> Cards { get; set; }
        public string EposToken { get; set; }
        public DateTime ExpiredTokenTime { get; set; }
    }
}
