using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.oldDataDto
{
    public class OldDto
    {
        public ApplicationInfo toSave { get; set; }
    }
}


public class targetApplicationDtos
{
    
        public int TargetAppId { get; set; }
     
}



public class ApplicationInfo
{
    public List<targetApplicationDtos> targetApplicationDtos { get; set; }
}