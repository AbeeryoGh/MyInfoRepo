using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.NotificationDtos
{
    public class NotificationTemplateDetailsForOneAction
    {        
        public int ActionId { get; set; }
        public List<NotificationTemplateWithDetailsGetDto> NotificationTemplateDetails { get; set; }

    } 
}
