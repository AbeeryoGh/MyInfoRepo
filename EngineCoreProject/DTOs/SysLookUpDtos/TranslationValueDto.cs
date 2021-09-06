using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.SysLookUpDtos
{
    public class TranslationValueDto
    {
        [Required]
        public int TypeId { get; set; }
        
    }
}
