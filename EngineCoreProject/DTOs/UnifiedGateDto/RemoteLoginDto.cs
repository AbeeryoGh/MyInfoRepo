using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.UnifiedGateDto
{
    //https://elawyeruat.moj.gov.ae/mainapi/remotelogin/784198402171619/0543426442/M2QwZmYxZDdjNzJjYzIzMjAyZDllNmUwMDYzZWQ0NmI4Y2VhNzJkZGM0NzIyZDI1ZmE0ZGNiNjc0YTA3ZjZmNQ==/0/j.cherian@smartv.ae/ar
    public class RemoteLoginDto
    {
        [Required]
        public string EmiratesId { set; get; }
        [Required]
        public string MobileNumber { set; get; }
         
        [Required]
        public string Hash { set; get; }

        [Required]
        public string ServiceId { set; get; }

        [Required]
        public string Email { set; get; }

        [Required]
        public string Lang { set; get; }

        [Required]
        public string theme { set; get; }


    }
}
 