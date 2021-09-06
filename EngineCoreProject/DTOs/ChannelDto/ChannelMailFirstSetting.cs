
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ChannelDto
{
    public class ChannelMailFirstSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }

    }
}



