using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.FilesUploader.FileTypes
{
    public sealed class Pdf : FileType
    {
        public Pdf()
        {
            Name = "PDF";
            Description = "PDF Document";
            AddExtensions("pdf");
            AddSignatures(
                new byte[] { 0x25, 0x50, 0x44, 0x46 }
            );
        }
    }
}
