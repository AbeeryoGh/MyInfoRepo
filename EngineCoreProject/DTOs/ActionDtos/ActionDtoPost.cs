using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ActionDtos
{
    public class ActionDtoPost
    {
                  
          
            public bool? ActiveNotification { get; set; }
            public int NotificationTypeId { get; set; }
            public string actionName { get; set; }
            public string Language { get; set; }

        
    }
}
