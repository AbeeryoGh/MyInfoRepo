using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs
{
    public class RegisterDto
    {
        
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        //[StringLength(30, MinimumLength = 8, ErrorMessage = "Maximum 30 characters and minmum 8 characters")]
        [Required]
        public string PassWord { get; set; }
        [Required]
        public int RoleId { get; set; }// 
        [Required]
        public string MobileNo { get; set; }//



    }
}
