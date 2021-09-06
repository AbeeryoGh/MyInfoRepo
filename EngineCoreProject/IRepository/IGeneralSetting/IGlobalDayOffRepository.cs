using EngineCoreProject.DTOs.GeneralSettingDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EngineCoreProject.Services.GeneralSetting
{
    public interface IGlobalDayOffRepository
    {
        int ThrowExceptionTest(bool throwException);
        Task<int> AddDayOff(GlobalDayOffPostDto GlobalDayOffPostDto, string lang);

        Task<int> UpdateDayOff(GlobalDayOffPostDto GlobalDayOffPostDto, string lang, int rowId);

        Task<int> DeleteDayOff(int id);

        Task<List<GlobalDayOffGetDto>> GetDaysOff(string lang);

        Task<GlobalDayOffGetDto> GetDayOff(int rowId, string lang);

        Task<bool> IsDayOff(DateTime checkDate);

    }
}