using System.ComponentModel.DataAnnotations;
using EngineCoreProject.Models;
using System.Collections.Generic;



namespace EngineCoreProject.DTOs.GeneralSettingDto
{
    public class LocationPostDto
    {
        [Required]
        public Dictionary<string, string> NameShortCutLangValue { get; set; }

        public int? ParentId { get; set; }
    }
}
