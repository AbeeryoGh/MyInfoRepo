using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.NotificationDtos
{
    public class NotificationTemplateWithDetailsGetDto
    {        
        public int NotificationTemplateId { get; set; }
        public Dictionary<string, string> NotificationTemplateShortCutLangValue { get; set; }
        public List<NotificationTemplateDetailGetDto> NotificationTemplateDetails { get; set; }

        public NotificationTemplateWithDetailsGetDto()
        {
            NotificationTemplateShortCutLangValue = new Dictionary<string, string>();
            NotificationTemplateDetails = new List<NotificationTemplateDetailGetDto>();
        }
    } 
}
