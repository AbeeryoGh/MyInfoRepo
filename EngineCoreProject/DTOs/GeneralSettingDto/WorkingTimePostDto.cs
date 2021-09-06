using System;
using System.ComponentModel.DataAnnotations;
using EngineCoreProject.Models;
using System.Collections.Generic;

namespace EngineCoreProject.DTOs.GeneralSettingDto
{
    public class WorkingTimePostDto
    {
        [Required]
        public Dictionary<string, string> NameShortCutLangValue { get; set; }

        [Required]
        public int DayOfWeek { get; set; }  // 0 for Sunday.

        [Required]
        public int StartFrom { get; set; }
        [Required]
        public int FinishAt { get; set; }

        public DateTime? StartDateFrom { get; set; }

        public DateTime? FinishDateAt { get; set; }

        WorkingTimePostDto()
        {
        }
        public WorkingHours GetEntity()
        {
            WorkingHours workingHours = new WorkingHours
            {
                DayOfWeek = DayOfWeek,
                StartFrom = StartFrom,
                FinishAt = FinishAt,
                StartDate = StartDateFrom,
                FinishDate = FinishDateAt
            };

            return workingHours;
        }

    }
}
