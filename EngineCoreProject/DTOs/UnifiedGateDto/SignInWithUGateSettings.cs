using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.UnifiedGateDto
{
    public class SignInWithUGateSettings
    {
        public string domain { get; set; }
        public string EL_Portal { get; set; }
        public string OutCommingSecretKey { get; set; }  
        public string InCommingSecretKey { get; set; }
        public string signOut { get; set; }
        public string secret_key { get; set; }
    }
}
 