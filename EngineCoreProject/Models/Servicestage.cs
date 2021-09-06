using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Servicestage
    {
        public int Id { get; set; }
        public string Shortcut { get; set; }
        public int? StageId { get; set; }
        public string Expr1 { get; set; }
    }
}
