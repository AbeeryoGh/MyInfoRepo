using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class TransactionDto
    {
        public int? ApplicationId { get; set; }
        public string Content { get; set; }
        public string TransactionNo { get; set; }
        public string FileName { get; set; }
        public string DocumentUrl { get; set; }
        public string DecisionText { get; set; }
        public DateTime? TransactionStartDate { get; set; }
        public DateTime? TransactionEndDate { get; set; }
        public bool? UnlimitedValidity { get; set; }
        public int? TransactionStatus { get; set; }
        public DateTime? TransactionCreatedDate { get; set; }
        public string QrCode{ get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string Title { get; set; }

        public double? ContractValue { get; set; }



        public static TransactionDto GetDto(AppTransaction appTransaction)
        {
            TransactionDto toDto = new TransactionDto()
            {
                ApplicationId = appTransaction.ApplicationId,
                Content = appTransaction.Content,
                DecisionText = appTransaction.DecisionText,
                DocumentUrl = appTransaction.DocumentUrl,
                FileName = appTransaction.FileName,
                TransactionNo = appTransaction.TransactionNo,
                TransactionStartDate = appTransaction.TransactionStartDate,
                TransactionEndDate = appTransaction.TransactionEndDate,
                TransactionStatus = appTransaction.TransactionStatus,
                UnlimitedValidity = appTransaction.UnlimitedValidity,
                TransactionCreatedDate = appTransaction.CreatedDate,
                QrCode= appTransaction.Qrcode,
                LastUpdatedDate= appTransaction.LastUpdatedDate,
                LastUpdatedBy= appTransaction.LastUpdatedBy,
                Title= appTransaction.Title,
                ContractValue= appTransaction.ContractValue
  
            };
            return toDto;
        }
    }
}
