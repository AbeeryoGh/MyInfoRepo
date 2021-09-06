
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;

namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class TransactionInfo :TransactionStatus
    {
        public string TransactionNo { get; set; }
        public string DocumentUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<string> Messages { get; set; }
        public List<TransactionOldVersionDto> OldVersion { get; set; }
        public List<RelatedContentView> RelatedDataUrlsDocumentUrl { get; set; }
    }
}
