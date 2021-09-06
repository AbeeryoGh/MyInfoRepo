using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TemplateSetDtos
{
    public class TypeList
    {
        public int Id { get; set; }
        public int ValueId { get; set; }
        public string Value { get; set; }
        public bool? BoolParameter { get; set; }
        public int? Order { get; set; }

        public string ShortCut { get; set; }

    }
}
