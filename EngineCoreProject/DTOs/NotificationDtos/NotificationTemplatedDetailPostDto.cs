using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.NotificationDtos
{
    public class NotificationTemplatedDetailPostDto
    {     
        [Required]
        public int NotificationChannelId { get; set; }

        [Required]
        public Dictionary<string, string> TitleShortCutLangValue { get; set; }

        [Required]
        public Dictionary<string, string> BodyShortCutLangValue { get; set; }

        [Required]
        public bool ChangeAble { get; set; }
    }
}
