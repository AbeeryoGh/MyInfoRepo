using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ChannelDto
{
    public class ChannelGetDto
    {
        [Required]
        public int? Id { get; set; }
        [Required]
        public string ChannelName { get; set; }
        public string ChannelNameShortcut { get; set; }
    }
}
