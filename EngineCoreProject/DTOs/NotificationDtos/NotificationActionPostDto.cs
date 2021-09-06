using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.NotificationDtos
{
    public class NotificationActionPostDto
    {
        [Required]
        public List<int> ActionListId { get; set; }
       
    }
}
