using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.FileDto
{
    public class CreateFolderMessage
    {
        //public string FolderName { get; set; }
        public bool SuccessCreation { get; set; } = false;
        public string Message { get; set; }
        public string FolderPath { get; set; }
    }
}
