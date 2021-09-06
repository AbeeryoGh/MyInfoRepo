using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TemplateSetDtos.ModelView
{
    public class PartyView
    {
        public int    PartyTypeId          { get; set; }
        public int    RelationId       { get; set; }
        public int    TranslationId    { get; set; }
        public string PartyName        { get; set; }
        public bool   Required         { get; set; }
        public string RequiredText     { get; set; }
        public bool   SignRequired     { get; set; }
        public string SignRequiredText { get; set; }
    }
}
