using System;

namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class UserApplication
    {
        public int ApplicationId { get; set; }
        public string ApplicationNo { get; set; }
        public int TransactionId { get; set; }
        public string TransactionNo { get; set; }

        public string Template { get; set; }
        public string Owner { get; set; }
        public DateTime?  ApplicationDate { get; set; }
        public DateTime?  StartDate { get; set; }
        public DateTime?  ExpireDate { get; set; }
        public bool Enabled { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        public string DocumentUrl { get; set; }
        public bool Existing { get; set; }

    }
}
