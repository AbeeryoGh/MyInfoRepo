using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.UnifiedGatePostDto
{
    public class UnifiedGateUserInformationPostDto
    {
        public UnifiedGateUserInformationPostDto() 
        {
            dtData = new Dtdata[1] { new Dtdata()};
        }
        public Dtdata[] dtData { get; set; }
    }

    

    public class Dtdata
    {
        public Dtdata()
        {
            
        }
        [Required]
        public string Lang { get; set; }
        [Required]
        public string EmiratesId { get; set; }
        [Required]
        public string EmailID { get; set; }
    }

}
