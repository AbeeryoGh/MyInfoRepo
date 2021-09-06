using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ApplicationPartyExtraAttachmentDto
    {
        public int?   ApplicationPartyId { get; set; }
          
        public int?   AttachmentId { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentUrl { get; set; }
        public string Number { get; set; }
        public int?  CountryOfIssue { get; set; }
        public DateTime? ExpirationDate { get; set; }
        

    }
}
