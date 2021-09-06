using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ApplicationPartyWithAttachmentsDto
    {
        public int?  PartyId { get; set; }
        public int?  TransactionId { get; set; }
        public bool? IsOwner { get; set; }
        public int?  PartyTypeValueId { get; set; }

        public string FullName { get; set; }
        public string Mobile   { get; set; }
        public string Email    { get; set; }
        public int Nationality   { get; set; }
        public DateTime? BirthDate  { get; set; }
        public int MaritalStatus { get; set; }
        public int Gender { get; set; }
        public string EmiratesIdNo { get; set; }
        public DateTime? IdIssuanceDate   { get; set; }
        public DateTime? IdExpirationDate { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? PassportIssuanceDate   { get; set; }
        public DateTime? PassportExpirationDate { get; set; }
        //public string IdAttachment { get; set; }
        public IFormFile IdAttachmentFile { get; set; }

        //public string PassportAttachment { get; set; }
        public IFormFile PassportAttachmentFile { get; set; }
        public string UnifiedNumber { get; set; }
        public string VisaNo { get; set; }
        public DateTime? VisaIssuanceDate   { get; set; }
        public DateTime? VisaExpirationDate { get; set; }
       // public string VisaAttachment { get; set; }
        public IFormFile VisaAttachmentFile { get; set; }

    }
}
