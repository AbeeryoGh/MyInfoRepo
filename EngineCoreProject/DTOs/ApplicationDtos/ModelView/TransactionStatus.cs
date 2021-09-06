using System;

namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class TransactionStatus
    {
        public int    StatusId { get; set; }
        public string Status   { get; set; }
        public bool   IsValid  { get; set; }

        public bool MultiVersion { get; set; } = false;

    }
}
