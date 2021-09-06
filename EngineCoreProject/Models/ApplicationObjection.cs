using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class ApplicationObjection
    {
        public ApplicationObjection()
        {
            ApplicationObjectionAttachment = new HashSet<ApplicationObjectionAttachment>();
        }

        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public int? Nationality { get; set; }
        public string Gender { get; set; }
        public string EmiratesId { get; set; }
        public string Address { get; set; }
        public int? EmaraId { get; set; }
        public string City { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<ApplicationObjectionAttachment> ApplicationObjectionAttachment { get; set; }
    }
}
