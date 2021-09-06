using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class AramexRequests
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string OwnerName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int? StateId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Note { get; set; }

        public virtual Application Application { get; set; }
    }
}
