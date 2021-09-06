using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.NotificationDtos
{
    public class NotificationTemplateGetDto
    {        
        public int NotificationTemplateId { get; set; }
        public Dictionary<string, string> NotificationTemplateShortCutLangValue { get; set; }

        public NotificationTemplateGetDto()
        {
        }
    } 
}
