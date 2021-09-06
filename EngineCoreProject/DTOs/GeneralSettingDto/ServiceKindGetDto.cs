using System;
using EngineCoreProject.Models;
using System.Collections.Generic;

namespace EngineCoreProject.DTOs.GeneralSettingDto
{
    public class ServiceKindGetDto
    {
        public int Id { get; set; }
        public int EmployeeCount { get; set; }
        public int EstimatedTimePerProcess { get; set; }
        public Dictionary<string, string> serviceName { get; set; }
        public string Symbol { get; set; }

        public string NameShortCut { get; set; }

        public static ServiceKindGetDto GetDTO(ServiceKind serviceKind)
        {
            ServiceKindGetDto dto = new ServiceKindGetDto
            {
                Id = serviceKind.Id,
                EmployeeCount = serviceKind.EmployeeCount,
                EstimatedTimePerProcess = serviceKind.EstimatedTimePerProcess,
                Symbol = serviceKind.Symbol
            };
            return dto;
        }

    }
}
