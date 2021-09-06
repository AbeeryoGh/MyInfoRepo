using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class DraftApplicationDto
    {
        public int? ServiceId { get; set; }
        public string ApplicationNo { get; set; }
        public DateTime? TransactionStartDate { get; set; }
        public DateTime? TransactionEndDate { get; set; }
        public int? TemplateId { get; set; }
        public int? CurrentStageId { get; set; }

    }
}
