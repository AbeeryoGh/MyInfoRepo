using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.PDFDto
{
    public class DocToPdfDto
    {
        public string ShortCut { get; set; }
        public short? ServiceResult { get; set; }
        public string TemplateName { get; set; }
        public List<Party> parties { get; set; }
        public string transactionText { get; set; }
        public string Title { set; get; }
        // public int? CopiesNum { get; set; }
        public string FileName { get; set; }
        public string decisionText { get; set; }
        public string ReceiptNumber { get; set; }
        public double? PaymentFee { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string NumPages { get; set; }
        public string TransactionNo { get; set; }
        public string ApplicationNo { get; set; }
        public string DocumentType { get; set; }
        public int? appID { get; set; }
        public NotaryInfo notaryInfo { set; get; }

        public DateTime MeetingDate { get; set; }
        public List<string> relatedTransactions { set; get; }

        //////////////////////
        public List<RelatedContentView> relatedContents { set; get; }
    }

    public class Party
    {
        public string fullName { get; set; }
        public string partyType { get; set; }
        public string nationality { get; set; }
        public string documentType { get; set; }
        public string documentNumber { get; set; }
        public string countryOfIssue { get; set; }
        public string address { get; set; }
        public string phoneNumber { get; set; }
        public string signtureUrl { get; set; }

        public bool? SignRequired { get; set; }

    }
}