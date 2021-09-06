using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.NotificationDtos
{
    public class NotificationTemplateDetailGetDto
    {     
        public int NotificationChannelId { get; set; }

        public Dictionary<string, string> ChannelShortCutLangValue { get; set; }

        public Dictionary<string, string> TitleShortCutLangValue { get; set; }

        public Dictionary<string, string> BodyShortCutLangValue { get; set; }

        public bool ChangeAble { get; set; }
    }
}
