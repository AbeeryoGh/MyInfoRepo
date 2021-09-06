using EngineCoreProject.Models;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.DTOs.AccountDto;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace EngineCoreProject.Services.UserService
{
    public class EmployeeSettingRepository : IEmployeeSettingRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        ValidatorException _exception;
        public EmployeeSettingRepository(EngineCoreDBContext EngineCoreDBContext,  IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _exception = new ValidatorException();
        }

        public async Task<int> AddEmployeeSetting(int enotaryUserId, EmployeeSettingPostDto employeeSettingPostDto, string lang)
        {

            if (await _EngineCoreDBContext.EmployeeSetting.AnyAsync(x => x.EnotaryId == enotaryUserId))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "existedBefore"));
                throw _exception;
            }

            EmployeeSetting employeeSetting = employeeSettingPostDto.GetEntity();
            employeeSetting.EnotaryId = enotaryUserId;
            _EngineCoreDBContext.EmployeeSetting.Add(employeeSetting);
            await _EngineCoreDBContext.SaveChangesAsync();
            return employeeSetting.Id;
        }


        public async Task<int> AddUpdatEmployeeSetting(int enotaryUserId, EmployeeSettingPostDto employeeSettingPostDto, string lang)
        {
            if (!await _EngineCoreDBContext.User.AnyAsync(x => x.Id == enotaryUserId))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }


            var employeeSetting = await _EngineCoreDBContext.EmployeeSetting.Where(x => x.EnotaryId == enotaryUserId).FirstOrDefaultAsync();
            if (employeeSetting == null)
            {
               return await AddEmployeeSetting(enotaryUserId, employeeSettingPostDto, lang);
            }

            employeeSetting.UserId = employeeSettingPostDto.EposUserId;
            employeeSetting.EntityCode = employeeSettingPostDto.EntityCode;
            employeeSetting.TerminalId = employeeSettingPostDto.TerminalId;
            employeeSetting.SessionToken = employeeSettingPostDto.SessionToken;
            employeeSetting.Tokens = employeeSettingPostDto.Tokens;
            employeeSetting.ExpiredSessionToken = employeeSettingPostDto.ExpiredSessionToken;
            employeeSetting.Channel = employeeSettingPostDto.Channel;
            employeeSetting.LocationCode = employeeSettingPostDto.LocationCode;
            employeeSetting.NotaryLocationId = employeeSettingPostDto.NotaryLocationId;
            employeeSetting.TransactionReference = employeeSettingPostDto.TransactionReference;
            employeeSetting.SourceReference = employeeSettingPostDto.SourceReference;
            employeeSetting.ActiveDirectoryAccount = employeeSettingPostDto.ActiveDirectoryAccount;
            employeeSetting.DefaultMuteVoice = employeeSettingPostDto.DefaultMuteVoice;
            employeeSetting.DefaultShowCam = employeeSettingPostDto.DefaultShowCam;
            employeeSetting.DefaultViewCards = employeeSettingPostDto.DefaultViewCards;
            if (employeeSettingPostDto.NotaryLocationId > 0)
            {
                if (!await _EngineCoreDBContext.Location.AnyAsync(x => x.Id == employeeSettingPostDto.NotaryLocationId))
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "missedLocation"));
                    throw _exception;

                }
                employeeSetting.NotaryLocationId = employeeSettingPostDto.NotaryLocationId;
            }
            try
            {

                _EngineCoreDBContext.EmployeeSetting.Update(employeeSetting);
                await _EngineCoreDBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var unknownError = ex.Message;
                if (ex.InnerException.Message != null)
                {
                    unknownError += " inner is  " + ex.InnerException.Message;
                }
               
                _exception.AttributeMessages.Add(unknownError);
                throw _exception;
            }
            return employeeSetting.Id;
        }


        public async Task<EmployeeSettingGetDto> GetEmployeeSetting(int userId, string lang)
        {
            var employeeSetting = await _EngineCoreDBContext.EmployeeSetting.Where(x => x.EnotaryId == userId).FirstOrDefaultAsync();

            EmployeeSettingGetDto res = new EmployeeSettingGetDto();
            if (employeeSetting != null)
            {
                res = EmployeeSettingGetDto.GetDto(employeeSetting);

                if (employeeSetting.NotaryLocationId != null)
                {
                    var location = await _EngineCoreDBContext.Location.Where(x => x.Id == employeeSetting.NotaryLocationId).FirstOrDefaultAsync();

                    if (location != null)
                    {
                        res.EnotaryLocationName = await _iGeneralRepository.GetTranslateByShortCut(lang, location.NameShortcut);
                        if (location.ParentLocationId != null)
                        {
                            var Emarit = await _EngineCoreDBContext.Location.Where(x => x.Id == location.ParentLocationId).FirstOrDefaultAsync();
                            res.EnotaryLocationName += "_" + await _iGeneralRepository.GetTranslateByShortCut(lang, Emarit.NameShortcut);
                        }
                        else
                        {
                            res.EnotaryLocationName += "_" + res.EnotaryLocationName;
                        }
                    }
                }


    
            }
            return res;
        }
      
    }
}
