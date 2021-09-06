using System;


namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ApplicationDto
    {
        public int?   ServiceId { get; set; }
        public string ApplicationNo { get; set; }
        public int? TemplateId     { get; set; }
        public int? CurrentStageId { get; set; }
        public int? StateId { get; set; }
        public string Note  { get; set; }
        public DateTime? AppLastUpdateDate { get; set; }
        public int? LastUpdatedBy { get; set; }

        public byte[] RowVersion { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public int? Channel { get; set; }
        public int? Reason { get; set; }
        public bool? Locked { get; set; }
        public bool? Delivery { get; set; }
        public DateTime? LastReadDate { get; set; }
        public int? LastReader { get; set; }
        


    }
}
