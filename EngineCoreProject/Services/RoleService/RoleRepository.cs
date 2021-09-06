
using EngineCoreProject.IRepository.IRoleRepository;
using EngineCoreProject.Models;
using EngineCoreProject.DTOs.RoleDto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Transactions;
using EngineCoreProject.IRepository.ITabRepository;
using EngineCoreProject.Properties;
using EngineCoreProject.IRepository.AdmServices;
using Microsoft.AspNetCore.Authorization;

namespace EngineCoreProject.Services.RoleService
{
    public class RoleRepository : IRoleRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly RoleManager<Role> _roleManager;
        private readonly ITabRepository _iTabRepository;
        private readonly IStageActionsRepository _iStageActionsRepository;
        ValidatorException _exception;

        public RoleRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository, RoleManager<Role> roleManager, ITabRepository iTabRepository, IStageActionsRepository iStageActionsRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _roleManager = roleManager;
            _iTabRepository = iTabRepository;
            _iStageActionsRepository = iStageActionsRepository;
            _exception = new ValidatorException();
        }

        public async Task CreateRoleWithPermissionsAsync(RoleWithPermissionsPostDto rolePostDto, string lang)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (rolePostDto.RoleNameShortCut.Values.ToList().Count < 1)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "missedRoleName"));
                throw _exception;
            }

            if (await DuplicateRoleNames(rolePostDto.RoleNameShortCut.Values.ToList(), 0))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "duplicatedRoleName"));
                throw _exception;
            }

            var roleNameShortcut = _iGeneralRepository.GenerateShortCut(Constants.ROLE, Constants.ROLE_NAME_SHORTCUT);
            var newRole = new Role(roleNameShortcut);
            var result = await _roleManager.CreateAsync(newRole);
            await _iGeneralRepository.InsertUpdateSingleTranslation(roleNameShortcut, rolePostDto.RoleNameShortCut);

            // add permissions.
            var role = await _roleManager.FindByNameAsync(roleNameShortcut);
            foreach (var claim in rolePostDto.Permissions)
            {
                foreach (var per in claim.Value)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(claim.Key, per));
                }
            }
            scope.Complete();
        }

        public async Task<IdentityResult> CreateRoleAsync(RolePostDto rolePostDto, string lang)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (rolePostDto.RoleNameShortCut.Values.ToList().Count < 1)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "missedRoleName"));
                throw _exception;
            }

            if (await DuplicateRoleNames(rolePostDto.RoleNameShortCut.Values.ToList(), 0))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "duplicatedRoleName"));
                throw _exception;
            }

            var roleNameShortcut = _iGeneralRepository.GenerateShortCut(Constants.ROLE, Constants.ROLE_NAME_SHORTCUT);
            var newRole = new Role(roleNameShortcut);
            var result = await _roleManager.CreateAsync(newRole);
            await _iGeneralRepository.InsertUpdateSingleTranslation(roleNameShortcut, rolePostDto.RoleNameShortCut);
            scope.Complete();
            return result;
        }

        public async Task<IdentityResult> CreateHardCodedRoleAsync(string roleName)
        {
            var newRole = new Role(roleName);
            var result = await _roleManager.CreateAsync(newRole);
            return result;
        }

        public async Task<int> UpdateRole(RolePostDto rolePostDto, int roleId, string lang)
        {
            int res = 0;
            Role originalRole = await _EngineCoreDBContext.Role.Where(a => a.Id == roleId).FirstOrDefaultAsync();
            if (originalRole == null)
            {
                return res;
            }

            if (rolePostDto.RoleNameShortCut.Values.ToList().Count < 1)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "missedRoleName"));
                throw _exception;
            }

            if (await DuplicateRoleNames(rolePostDto.RoleNameShortCut.Values.ToList(), roleId))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "duplicatedRoleName"));
                throw _exception;
            }

            await _iGeneralRepository.InsertUpdateSingleTranslation(originalRole.Name, rolePostDto.RoleNameShortCut);
            res = originalRole.Id;
            return res;
        }

        public async Task<IdentityResult> DeleteRoleAsync(int roleId, string lang)
        {
            var role = await _roleManager.Roles.Where(x => x.Id == roleId).FirstOrDefaultAsync();
            if (role == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "RoleNotFound"));
                throw _exception;
            }

            if (role.Name == Constants.AdminPolicy || role.Name == Constants.EmployeePolicy || role.Name == Constants.DefaultUserPolicy || role.Name == Constants.DefaultVisitorPolicy)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "MainRole"));
                throw _exception;
            }

            if (await _EngineCoreDBContext.UserRole.AnyAsync(x => x.RoleId == roleId))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "RoleJoined"));
                throw _exception;
            }

            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await _iGeneralRepository.DeleteTranslation(role.Name);
            var result = await _roleManager.DeleteAsync(role);
            scope.Complete();
            return result;
        }


        public async Task<List<RoleGetDto>> GetAllRoles(string lang)
        {
            List<RoleGetDto> roles = new List<RoleGetDto>();
            foreach (var role in _roleManager.Roles.ToList())
            {
                var LangValue = await _iGeneralRepository.getTranslationsForShortCut(role.Name);
                RoleGetDto roleDTO = new RoleGetDto()
                {
                    Id = role.Id,
                    RoleName = LangValue.ContainsKey(lang) ? LangValue[lang] : role.Name
                };

                roles.Add(roleDTO);
            }
            return roles;
        }

        public async Task<RolePermissionsGetDTO> GetRolePermissions(int roleId, string lang)
        {
            RolePermissionsGetDTO rolePermissionsDTO = new RolePermissionsGetDTO();

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return rolePermissionsDTO;
            }
            rolePermissionsDTO.RoleId = roleId;

            var roleClaims = await _roleManager.GetClaimsAsync(role);

            var actions = roleClaims.Where(x => x.Type == CustomClaimTypes.Action).Select(x => Int32.Parse(x.Value)).ToList();
            if (actions != null)
            {
                rolePermissionsDTO.ActionPermissions = await _iStageActionsRepository.GetActionstoRole(actions, lang);
            }

            var tabs = roleClaims.Where(x => x.Type == CustomClaimTypes.Tab).Select(x => x.Value).ToList();
            if (tabs != null)
            {
                rolePermissionsDTO.TabPermissions = await _iTabRepository.GetTabsByIds(tabs.Select(int.Parse).ToList(), lang);
            }
            return rolePermissionsDTO;
        }

        public async Task<IdentityResult> AddUpdateActionToRoles(int actionId, List<int> roles)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (await _EngineCoreDBContext.AdmAction.AnyAsync(x => x.Id == actionId))
            {
                var oldCalims = _EngineCoreDBContext.RoleClaim.Where(x => x.ClaimType == CustomClaimTypes.Action && x.ClaimValue == actionId.ToString());
                _EngineCoreDBContext.RoleClaim.RemoveRange(oldCalims);

                foreach (var role in roles)
                {
                    var newClaim = new Claim(CustomClaimTypes.Action, actionId.ToString());
                    await AddPermissionToRole(role, newClaim);
                }
            }

            scope.Complete();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> AddUpdatePermissionsToRole(RolePermissionsPostDTO rolePermissionsDTO, string lang)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var res = IdentityResult.Success;
            var role = await _roleManager.FindByIdAsync(rolePermissionsDTO.RoleID.ToString());
            if (role == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "missedRoleName"));
                throw _exception;
            }

            var oldRoleClaims = await _EngineCoreDBContext.RoleClaim.Where(x => x.RoleId == rolePermissionsDTO.RoleID).ToListAsync();
            _EngineCoreDBContext.RoleClaim.RemoveRange(oldRoleClaims);

            List<RoleClaim> roleActionClaims = new List<RoleClaim>();
            foreach (var x in rolePermissionsDTO.ActionPermissions.Distinct())
            {
                roleActionClaims.Add(new RoleClaim { RoleId = rolePermissionsDTO.RoleID, ClaimType = CustomClaimTypes.Action, ClaimValue = x.ToString() });
            }
            await _EngineCoreDBContext.RoleClaim.AddRangeAsync(roleActionClaims);

            List<RoleClaim> roleTabClaims = new List<RoleClaim>();
            foreach (var x in rolePermissionsDTO.TabPermissions.Distinct())
            {
                roleTabClaims.Add(new RoleClaim { RoleId = rolePermissionsDTO.RoleID, ClaimType = CustomClaimTypes.Tab, ClaimValue = x.ToString() });
            }
            await _EngineCoreDBContext.RoleClaim.AddRangeAsync(roleTabClaims);

            await _EngineCoreDBContext.SaveChangesAsync();
            scope.Complete();
            return res;
        }

        private async Task<IdentityResult> AddPermissionToRole(int roleId, Claim claim)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role != null)
            {
                var oldClaims = await _roleManager.GetClaimsAsync(role);
                if (!oldClaims.Contains(claim))
                {
                   return await _roleManager.AddClaimAsync(role, claim);
                }
            }
            return IdentityResult.Success;
        }


        private async Task<bool> DuplicateRoleNames(List<string> newNames, int exclude)
        {
            var names = await _roleManager.Roles.Where(x => x.Id != exclude).Select(x => x.Name).ToListAsync();
            List<string> oldNames = new List<string>();
            foreach (var name in names)
            {
                var trans = await _iGeneralRepository.getTranslationsForShortCut(name);
                oldNames.AddRange(trans.Values.ToList());
            }

            if (oldNames.Intersect(newNames).Count() > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<List<int>> GetRolesIdByUserId(int userId)
        {
            var RolesId =await _EngineCoreDBContext.UserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToListAsync();

            return RolesId;
        }
    }
}
