using System;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class TransactionOldVersionDto
    {
        public int TransactionId { get; set; }
       // public int OldTransactionId { get; set; }
        public string TransactionNo { get; set; }
        public string DocumentUrl { get; set; }
        public DateTime TransactionCreatedDate { get; set; }
        public string Note { get; set; }
    }
}
