using System;
using System.Collections.Generic;

namespace EngineCoreProject.Models
{
    public partial class StagetypeView
    {
        public int Typeid { get; set; }
        public string Value { get; set; }
        public int? LookupTypeId { get; set; }
        public string Shortcut { get; set; }
        public string Trans { get; set; }
        public string Lang { get; set; }
        public int Lookupvalueid { get; set; }
    }
}
