using EngineCoreProject.DTOs.AccountDto;
using EngineCoreProject.DTOs.RoleDto;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EngineCoreProject.IRepository.IUserRepository
{
    public interface IUserRepository
    {
        Task<bool> IsExist(string EmiratId);
        Task<UserDto> GetOne(int Id);
        public int GetUserID();
        public string GetUserName();
        public bool IsAdmin();
        public bool IsInspector();
        public bool IsEmployee();
        public Task<bool> IsEmployee(int userId);
        public string GetUserEid();
        public string GetUserEmail();
        public Task<UserDto> FindUserById(int id, string lang);
        public Task<int> VisitorsCount();
        public Task<object> Addoldusers();
        public Task<string> RefreshToken();
        public Task<LogInResultDto> VisitorSignIn(LogInDtoLocal logInDto, string lang);
        public Task<LogInResultDto> LocalSignIn(LogInDtoLocal logInDto, string lang);
        public Task<LogInResultDto> WindowsSignIn(string accountName, string lang);
        public Task<LogInResultDto> UGSignIn(LogInDtoUg logInDto, string lang);
        public Task<List<RoleGetDto>> GetUserRoles(int userId, string lang);
        public Task<List<UserDto>> GetUsersRoles(string blindSearch, string lang);
        public Task<UserResultDto> EditUser(int id, UserPostDto UserPostDto, bool fromUg, bool updateRoles, string lang);
        public Task<bool> AddEditSignature(SignaturePostDto signaturePostDto, string lang);
        public Task<string> AddEditSignature64(string signatureBase64, string lang);
        public Task<bool> EditPassword(EditUserPasswordDTO editUserPasswordDTO, string lang);
        public Task<UserResultDto> CreateUser(UserPostDto UserPostDto, string ImageUrl, bool updateRoles, string lang, bool FromUg);
        public Task<UserResultDto> DeleteUser(int id);
        public Task<List<UserDto>> GetUsers();
        public Task<UserPermissionsDTO> GetUserPermissions(int userId);
        public Task<IdentityResult> EditUserRolesAsync(int userId, List<int> userRoles);
        public Task<List<int>> GetUserActionsPermissions(int userId);
        public Task<List<int>> GetUserTabsPermissions(string lang);
        public void SignOut();
        public Task<Dictionary<int, string>> GetEmployees();

        public Task<List<OnLineEmployee>> GetOnlineEmployees();
        public Task<bool> StartStopWork(bool? start);
        public Task<CreateUserOldResultDto> CreateUserForOldAppParties(OldUserPostDto UserPostDto);
    }
}
