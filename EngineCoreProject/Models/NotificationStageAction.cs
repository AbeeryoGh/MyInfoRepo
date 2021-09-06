using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class NotificationStageAction
    {
        public int? ActionId { get; set; }
        public int? NotificationDetialsId { get; set; }
        public string NotificationName { get; set; }
        public string Lang { get; set; }
        public string Value { get; set; }
        public string ActionShortcut { get; set; }
        public int? Expr1 { get; set; }
        public int? StageId { get; set; }
        public string ActionTranslate { get; set; }
    }
}
