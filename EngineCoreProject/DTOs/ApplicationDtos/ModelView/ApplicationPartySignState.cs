using System;


namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class ApplicationPartySignState
    {
        public int Id { get; set; }
        public int? PartyId { get; set; }
        public bool? IsOwner { get; set; }
        public int? PartyTypeId { get; set; }
        public string FullName { get; set; }
        public bool? SignRequired { get; set; }
        public bool? Signed { get; set; }
        public int? SignType { get; set; }
        public DateTime? SignDate { get; set; }
        public string SignUrl { get; set; }
        public bool? EditableSign { get; set; }
        public int? NotaryId { get; set; }
    }
}
