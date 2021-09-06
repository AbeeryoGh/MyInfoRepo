using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.SysLookUpDtos
{
    public class TranslationTableDto
    {
        [Required]
        public string shortcut { get; set; }

        [Required]
        public string lang { get; set; }

        [Required]
        public string value { get; set; }
    }
}
