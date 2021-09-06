using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class WorkingHours
    {
        public int Id { get; set; }
        public string WorkingTimeNameShortcut { get; set; }
        public int DayOfWeek { get; set; }
        public int StartFrom { get; set; }
        public int FinishAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
