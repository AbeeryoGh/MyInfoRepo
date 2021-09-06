using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs
{
    public class GetUserDto
    {
       [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
   


    }
}
