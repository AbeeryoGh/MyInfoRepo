using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class ApplicationPartyFinalDocument : Documentation
    {
       
        public int    Id { get; set; }
        public string FullName     { get; set; }
        public string  Nationality  { get; set; }
        public bool?   IsOwner      { get; set; } 
        public string PartyType    { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string SignUrl { get; set; }

        public bool? SignRequired { get; set; }

        public string  EmirateId     { get; set; }
        public string  EmirateIdUrl  { get; set; }


    }
}
