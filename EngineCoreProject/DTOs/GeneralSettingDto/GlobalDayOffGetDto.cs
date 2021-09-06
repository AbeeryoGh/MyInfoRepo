using System;
using EngineCoreProject.Models;
using System.Collections.Generic;

namespace EngineCoreProject.DTOs.GeneralSettingDto
{
    public class GlobalDayOffGetDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public Dictionary<string,string> Dayoff { get; set; }

        public static GlobalDayOffGetDto GetDTO(GlobalDayOff dayOff)
        {
            GlobalDayOffGetDto dto = new GlobalDayOffGetDto
            {
                Id = dayOff.Id,
                StartDate = dayOff.StartDate,
                EndDate = dayOff.EndDate,
            };

            return dto;
        }

    }
}
