using EngineCoreProject.DTOs.GeneralSettingDto;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IGeneralSetting
{
    public interface IWorkingTimeRepository
    {
        Task<int> AddWorkingTime(WorkingTimePostDto workingPostDto, string lang = null);
        Task<int> UpdateWorkingTime(WorkingTimePostDto workingTimePostDto, int rowId, string lang = null);
        Task<int> DeleteWorkingTime(int id, string lang);
        Task<List<WorkingTimeGetDto>> GetWorkingHours(string lang);
        Task<WorkingTimeGetDto> GetWorkTimeId(int id, string lang);
        Task<List<WorkingTimeGetDto>> GetWorkingForDate(DateTime date);
        Task<int> GetWorkingMinutesBetweenTwoDates(DateTime sdateTime, DateTime edateTime);
        Task<Dictionary<DateTime, int>> GetWorkingDates(DateTime untilDate);
        Task InitialaizeWorkingDic(DateTime minDate, DateTime maxDate);
        Task<bool> IsOrderLate(DateTime sDateTime, DateTime eDateTime, int timePeriodInMinutes);
        Task<bool> IsInsideWorkingHours(DateTime checkDateTime);

        Task<Dictionary<int,DateTime>> GetDeadline(List<int> hours);
    }
}
