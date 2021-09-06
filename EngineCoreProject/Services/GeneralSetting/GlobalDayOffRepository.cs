using EngineCoreProject.DTOs.GeneralSettingDto;
using EngineCoreProject.IRepository.IGeneralSetting;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EngineCoreProject.Services.GeneralSetting
{
    public class GlobalDayOffRepository : IGlobalDayOffRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        ValidatorException _exception;
        public GlobalDayOffRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _exception = new ValidatorException();
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
        }

        public int ThrowExceptionTest (bool throwException)
        {
            if (throwException)
            {
                _exception.AttributeMessages.Add(Constants.getMessage("ar", "missedParameter"));
                _exception.AttributeMessages.Add(Constants.getMessage("ar", "UnknownError"));
                _exception.AttributeMessages.Add(Constants.getMessage("ar", "ExistedPhoneNumber"));

                throw _exception;
            }

            return 0;
        }
        public async Task<int> AddDayOff(GlobalDayOffPostDto globalDayOffPostDto, string lang)
        {
            ValidateDayOff(globalDayOffPostDto, lang);
            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();

            GlobalDayOff dayOff = globalDayOffPostDto.GetEntity();
            dayOff.ReasonShortcut = _iGeneralRepository.GenerateShortCut(Constants.DAY_OFF, Constants.DAY_OFF_REASON_SHORTCUT);
            _EngineCoreDBContext.GlobalDayOff.Add(dayOff);
            _EngineCoreDBContext.SaveChanges();

            await _iGeneralRepository.InsertUpdateSingleTranslation(dayOff.ReasonShortcut, globalDayOffPostDto.ReasonShotCutLangValue);
            await transaction.CommitAsync();
            return dayOff.Id;
        }

        public async Task<int> UpdateDayOff(GlobalDayOffPostDto globalDayOffPostDto, string lang, int rowId)
        {
            int res = 0;
            ValidateDayOff(globalDayOffPostDto, lang, rowId);
            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();

            GlobalDayOff originalDayOff = await _EngineCoreDBContext.GlobalDayOff.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (originalDayOff == null)
            {
                return res;
            }

            originalDayOff.StartDate = globalDayOffPostDto.StartDate;
            originalDayOff.EndDate = globalDayOffPostDto.EndDate;
            _EngineCoreDBContext.GlobalDayOff.Update(originalDayOff);
            _EngineCoreDBContext.SaveChanges();

            await _iGeneralRepository.InsertUpdateSingleTranslation(originalDayOff.ReasonShortcut, globalDayOffPostDto.ReasonShotCutLangValue);

            transaction.Commit();
            res = originalDayOff.Id;
            return res;
        }

        public async Task<int> DeleteDayOff(int id)
        {
            int res = 0;
            GlobalDayOff originalDayOff = await _EngineCoreDBContext.GlobalDayOff.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (originalDayOff == null)
            {
                return res;
            }

            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();

            await _iGeneralRepository.DeleteTranslation(originalDayOff.ReasonShortcut);
            _EngineCoreDBContext.GlobalDayOff.Remove(originalDayOff);
            _EngineCoreDBContext.SaveChanges();
            transaction.Commit();
            res = originalDayOff.Id;
            return res;
        }

        public async Task<List<GlobalDayOffGetDto>> GetDaysOff(string lang)
        {
            var daysOff = await _EngineCoreDBContext.GlobalDayOff.ToListAsync();
            List<GlobalDayOffGetDto> result = new List<GlobalDayOffGetDto>();
            foreach (var row in daysOff)
            {
                var res = GlobalDayOffGetDto.GetDTO(row);
                var LangValue = await _iGeneralRepository.getTranslationsForShortCut(row.ReasonShortcut);
                if (LangValue.ContainsKey(lang))
                {
                    res.Dayoff = LangValue;
                    res.Reason = LangValue[lang];
                }
                result.Add(res);
            }
            return result;
        }

        public async Task<GlobalDayOffGetDto> GetDayOff(int rowId, string lang)
        {
            GlobalDayOff dayOffRec = new GlobalDayOff();
            dayOffRec = await _EngineCoreDBContext.GlobalDayOff.Where(d => d.Id == rowId).FirstOrDefaultAsync();

            GlobalDayOffGetDto res = new GlobalDayOffGetDto();
            if (dayOffRec == null)
            {
                return res;
            }

            res = GlobalDayOffGetDto.GetDTO(dayOffRec);
            var LangValue = await _iGeneralRepository.getTranslationsForShortCut(dayOffRec.ReasonShortcut);
            if (LangValue.ContainsKey(lang))
            {
                res.Reason = LangValue[lang];
            }

            return res;
        }

        public async Task<bool> IsDayOff(DateTime checkDate)
        {
            return await _EngineCoreDBContext.GlobalDayOff.AnyAsync(x => checkDate.Date >= x.StartDate && checkDate.Date <= x.EndDate);
        }

        private void ValidateDayOff(GlobalDayOffPostDto dayOffDto, string lang, int exclude = 0)
        {
            if (dayOffDto.EndDate < dayOffDto.StartDate)
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " for date");
            }

            IQueryable<GlobalDayOff> dayOff = _EngineCoreDBContext.GlobalDayOff.Where(a => a.Id != exclude);

            var violationStartDate = dayOff.Where(a => a.StartDate <= dayOffDto.StartDate && a.EndDate >= dayOffDto.StartDate).FirstOrDefault();
            if (violationStartDate != null)
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " intersection start date from");
            }

            var violationFinishDate = dayOff.Where(a => a.StartDate <= dayOffDto.EndDate && a.EndDate >= dayOffDto.EndDate).FirstOrDefault();
            if (violationFinishDate != null)
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " intersection finish date from");
            }

            var violationBoundaryDate = dayOff.Where(a => a.StartDate > dayOffDto.StartDate && a.StartDate <= dayOffDto.EndDate).FirstOrDefault();
            if (violationBoundaryDate != null)
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " intersection finish date from");
            }

            var violationBoundaryDateF = dayOff.Where(a => a.EndDate > dayOffDto.StartDate && a.EndDate <= dayOffDto.EndDate).FirstOrDefault();
            if (violationBoundaryDateF != null)
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " intersection finish date from");
            }

        }

    }
    }
