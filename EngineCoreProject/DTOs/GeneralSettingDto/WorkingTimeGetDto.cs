using System;
using EngineCoreProject.Models;
using System.Collections.Generic;

namespace EngineCoreProject.DTOs.GeneralSettingDto
{
    public class WorkingTimeGetDto
    {
        public int Id { get; set; }
        public string NameShortCut { get; set; }
        public int DayOfWeek { get; set; }  // start from 0 for Sunday.
        public int StartFrom { get; set; }
        public int FinishAt { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? FinishdateAt { get; set; }
        public Dictionary<string, string> Workingday {get;set;}


    public static WorkingTimeGetDto GetDTO(WorkingHours workingHours)
        {
            WorkingTimeGetDto dto = new WorkingTimeGetDto
            {
                Id = workingHours.Id,
                DayOfWeek = workingHours.DayOfWeek,
                StartFrom = workingHours.StartFrom,
                FinishAt = workingHours.FinishAt,
                StartDateFrom = workingHours.StartDate,
                FinishdateAt = workingHours.FinishDate
            };
        
            return dto;
        }

    }


    public class WorkingHoursDates
    {
        public DateTime Date { get; set; }

        public int Minutes { get; set; }

        public List<WorkingTimeGetDto> WorkingTimes { get; set; }
}
}
