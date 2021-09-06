using EngineCoreProject.DTOs.GeneralSettingDto;
using EngineCoreProject.DTOs;
using EngineCoreProject.IRepository.IGeneralSetting;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EngineCoreProject.Services.GeneralSetting
{
    public class WorkingTimeRepository : IWorkingTimeRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IGlobalDayOffRepository _iGlobalDayOffRepository;
        private readonly Dictionary<DateTime, int> WorkingMinutesDic;
        ValidatorException _exception;

        public WorkingTimeRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository, IGlobalDayOffRepository iGlobalDayOffRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _iGlobalDayOffRepository = iGlobalDayOffRepository;
            WorkingMinutesDic = new Dictionary<DateTime, int>();
            _exception = new ValidatorException();
        }

        public async Task InitialaizeWorkingDic(DateTime minDate, DateTime maxDate)
        {
            WorkingMinutesDic.Clear();
            var daysOffRows =  await _EngineCoreDBContext.GlobalDayOff.ToListAsync();

            List<DateTime> daysOff = new List<DateTime>();
            foreach (var row in daysOffRows)
            {
                for (var date = row.StartDate; date <= row.EndDate; date = date.AddDays(1))
                {
                    daysOff.Add(date);
                }
            }

            daysOff.Sort();
            int removednum = daysOff.RemoveAll(x => x > maxDate || x < minDate);

            List<List<WorkingHours>> WorkingHoursForWeek = new List<List<WorkingHours>>();
            for (var i = 0; i < 7; i++)
            {
                WorkingHoursForWeek.Add(await _EngineCoreDBContext.WorkingHours.Where(x => x.DayOfWeek == i).ToListAsync());
            }

            for (var date = minDate; date <= maxDate; date = date.AddDays(1))
            {
                if (daysOff.Contains(date))
                {
                    continue;
                }

                var limitedDateResult = WorkingHoursForWeek[(int)date.DayOfWeek].Where(x => x.StartDate.HasValue && date.Date >= x.StartDate && date.Date <= x.FinishDate);
                var oriResult = (limitedDateResult.Count() > 0) ? limitedDateResult.ToList() : WorkingHoursForWeek[(int)date.DayOfWeek].Where(x => x.StartDate == null).ToList();

                int res = 0;

                res += oriResult.Sum(w => w.FinishAt - w.StartFrom);

                if (res > 0)
                {
                    WorkingMinutesDic.Add(date.Date, res);
                }
            }
        }

        public async Task<int> AddWorkingTime(WorkingTimePostDto workingTimeDto, string lang)
        {  
            ValidateWorkingHours(workingTimeDto, lang);

            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
            WorkingHours workTime = workingTimeDto.GetEntity();
            workTime.WorkingTimeNameShortcut = _iGeneralRepository.GenerateShortCut(Constants.WORKING_HOURS, Constants.WORKING_TIME_NAME_SHORTCUT);
            _EngineCoreDBContext.WorkingHours.Add(workTime);
            _EngineCoreDBContext.SaveChanges();

            await _iGeneralRepository.InsertUpdateSingleTranslation(workTime.WorkingTimeNameShortcut, workingTimeDto.NameShortCutLangValue);

            await transaction.CommitAsync();
            return workTime.Id;
        }

        public async Task<int> UpdateWorkingTime(WorkingTimePostDto workingDto, int rowId, string lang)
        {
            int res = 0;
            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
            WorkingHours originalWorkingTime = await _EngineCoreDBContext.WorkingHours.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (originalWorkingTime == null)
            {
                return res;
            }

            ValidateWorkingHours(workingDto, lang, rowId);

            originalWorkingTime.DayOfWeek = workingDto.DayOfWeek;
            originalWorkingTime.StartFrom = workingDto.StartFrom;
            originalWorkingTime.FinishAt = workingDto.FinishAt;
            originalWorkingTime.StartDate = workingDto.StartDateFrom;
            originalWorkingTime.FinishDate = workingDto.FinishDateAt;

            _EngineCoreDBContext.WorkingHours.Update(originalWorkingTime);
            _EngineCoreDBContext.SaveChanges();

            await _iGeneralRepository.InsertUpdateSingleTranslation(originalWorkingTime.WorkingTimeNameShortcut, workingDto.NameShortCutLangValue);
            transaction.Commit();

            res = originalWorkingTime.Id;
            return res;
        }

        public async Task<int> DeleteWorkingTime(int id, string lang)
        {
            int res = 0;
            WorkingHours workingHour = await _EngineCoreDBContext.WorkingHours.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (workingHour == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "missedParameter"));
                throw _exception;
            }

            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();

            await _iGeneralRepository.DeleteTranslation(workingHour.WorkingTimeNameShortcut);
            _EngineCoreDBContext.WorkingHours.Remove(workingHour);
            _EngineCoreDBContext.SaveChanges();
            transaction.Commit();
            res = workingHour.Id;
            return res;
        }

        public async Task<List<WorkingTimeGetDto>> GetWorkingHours(string lang)
        {
            var workingHoursRows = await _EngineCoreDBContext.WorkingHours.ToListAsync();

            List<WorkingTimeGetDto> result = new List<WorkingTimeGetDto>();
            foreach (var row in workingHoursRows)
            {
                var workTime = WorkingTimeGetDto.GetDTO(row);
                var LangValue = await _iGeneralRepository.getTranslationsForShortCut(row.WorkingTimeNameShortcut);
                if (LangValue.ContainsKey(lang))
                {
                    workTime.Workingday = LangValue;
                    workTime.NameShortCut = LangValue[lang];
                }
                result.Add(workTime);
            }
            return result;
        }

        public async Task<WorkingTimeGetDto> GetWorkTimeId(int rowId, string lang)
        {
            WorkingHours workHourRec = new WorkingHours();
            workHourRec = await _EngineCoreDBContext.WorkingHours.Where(d => d.Id == rowId).FirstOrDefaultAsync();
            WorkingTimeGetDto res = new WorkingTimeGetDto();
            if (workHourRec == null)
            {
                return res;
            }
            res = WorkingTimeGetDto.GetDTO(workHourRec);
            var LangValue = await _iGeneralRepository.getTranslationsForShortCut(workHourRec.WorkingTimeNameShortcut);
            if (LangValue.ContainsKey(lang))
            {
                res.NameShortCut = LangValue[lang];
            }
            return res;
        }

        public async Task<List<WorkingTimeGetDto>> GetWorkingForDate(DateTime date)
        {
            List<WorkingTimeGetDto> result = new List<WorkingTimeGetDto>();
            if (await _iGlobalDayOffRepository.IsDayOff(date))
            {
                return result;
            }
            List<WorkingHours> oriResult = new List<WorkingHours>();
            var workingHoursRows = await _EngineCoreDBContext.WorkingHours.Where(x => x.DayOfWeek == (int)date.DayOfWeek).ToListAsync();
            var limitedDateResult = workingHoursRows.Where(x => x.StartDate.HasValue && date.Date >= x.StartDate && date.Date <= x.FinishDate);
            oriResult = (limitedDateResult.Count() > 0) ? limitedDateResult.ToList() : workingHoursRows.Where(x => x.StartDate == null).ToList();

            foreach (var row in oriResult)
            {
                result.Add(WorkingTimeGetDto.GetDTO(row));
            }
            return result;
        }

        public async Task<int> GetWorkingMinutesBetweenTwoDates(DateTime sDateTime, DateTime eDateTime)
        {
            int res = 0;
            if (eDateTime <= sDateTime)
            {
                return res;
            }

            if (sDateTime.Date == eDateTime.Date)
            {
                if (!await _iGlobalDayOffRepository.IsDayOff(sDateTime.Date))
                {
                    int sTime = sDateTime.Hour * 60 + sDateTime.Minute;
                    int eTime = eDateTime.Hour * 60 + eDateTime.Minute;
                    var workingHoursRows = await _EngineCoreDBContext.WorkingHours.Where(x => x.DayOfWeek == (int)sDateTime.DayOfWeek && x.StartFrom >= sTime).ToListAsync();
                    var limitedDateResult = workingHoursRows.Where(x => x.StartDate.HasValue && sDateTime.Date >= x.StartDate && sDateTime.Date <= x.FinishDate);
                    var oriResult = (limitedDateResult.Count() > 0) ? limitedDateResult.ToList() : workingHoursRows.Where(x => x.StartDate == null).ToList();
                    foreach (var w in oriResult)
                    {
                        res += (eTime >= w.FinishAt) ? w.FinishAt - w.StartFrom : w.FinishAt - eTime;
                    }
                    return res;
                }
            }

            if (!await _iGlobalDayOffRepository.IsDayOff(sDateTime.Date))
            {
                int sTime = sDateTime.Hour * 60 + sDateTime.Minute;
                var workingHoursRows = await _EngineCoreDBContext.WorkingHours.Where(x => x.DayOfWeek == (int)sDateTime.DayOfWeek && x.FinishAt > sTime).ToListAsync();
                var limitedDateResult = workingHoursRows.Where(x => x.StartDate.HasValue && sDateTime.Date >= x.StartDate && sDateTime.Date <= x.FinishDate);
                var oriResult = (limitedDateResult.Count() > 0) ? limitedDateResult.ToList() : workingHoursRows.Where(x => x.StartDate == null).ToList();
                foreach (var w in oriResult)
                {
                    res += (sTime <= w.StartFrom) ? w.FinishAt - w.StartFrom : w.FinishAt - sTime;
                }
            }

            if (!await _iGlobalDayOffRepository.IsDayOff(eDateTime.Date))
            {
                int eTime = eDateTime.Hour * 60 + eDateTime.Minute;
                var workingHoursRows = await _EngineCoreDBContext.WorkingHours.Where(x => x.DayOfWeek == (int)eDateTime.DayOfWeek && x.StartFrom < eTime).ToListAsync();
                var limitedDateResult = workingHoursRows.Where(x => x.StartDate.HasValue && eDateTime.Date >= x.StartDate && eDateTime.Date <= x.FinishDate);
                var oriResult = (limitedDateResult.Count() > 0) ? limitedDateResult.ToList() : workingHoursRows.Where(x => x.StartDate == null).ToList();
                foreach (var w in oriResult)
                {
                    res += (eTime >= w.FinishAt) ? w.FinishAt - w.StartFrom : eTime - w.StartFrom;
                }
            }

            res += WorkingMinutesDic.Where( x=> x.Key > sDateTime.Date && x.Key < eDateTime.Date).Sum(x => x.Value);
            return res;
        }

        public async Task<Dictionary<DateTime, int>> GetWorkingDates(DateTime untilDate) 
        {
            await InitialaizeWorkingDic(DateTime.Now.Date, untilDate);
            return WorkingMinutesDic;
        }

        public async Task<Dictionary<int,DateTime>> GetDeadline(List<int> hours)
        {

            Dictionary<int, DateTime> tempResult = new Dictionary<int, DateTime>();

            Dictionary<int, DateTime> result = new Dictionary<int, DateTime>();

            int maxMinutes = hours.Max()*60;

            List<WorkingHoursDates> workHours = new List<WorkingHoursDates>();

            // get hours for this day.
            DateTime today = DateTime.Now;
            var workingList = await GetWorkingForDate(today);
            var timeNowInMinutes = DateTime.Now.Minute;
            var previousMinutesWork = 0;
            foreach (var row in workingList)
            {
                if (timeNowInMinutes >= row.FinishAt)
                {
                    previousMinutesWork += row.FinishAt - row.StartFrom;
                }
                else if (timeNowInMinutes > row.StartFrom)
                {
                    previousMinutesWork += timeNowInMinutes - row.StartFrom;
                }
            }

            if (previousMinutesWork > 0)
            {
                WorkingHoursDates workingHoursDates = new WorkingHoursDates
                {
                    Date = DateTime.Now,
                    Minutes = previousMinutesWork,
                    WorkingTimes = workingList
                };
                workHours.Add(workingHoursDates);
            }

            while (previousMinutesWork < maxMinutes)
            {
                today = today.AddDays(-1);
                var workingRows = await GetWorkingForDate(today);
                if (workingRows != null)
                {
                    var minutes = workingRows.Sum(x => x.FinishAt - x.StartFrom);
                    if (minutes > 0)
                    {
                        WorkingHoursDates workingHoursDates = new WorkingHoursDates
                        {
                            Date = today.Date,
                            Minutes = minutes,
                            WorkingTimes = workingRows
                        };
                        workHours.Add(workingHoursDates);
                    }

                    previousMinutesWork += minutes;
                }
            }

            foreach( var hour in hours)
            {
                if (tempResult.ContainsKey(hour))
                {
                    result.Add(hour, tempResult[hour]);
                }
                else
                {
                    int curMin = 0;
                    foreach (var work in workHours)
                    {
                        if (curMin + work.Minutes >= hour*60)
                        {
                            var remain = (curMin + work.Minutes) - (hour * 60);
                            for (int i = work.WorkingTimes.Count -1; i > -1 ; i--)
                            {
                                if (remain <= work.WorkingTimes[i].FinishAt - work.WorkingTimes[i].StartFrom)
                                {
                                    var time = work.WorkingTimes[i].FinishAt - remain;
                                    result.Add(hour,work.Date.AddMinutes(time));

                                    tempResult.Add(hour, work.Date.AddMinutes(time));
                                    break;
                                }
                                else
                                {
                                    remain -= work.WorkingTimes[i].FinishAt - work.WorkingTimes[i].StartFrom;
                                }
                            }

                            break;
                        }
                        else
                        {
                            curMin += work.Minutes;
                        }
                    }
                }
            }

            return result;
        }

        private void ValidateWorkingHours(WorkingTimePostDto workingTimeDto, string lang, int exclude = 0)
        {

            IQueryable<WorkingHours> dayOfWeekWork = _EngineCoreDBContext.WorkingHours.Where(a => a.DayOfWeek == workingTimeDto.DayOfWeek && a.Id != exclude);

            // validate if the working hours are intersected.
            if (workingTimeDto.StartFrom < 0 || workingTimeDto.StartFrom > 1440 || workingTimeDto.FinishAt < 0 || workingTimeDto.FinishAt > 1440)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " for time");
            }

            if (workingTimeDto.StartFrom >= workingTimeDto.FinishAt)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " for time");
            }

            if (workingTimeDto.FinishDateAt != null && workingTimeDto.StartDateFrom == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "missedParameter") + " start date from");
            }

            if (workingTimeDto.FinishDateAt == null && workingTimeDto.StartDateFrom != null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "missedParameter") + " finish date from");
            }

            if (workingTimeDto.DayOfWeek < 0 || workingTimeDto.DayOfWeek > 6)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " for DayOfWeek");
            }

            if (workingTimeDto.FinishAt <= workingTimeDto.StartFrom)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " finish time from");
            }


            if (workingTimeDto.FinishDateAt != null && workingTimeDto.StartDateFrom != null)
            {
                if (workingTimeDto.FinishDateAt <= workingTimeDto.StartDateFrom)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " finish date from");
                }

                var violationStartDate = dayOfWeekWork.Where(a => a.StartDate != null).Where(a => a.StartDate <= workingTimeDto.StartDateFrom && a.FinishDate >= workingTimeDto.StartDateFrom).FirstOrDefault();
                if (violationStartDate != null)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " intersection start date from");
                }

                var violationFinishDate = dayOfWeekWork.Where(a => a.StartDate != null).Where(a => a.StartDate <= workingTimeDto.FinishDateAt && a.FinishDate >= workingTimeDto.FinishDateAt).FirstOrDefault();
                if (violationFinishDate != null)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " intersection finish date from");
                }

                var violationBoundaryDate = dayOfWeekWork.Where(a => a.StartDate != null).Where(a => a.StartDate > workingTimeDto.StartDateFrom && a.StartDate <= workingTimeDto.FinishDateAt).FirstOrDefault();
                if (violationBoundaryDate != null)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " intersection finish date from");
                }

                var violationBoundaryDateF = dayOfWeekWork.Where(a => a.StartDate != null).Where(a => a.FinishDate > workingTimeDto.StartDateFrom && a.FinishDate <= workingTimeDto.FinishDateAt).FirstOrDefault();
                if (violationBoundaryDateF != null)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " intersection finish date from");
                }
            }
            else
            {
                var violationStartTime = dayOfWeekWork.Where(a => a.StartDate == null).Where(a => a.StartFrom <= workingTimeDto.StartFrom && a.FinishAt >= workingTimeDto.FinishAt).FirstOrDefault();
                if (violationStartTime != null)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " intersection start time from");
                }

                var violationFinishTime = dayOfWeekWork.Where(a => a.StartDate == null).Where(a => a.StartFrom <= workingTimeDto.FinishAt && a.FinishAt >= workingTimeDto.FinishAt).FirstOrDefault();
                if (violationFinishTime != null)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " intersection finish time from");
                }

                var violationBoundaryTime = dayOfWeekWork.Where(a => a.StartDate == null).Where(a => a.StartFrom > workingTimeDto.StartFrom && a.StartFrom <= workingTimeDto.FinishAt).FirstOrDefault();
                if (violationBoundaryTime != null)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " intersection finish time from");
                }

                var violationBoundaryTimeF = dayOfWeekWork.Where(a => a.StartDate == null).Where(a => a.FinishAt > workingTimeDto.StartFrom && a.FinishAt <= workingTimeDto.FinishAt).FirstOrDefault();
                if (violationBoundaryTimeF != null)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongParameter") + " intersection finish date from");
                }


                if (_exception.AttributeMessages.Count() > 0)
                {
                    throw _exception;
                }
            }
        }

        public async Task<bool> IsOrderLate(DateTime sDateTime, DateTime eDateTime, int timePeriodInMinutes)
        {
            return await GetWorkingMinutesBetweenTwoDates(sDateTime, eDateTime) > timePeriodInMinutes;
        }

        public async Task<bool> IsInsideWorkingHours(DateTime checkDateTime)
        {
            var workingPeriods = await GetWorkingForDate(checkDateTime.Date);
            if (workingPeriods != null)
            {
                var timeInMinutes = checkDateTime.TimeOfDay.TotalMinutes;
                foreach (var workingPeriod in workingPeriods)
                {
                   if (timeInMinutes >= workingPeriod.StartFrom && timeInMinutes <= workingPeriod.FinishAt)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
