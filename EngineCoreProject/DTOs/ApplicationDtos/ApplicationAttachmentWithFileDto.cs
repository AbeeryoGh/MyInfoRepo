using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ApplicationAttachmentWithFileDto
    {
        public int? ApplicationId { get; set; }
        public int? AttachmentId { get; set; }
        public    IFormFile File { get; set; }
    }
}
