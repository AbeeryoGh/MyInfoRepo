using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AccountDto
{
    public class LogInDtoLocal
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PassWord { get; set; }
    }

    public class LogInDtoUg
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PassWord { get; set; }
        public int? ServiceId { get; set; }

        public int? AppId { get; set; }
    }

    public class LogInDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PassWord { get; set; }
        public int? ServiceId { get; set; }

        public int? AppId { get; set; }
        public int loginType { get; set; }   // 0 for local user, 1 for UG, 2 for visitor.
    }
}
