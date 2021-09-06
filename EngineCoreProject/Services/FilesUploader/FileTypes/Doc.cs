using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.FilesUploader.FileTypes
{
    public sealed  class Doc : FileType
    {
        public Doc()
        {
            Name = "DOC";
            Description = "MS Word Documents";
            AddExtensions("doc", "docx");
            AddSignatures(
                new byte[] { 0xD0 ,0xCF, 0x11 ,0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }, 
                new byte[] { 0x50, 0x4B, 0x03 ,0x04 ,0x14 ,0x00, 0x06 ,0x00 }

            );
        }
    }
}
