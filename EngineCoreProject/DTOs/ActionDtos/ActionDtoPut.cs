using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ActionDtos
{
    public class ActionDtoPut
    {
        public int Id { get; set; }
        // public DateTime? CreatedDate { get; set; }
        // public DateTime? UpdatedDate { get; set; }
        public bool? ActiveNotification { get; set; }
        public int NotificationTypeId { get; set; }
        public int actionNameId { get; set; }
        public int NotificationColorId { get; set; }
    }
}
