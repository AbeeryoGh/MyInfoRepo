using EngineCoreProject.DTOs.TransactionFeeDto;
using EngineCoreProject.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Payment
{
    public class PaymentDto
    {
        public int? Id { get; set; }
        public int ApplicationNo { set; get; }
        public bool PaymentStatus { get; set; }
        public string ServiceName { set; get; }
        public string InvoiceNo { set; get; }
        public DateTime? DatePayment { set; get; }
        public double? Total { get; set; }
        public double? ActualPaid { get; set; }
        public string Status { set; get; }
        public string Message { set; get; }
        public string UserName { set; get; }
        public string PaymentType { set; get; }

        public string UpdatedName { set; get; }
        public string CreatedName { set; get; }

    }


    public class PaymentDetialsDto
    {
        public string PaymentDate { set; get; }
        public string ServiceName { set; get; }
        public DateTime? ApplicationDate { set; get; }
        public double? Total { get; set; }
        public string PaymentStatus { set; get; }
        public bool Status { set; get; }
        public string InvoiceNo { set; get; }

    }

    public class PaymentsDates
    {
        public string InvoicesNumbers { set; get; }
        public string InvoicesDates { set; get; }
    }

    public class PaymentDetialsForAPPDto
    {
        public List<PaymentsDates> PaymentsDates { get; set; }
        public double Total { get; set; }
        public Constants.PAYMENT_STATUS_ENUM PaymentStatus { set; get; }   // four kinds, 1 paid, 0 unpaid, 2 partial paid, 3 no payment

        public string PaymentStatusName { set; get; }
        public List<TransactionFeeOutput> ApplicationFees { set; get; }

        public PaymentDetialsForAPPDto()
        {
            ApplicationFees = new List<TransactionFeeOutput>();
            PaymentsDates = new List<PaymentsDates>();
        }

    }


 
}
