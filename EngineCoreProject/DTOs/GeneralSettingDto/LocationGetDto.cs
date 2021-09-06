using System.ComponentModel.DataAnnotations;
using EngineCoreProject.Models;
using System.Collections.Generic;



namespace EngineCoreProject.DTOs.GeneralSettingDto
{
    public class LocationGetDto
    {
        public int LocationId { get; set; }

        public string EmaritLocationName { get; set; }
    }
}
