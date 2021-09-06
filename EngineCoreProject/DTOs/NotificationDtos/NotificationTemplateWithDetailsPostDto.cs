using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.NotificationDtos
{
    public class NotificationTemplateWithDetailsPostDto
    {        
        [Required]
        public Dictionary<string, string> NotificationTemplateShortCutLangValue { get; set; }

        public List<NotificationTemplatedDetailPostDto> NotificationTemplatedDetails { get; set; }



    }
}
