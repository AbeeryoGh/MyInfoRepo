using EngineCoreProject.DTOs.GeneralSettingDto;
using EngineCoreProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EngineCoreProject.IRepository.IGeneralSetting
{
    public interface ILocationRepository
    {
        Task<int> AddLocation(LocationPostDto locationPostDto, string lang);

        Task<List<LocationGetDto>> GetLocations(string lang);
    }
}
