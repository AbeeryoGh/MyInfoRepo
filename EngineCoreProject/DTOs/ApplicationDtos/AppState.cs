using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class AppState
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsCurrent { get; set; }
    }
}
