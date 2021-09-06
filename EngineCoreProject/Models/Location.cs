using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class Location
    {
        public Location()
        {
            ApplicationTrack = new HashSet<ApplicationTrack>();
            EmployeeSetting = new HashSet<EmployeeSetting>();
            InverseParentLocation = new HashSet<Location>();
        }

        public int Id { get; set; }
        public string NameShortcut { get; set; }
        public int? ParentLocationId { get; set; }

        public virtual Location ParentLocation { get; set; }
        public virtual ICollection<ApplicationTrack> ApplicationTrack { get; set; }
        public virtual ICollection<EmployeeSetting> EmployeeSetting { get; set; }
        public virtual ICollection<Location> InverseParentLocation { get; set; }
    }
}
