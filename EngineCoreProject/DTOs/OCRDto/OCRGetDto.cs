using EngineCoreProject.DTOs.FileDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.OCRDto
{
    public class OCRGetDto
    {
        public string OCRResult { get; set; }
        public UploadedFileMessage FileUploadResult { get; set; }
    }
}
