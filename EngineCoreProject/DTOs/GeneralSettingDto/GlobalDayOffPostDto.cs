using System;
using System.ComponentModel.DataAnnotations;
using EngineCoreProject.Models;

using System.Collections.Generic;

namespace EngineCoreProject.DTOs.GeneralSettingDto
{
    public class GlobalDayOffPostDto
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public Dictionary<string, string> ReasonShotCutLangValue { get; set; }
        public GlobalDayOffPostDto()
        {
        }
        public GlobalDayOff GetEntity()
        {
            GlobalDayOff dayOff = new GlobalDayOff
            {
                StartDate = StartDate,
                EndDate = EndDate
            };

            return dayOff;
        }

    }
}
