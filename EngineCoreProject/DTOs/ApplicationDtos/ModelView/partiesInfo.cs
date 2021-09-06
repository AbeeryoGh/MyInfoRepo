
using System;
using System.Collections.Generic;


namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class partiesInfo
    {
        public short? ServiceResult { get; set; }
        public string ApplicationNo { set; get; }
        public int    TransactionId { set; get; }
        public string    Title { set; get; }
        public string TransactionNo { set; get; }
        public string TemplateName { set; get; }
        public string DocumentType { set; get; }
        public string transactionText { set; get; }
        public string FileName { get; set; }

        public DateTime MeetingDate { get; set; }

        // public int? CopiesNum { get; set; }
        public string decisionText  { set; get; }
        public List<ApplicationPartyFinalDocumentGrouped> parties { set; get; }
        public List<RelatedContentView> relatedContents { set; get; }
        public List<string> relatedTransactions { set; get; }
        public NotaryInfo notaryInfo { set; get; }


    }
}
