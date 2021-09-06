using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.SysLookUpDtos
{
    public class TranslationTypeDtoGet
    {
        [Required]
        public int TypeID { get; set; }
        [Required]
        public string shortcut { get; set; }

        public string translationValue { get; set; }
    }
    public class AllTranslationTypeDtoGet
    {
        [Required]
        public int TypeID { get; set; }
        [Required]
        public string shortcut { get; set; }

        public string translationValue { get; set; }
        public string lang { get; set; }
    }
}

 