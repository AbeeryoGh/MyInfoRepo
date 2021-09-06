using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;



namespace EngineCoreProject.Models
{


    [DataContract]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://enotary.moj.gov.ae/enotary/notaryFee")]
    public class RetrieveNotaryFeesMOJRequest
    {
       [DataMember]
       public string EODBTrackingNumber { get; set; }

       [DataMember]
       public int numberOfInvestors { get; set; }  // lowercase required for Bashr.

        [DataMember]
       public decimal capitalAmount { get; set; }  // lowercase required for Bashr.
    }






    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://enotary.moj.gov.ae/enotary/notaryFee")]
    [MessageContract]
    public class MemberSoapHeader : SoapHeader
    {
        [MessageHeader]
        public string transactionId { get; set; }

        [MessageHeader]
        public string serviceProviderEntity { get; set; }

    }








    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://enotary.moj.gov.ae/enotary/MOADetails")]
    public class PaymentInformationObj
    {
        [DataMember]
        public string paymentReferenceNumber { get; set; }

        [DataMember]
        public DateTime paymentDate { get; set; }

        [DataMember]
        public double totalFees { get; set; }

        [DataMember]
        public string voucherReferenceNo { get; set; }

        [DataMember]
        public string paymentMode { get; set; }

    }



    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://enotary.moj.gov.ae/enotary/MOADetails")]
    public partial  class InvestorInformation
    {
        [DataMember]
        public string fullNameAr { get; set; }

        [DataMember]
        public string fullNameEn { get; set; }

        [DataMember]
        public string passportNumber { get; set; }

        [DataMember]
        public string emiratesID { get; set; }

        [DataMember]
        public string nationalityCountryCode { get; set; }

        [DataMember]
        public string nationalityCountryDescAr { get; set; }

        [DataMember]
        public string nationalityCountryDescEn { get; set; }

        [DataMember]
        public string cityCode { get; set; }

        [DataMember]
        public string cityDescAr { get; set; }

        [DataMember]
        public string cityDescEn { get; set; }

        [DataMember]
        public string emirateCode { get; set; }

        [DataMember]
        public string emirateDescAr { get; set; }

        [DataMember]
        public string emirateDescEn { get; set; }

        [DataMember]
        public string buildingName { get; set; }

        [DataMember]
        public string flatNumber { get; set; }

        [DataMember]
        public string streetName { get; set; }

        [DataMember]
        public string areaDescAr { get; set; }

        [DataMember]
        public string areaDescEn { get; set; }

        [DataMember]
        public string dateOfBirth { get; set; }

        [DataMember]
        public string capitalSharePercentage { get; set; }

        [DataMember]
        public string profitSharePercentage { get; set; }

        [DataMember]
        public string isManager { get; set; }

        [DataMember]
        public string isAuthorizedSignatory { get; set; }

        [DataMember]
        public string mobileNumber { get; set; }

        [DataMember]
        public string emailID { get; set; }
    }




    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://enotary.moj.gov.ae/enotary/MOADetails")]
    public partial class FeesBreakDown
    {
        [DataMember]
        public string serviceCode { get; set; }

        [DataMember]
        public double serviceFees { get; set; }

        [DataMember]
        public string serviceDescAr { get; set; }

        [DataMember]
        public string serviceDescEn { get; set; }

    }




    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://enotary.moj.gov.ae/enotary/MOADetails")]
    public class CompanyInformationObj
    {
        [DataMember]
        public string tradeNameAr { get; set; }
        [DataMember]
        public string tradeNameEn { get; set; }
        [DataMember]
        public string companyDuration { get; set; } // optional.
        [DataMember]
        public string capitalAmount { get; set; } // Optional
        [DataMember]
        public string shareValue { get; set; } // Optional
        [DataMember]
        public string noOfShares { get; set; } // Optional
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://enotary.moj.gov.ae/enotary/MOADetails")]
    public class SendAppMOADetails_MOJ
    {
        public string EODBTrackingNumber { get; set; }

        [DataMember]
        public CompanyInformationObj CompanyInformation { get; set; }

        [DataMember]
        public byte[] MoaSignedCopy { get; set; }

        [DataMember]
        private InvestorInformation[] investorInformation;

        [DataMember]

        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "https://enotary.moj.gov.ae/enotary/MOADetails")]
        public InvestorInformation[] Records
        {
            get
            {
                return this.investorInformation;
            }
            set
            {
                this.investorInformation = value;
            }
        }

        [DataMember]
        public PaymentInformationObj PaymentInformation { get; set; }

        [DataMember]
        private FeesBreakDown[] FeesBreakDown;

        [DataMember]

        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "https://enotary.moj.gov.ae/enotary/MOADetails")]
        public FeesBreakDown[] FeesRecords
        {
            get
            {
                return this.FeesBreakDown;
            }
            set
            {
                this.FeesBreakDown = value;
            }
        }
    }



    public class sendMOADetailsMOJResponse
    {
        public string EODBTrackingNumber { get; set; }
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
        public string NotaryReferenceNo { get; set; }
    }


    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://enotary.moj.gov.ae/enotary/MOADetails")]
    [MessageContract]
    public class MemberSoapHeaderDetail : SoapHeader
    {
        [MessageHeader]
        public string transactionId { get; set; }

        [MessageHeader]
        public string serviceProviderEntity { get; set; }

    }

    public class ValidationSoapHeader : SoapHeader
    {
        private string _devToken;
        public ValidationSoapHeader()

        {
        }

        public ValidationSoapHeader(string devToken)
        {
            this._devToken = devToken;
        }

        public string DevToken
        {
            get { return this._devToken; }
            set { this._devToken = value; }
        }
    }

}
