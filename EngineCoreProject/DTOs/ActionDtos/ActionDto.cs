using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ActionDtos
{
    public class ActionDto
    {

        public int Id { get; set; }
       // public DateTime? CreatedDate { get; set; }
       // public DateTime? UpdatedDate { get; set; }
        public bool? ActiveNotification { get; set; }
        public string NotificationType { get; set; }
        public string actionName { get; set; }
        public string NotificationColor { get; set; }

    }
}
