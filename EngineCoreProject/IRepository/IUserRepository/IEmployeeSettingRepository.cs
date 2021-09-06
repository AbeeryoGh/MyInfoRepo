
using EngineCoreProject.DTOs.AccountDto;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EngineCoreProject.IRepository.IUserRepository
{
    public interface IEmployeeSettingRepository
    {
        Task<int> AddEmployeeSetting(int enotaryUserId, EmployeeSettingPostDto employeeSettingPostDto, string lang);
        Task<int> AddUpdatEmployeeSetting(int enotaryUserId, EmployeeSettingPostDto employeeSettingPostDto, string lang);

        Task<EmployeeSettingGetDto> GetEmployeeSetting(int userId, string lang);
    }
}
