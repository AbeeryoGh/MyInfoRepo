
using EngineCoreProject.DTOs.TabDto;
using EngineCoreProject.IRepository.ITabRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services.GeneralSetting;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EngineCoreProject.DTOs.RoleDto;
//using EngineCoreProject.IRepository.IUserRepository;
using Microsoft.AspNetCore.Identity;
using EngineCoreProject.IRepository.IUserRepository;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Transactions;

namespace EngineCoreProject.Services.TabService
{
    public class TabRepository : ITabRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGlobalDayOffRepository _iGlobalDayOffRepository;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly UserManager<User> _iUserManager;
        ValidatorException _exception;

        public TabRepository(EngineCoreDBContext EngineCoreDBContext, IGlobalDayOffRepository iGlobalDayOffRepository, UserManager<User> iUserManager, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGlobalDayOffRepository = iGlobalDayOffRepository;
            _iGeneralRepository = iGeneralRepository;
            _iUserManager = iUserManager;
            _exception = new ValidatorException();
        }

        public async Task<int> AddTab(TabPostDto tabPostDto)
        {
            // TODO: add validations.
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            Tab newTab = tabPostDto.GetEntity();
            newTab.TabNameShortcut = _iGeneralRepository.GenerateShortCut(Constants.TAB, Constants.TAB_NAME_SHORTCUT);
            await _iGeneralRepository.InsertUpdateSingleTranslation(newTab.TabNameShortcut, tabPostDto.NameShortCut);

            if (tabPostDto.IconImage != null)
            {
                using var ms = new MemoryStream();
                tabPostDto.IconImage.CopyTo(ms);
                newTab.Icon = ms.ToArray();
            }

            try
            {
                await _EngineCoreDBContext.Tab.AddAsync(newTab);
                await _EngineCoreDBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += " inner error is " + ex.InnerException.Message;
                }
                _exception.AttributeMessages.Add(msg);
                throw _exception;
            }

