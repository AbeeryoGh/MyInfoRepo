using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.NotificationDtos
{
    public class NotificationTemplatesActionPostDto
    {
        [Required]
        public int ActionId { get; set; }

        [Required]
        public List<int> NotificationTemplateIds { get; set; }
       
    }
}
