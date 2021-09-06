using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class EposMachine
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string EntityCode { get; set; }
        public string TerminalId { get; set; }
        public string SessionToken { get; set; }
        public string Tokens { get; set; }
        public DateTime? ExpiredSessionToken { get; set; }
        public string Channel { get; set; }
        public string LocationCode { get; set; }
        public string TransactionReference { get; set; }
        public string SourceReference { get; set; }
        public int? EnotaryId { get; set; }
    }
}
