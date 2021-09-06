using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class UserLogger
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LoggingDate { get; set; }
        public bool? StartWorkForEmployee { get; set; }

        public virtual User User { get; set; }
    }
}
