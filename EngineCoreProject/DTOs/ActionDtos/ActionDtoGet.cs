using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ActionDtos
{
    public class ActionDtoGet
    {

        [Required]
        public int? action_id { get; set; }
        [Required]
        public string action_translate { get; set; }


    }
}
