using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class ApplicationPartyFinalDocumentGrouped
    {
        public string TemplateName { get; set; }
        public string transactionText { get; set; }
        public int    Id { get; set; }
        public string FullName  { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Nationality { get; set; }
        public string PartyType { get; set; }
        public string AttachmentName { get; set; }
        public string CountryOfIssue { get; set; }
         public string AttachmentNo { get; set; }
        public string AttachmentUrl { get; set; }
        public string SignUrl { get; set; }
        public bool? SignRequired { get; set; }
        public   List<Documentation> AttachmentsList   { get; set; }
       

    }
}
