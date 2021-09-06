using System.ComponentModel.DataAnnotations;
using EngineCoreProject.Models;
using System.Collections.Generic;



namespace EngineCoreProject.DTOs.GeneralSettingDto
{
    public class ServiceKindPostDto
    {
        [Required]
        public int EmployeeCount { get; set; }
        [Required]
        public int EstimatedTimePerProcess { get; set; }
        [Required]
        [StringLength (5, MinimumLength = 1)]
        public string Symbol { get; set; }

        [Required]
        public Dictionary<string, string> NameShortCutLangValue { get; set; }

        public ServiceKindPostDto()
        {
        }
        public ServiceKind GetEntity()
        {
            ServiceKind serviceKind = new ServiceKind
            {
                EmployeeCount = EmployeeCount,
                EstimatedTimePerProcess = EstimatedTimePerProcess,
                Symbol = Symbol
            };

            return serviceKind;
        }

    }
}
