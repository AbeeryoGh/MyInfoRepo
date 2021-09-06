using EngineCoreProject.DTOs.PDFDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.PDFGenerator
{
    public class TemplateInfoDto
    {

        public string RecQRUrl { set; get; }
        public string lang { set; get; }
        public string FileName { set; get; }
        public int? appID { get; set; }
        public string FileNameWithoutSign { set; get; }
        public string path { set; get; }
        public string Title { set; get; }
        public string Content { set; get; }
        // public string note { set; get; }
        // public DateTime? date { set; get; }
        public string TransactionNo { set; get; }
        public List<Party> parties { set; get; }
        // public string PageNum { set; get; }
        public string PaymentFee { set; get; }
        public string ReceiptNumber { set; get; }
        public NotaryInfo notaryInfo { set; get; }
        public string PaymentDate { set; get; }

        public int? TransactionId { get; set; }
        public DateTime MeetingDate { get; set; }
    }


    //public class risalatTablighDto
    //{

    //    public string RecQRUrl { set; get; }
    //    public string lang { set; get; }
    //    public string FileName { set; get; }
    //    public string path { set; get; }
    //    public string Title { set; get; }
    //    public string Content { set; get; }
    //    // public string note { set; get; }
    //    // public DateTime? date { set; get; }
    //    public string TransactionNo { set; get; }
    //    public List<Party> parties { set; get; }
    //    // public string PageNum { set; get; }
    //    //public string PaymentFee { set; get; }
    //    //public string ReceiptNumber { set; get; }
    //    public NotaryInfo notaryInfo { set; get; }
    //    public int? TransactionId { get; set; }
    //}


    //public class DocumentTempleteDto
    //{

    //    public string RecQRUrl { set; get; }
    //    public string lang { set; get; }
    //    public string FileName { set; get; }
    //    public string path { set; get; }
    //    public string Title { set; get; }
    //    public string Content { set; get; }
    //    // public string note { set; get; }
    //    // public DateTime? date { set; get; }
    //    public string TransactionNo { set; get; }
    //    public List<Party> parties { set; get; }
    //    // public string PageNum { set; get; }
    //    public string transactionText { set; get; }
    //    public string TemplateName { set; get; }
    //    public NotaryInfo notaryInfo { set; get; }
    //    public int? TransactionId { get; set; }
    //}

    //public class EkhtarAdliDto
    //{

    //    public string RecQRUrl { set; get; }
    //    public string lang { set; get; }
    //    public string FileName { set; get; }
    //    public string path { set; get; }
    //    public string Title { set; get; }
    //    public string Content { set; get; }
    //    public string note { set; get; }
    //    public DateTime? date { set; get; }
    //    public string TransactionNo { set; get; }
    //    public List<Party> parties { set; get; }
    //    public string PageNum { set; get; }
    //    public string PaymentFee { set; get; }
    //    public int? TransactionId { get; set; }
    //}

}
