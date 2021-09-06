using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.FileDto
{
    public class UploadedFileMessage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public bool SuccessUpload { get; set; }
        public string Message { get; set; }
        public string FileUrl { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
    }
}
