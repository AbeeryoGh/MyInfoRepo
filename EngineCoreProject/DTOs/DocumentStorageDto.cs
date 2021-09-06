using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs
{
    public class DocumentStorageDto
    {
        public int Id { get; set; }
        public string File_Name { get; set; }
        public string File_Path { get; set; }
        public string id_User { get; set; }
    }
}
