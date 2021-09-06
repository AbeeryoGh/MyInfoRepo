using EngineCoreProject.Models;
using System;


namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ApplicationPartyDto
    {
        public int? PartyId { get; set; }
        public int? TransactionId { get; set; }
        public bool? IsOwner { get; set; }
        public int? PartyTypeId { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int? Nationality { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? MaritalStatus { get; set; }
        public int? Gender { get; set; }
        public string EmiratesIdNo { get; set; }
        public DateTime? IdExpirationDate { get; set; }
        public string IdAttachment { get; set; }
        public string UnifiedNumber { get; set; }
        public bool? SignRequired { get; set; }
        public bool? Signed { get; set; }
        public int? SignType { get; set; }
        public DateTime? SignDate { get; set; }
        public string SignUrl { get; set; }
        public bool? EditableSign { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int? Emirate { get; set; }
        public string City { get; set; }
        public string AlternativeEmail { get; set; }
        public int? NotaryId  { get; set; }

        public static ApplicationPartyDto GetDTO(ApplicationParty appParty)
        {
            ApplicationPartyDto dto = new ApplicationPartyDto
            {
                PartyId = appParty.PartyId,
                TransactionId = appParty.TransactionId,
                IsOwner = appParty.IsOwner,
                PartyTypeId = appParty.PartyTypeValueId,
                FullName = appParty.FullName,
                Mobile = appParty.Mobile,
                Email = appParty.Email,
                Nationality = appParty.Nationality,
                BirthDate = appParty.BirthDate,
                MaritalStatus = appParty.MaritalStatus,
                Gender = appParty.Gender,
                EmiratesIdNo = appParty.EmiratesIdNo,
                IdExpirationDate = appParty.IdExpirationDate,
                IdAttachment = appParty.IdAttachment,
                UnifiedNumber = appParty.UnifiedNumber,
                SignRequired = appParty.SignRequired,
                Signed = appParty.Signed,
                SignType = appParty.SignType,
                SignDate = appParty.SignDate,
                SignUrl = appParty.SignUrl,
                EditableSign = appParty.EditableSign,
                Address = appParty.Address,
                Description = appParty.Description,
                City = appParty.City,
                AlternativeEmail = appParty.AlternativeEmail,
                Emirate = appParty.Emirate,
                NotaryId= appParty.NotaryId

            };
            return dto;
        }
    }
}