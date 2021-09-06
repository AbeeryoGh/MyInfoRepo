using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class BlockChainPoa
    {
        public int Id { get; set; }
        public int AppId { get; set; }
        public string Vcid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsUgCancelled { get; set; }
        public bool? IsSysCancelled { get; set; }
        public bool? IsSentUg { get; set; }
    }
}
