using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AdmService
{
    public class serviceIconDto
    {
        public int? serviceid { get; set; }
        public IFormFile file { get; set; }
    }
}
