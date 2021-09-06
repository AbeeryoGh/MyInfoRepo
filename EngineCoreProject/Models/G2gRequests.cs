using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class G2gRequests
    {
        public int Id { get; set; }
        public DateTime ReqDate { get; set; }
        public string ApiName { get; set; }
        public string Reqest { get; set; }
        public string Response { get; set; }
    }
}
