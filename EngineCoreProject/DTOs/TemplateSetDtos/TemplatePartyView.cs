using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TemplateSetDtos
{
    public class TemplatePartyView
    {
        public int Id { get; set; }
        public string PartyName { get; set; }
        public string Required { get; set; }
        public bool? sign_required { get; set; }
    }
}
