using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class ApplicationPartyView : ApplicationPartyDto
    {

        public int Id { get; set; }
        public string  PartyTypeName { get; set; }
        public string  IsOwnerText { get; set; }
        public string  SignRequiredText { get; set; } 
        public string  SignedText { get; set; }

        public ICollection<ApplicationPartyExtraAttachment> PartyExtraAttachment { get; set; }

    }
}