            scope.Complete();
            return newTab.Id;
        }

        public async Task<int> UpdateTab(TabPostDto tabPostDto, int rowId)
        {
            int res = 0;
            try
            {
                using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
                Tab originalTab = await _EngineCoreDBContext.Tab.Where(a => a.Id == rowId).FirstOrDefaultAsync();

                if (originalTab == null)
                {
                    return res;
                }

                if (tabPostDto.IconImage != null)
                {
                    using var ms = new MemoryStream();
                    tabPostDto.IconImage.CopyTo(ms);
                    originalTab.Icon = ms.ToArray();
                }

                originalTab.ParentId = tabPostDto.ParentId;
                originalTab.TabOrder = tabPostDto.TabOrder;
                originalTab.Link = tabPostDto.Link;
                originalTab.IconString = tabPostDto.IconString;

                _EngineCoreDBContext.Tab.Update(originalTab);
                _EngineCoreDBContext.SaveChanges();

                await _iGeneralRepository.InsertUpdateSingleTranslation(originalTab.TabNameShortcut, tabPostDto.NameShortCut);
                transaction.Commit();

                res = originalTab.Id;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += " inner error is " + ex.InnerException.Message;
                }
                _exception.AttributeMessages.Add(msg);
                throw _exception;
            }
            return res;
        }

        public async Task<int> UpdateIconTab(IFormFile IconImage, int rowId)
        {
            int res = 0;
            Tab originalTab = await _EngineCoreDBContext.Tab.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (originalTab == null)
            {
                return res;
            }

            if (IconImage != null)
            {
                using var ms = new MemoryStream();
                IconImage.CopyTo(ms);
                originalTab.Icon = ms.ToArray();
            }
            _EngineCoreDBContext.Tab.Update(originalTab);
            _EngineCoreDBContext.SaveChanges();
            res = originalTab.Id;
            return res;
        }

        public async Task<int> UpdateIconStringTab(string iconString, int rowId)
        {
            int res = 0;
            Tab originalTab = await _EngineCoreDBContext.Tab.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (originalTab == null)
            {
                return res;
            }

            originalTab.IconString = iconString;

            _EngineCoreDBContext.Tab.Update(originalTab);
            _EngineCoreDBContext.SaveChanges();
            res = originalTab.Id;
            return res;
        }

        public async Task<int> DeleteTab(int id, string lang)
        {
            int res = 0;
            Tab tab = await _EngineCoreDBContext.Tab.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (tab == null)
            {
                return res;
            }

            if (await _EngineCoreDBContext.RoleClaim.AnyAsync(x => x.ClaimType == CustomClaimTypes.Tab && x.ClaimValue == id.ToString()))
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "joinedRecord") + " " + id.ToString());
            }
            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
            await _iGeneralRepository.DeleteTranslation(tab.TabNameShortcut);
            _EngineCoreDBContext.Tab.Remove(tab);
            await _EngineCoreDBContext.SaveChangesAsync();
            await transaction.CommitAsync();
            res = tab.Id;
            return res;
        }

        public async Task<List<TabGetDto>> GetTabs(string lang)
        {
            var tabs = await _EngineCoreDBContext.Tab.ToListAsync();
            List<TabGetDto> result = new List<TabGetDto>();
            foreach (var row in tabs)
            {
                var tab = TabGetDto.GetDTO(row);
                var LangValue = await _iGeneralRepository.getTranslationsForShortCut(row.TabNameShortcut);
                if (LangValue.ContainsKey(lang))
                {
                    tab.Name = LangValue[lang];
                }
                result.Add(tab);
            }
            return result;
        }

        public async Task<List<UserTabGetDTO>> GetTabsByIds(List<int> tabIds, string lang)
        {
            List<TabGetDto> result = new List<TabGetDto>();

            var tabs = await _EngineCoreDBContext.Tab.ToListAsync();

            foreach (var tab in tabs)
            {
                var tabDto = TabGetDto.GetDTO(tab);
                tabDto.HasAccess = tabIds.Contains(tab.Id);
                var langValue = await _iGeneralRepository.getTranslationsForShortCut(tab.TabNameShortcut);
                if (langValue.ContainsKey(lang))
                {
                    tabDto.Name = langValue[lang];
                }
                result.Add(tabDto);
            }

            List<UserTabGetDTO> ordereTabs = new List<UserTabGetDTO>();
            DoRecursive(result, ordereTabs, null);
            return ordereTabs;
        }

        private void DoRecursive(List<TabGetDto> mytabs, List<UserTabGetDTO> res, int? parentId)
        {
            var list = mytabs.Where(p => p.ParentId == parentId).ToList().OrderBy(p => p.TabOrder);
            foreach (var x in list)
            {
                UserTabGetDTO fath = new UserTabGetDTO()
                {
                    Id = x.Id,
                    Link = x.Link,
                    Name = x.Name,
                    TabOrder = x.TabOrder,
                    HasAccess = x.HasAccess,
                    ParentId = x.ParentId,
                    IconBase64 = (x.Icon == null) ? "" : Convert.ToBase64String(x.Icon),
                    IconString = x.IconString,
                    Elements = new List<UserTabGetDTO>()
                };
                res.Add(fath);
                DoRecursive(mytabs, fath.Elements, x.Id);
            }
        }

        public async Task<List<UserTabGetDTO>> GetMyTabs(int userId, string lang)
        {
            var user = await _iUserManager.Users.Include(x => x.UserRole).ThenInclude(x => x.Role).ThenInclude(x => x.RoleClaim).FirstOrDefaultAsync(u => u.Id == userId);
            List<UserTabGetDTO> res = new List<UserTabGetDTO>();
            if (user == null)
            {
                return res;
            }

            var allTabs = await GetTabs(lang);

            if (!await _iUserManager.IsInRoleAsync(user, Constants.AdminPolicy))
            {
                var userRoles = user.UserRole;
                List<RoleClaim> userclaims = new List<RoleClaim>();
                foreach (var role in userRoles)
                {
                    userclaims.AddRange(role.Role.RoleClaim.ToList());
                }
                userclaims = userclaims.Distinct().ToList();
                var tabsId = userclaims.Where(x => x.ClaimType == CustomClaimTypes.Tab).Select(x => Int32.Parse(x.ClaimValue)).ToList();
                allTabs = allTabs.Where(x => tabsId.Contains(x.Id)).ToList();
            }

            if (allTabs.Count > 0)
            {
                DoRecursive(allTabs, res, null);
            }

            return res;
        }
    }
}
