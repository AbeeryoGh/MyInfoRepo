using EngineCoreProject.DTOs.GeneralSettingDto;
using EngineCoreProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EngineCoreProject.IRepository.IGeneralSetting
{
    public interface IServiceKindRepository
    {
        Task<int> AddServiceKind(ServiceKindPostDto ServiceKindPostDto, string lang);

        Task<int> UpdateServiceKind(ServiceKindPostDto ServiceKindPostDto, int id, string lang);

        Task<int> DeleteServiceKind(int id);
        Task<List<ServiceKindGetDto>> GetServiceKinds(string lang);

        Task<ServiceKindGetDto> GetServiceKindById(int id, string lang);

    }
}
