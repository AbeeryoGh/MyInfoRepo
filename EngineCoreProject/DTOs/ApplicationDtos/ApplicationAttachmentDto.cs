
namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ApplicationAttachmentDto
    {

        public int? ApplicationId { get; set; }
        public int? AttachmentId { get; set; }   
        public string MimeType { get; set; }
        public int? Size { get; set; }
        public string FileName { get; set; }
        public string Note { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }

    }
}
