
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using EngineCoreProject.DTOs.CalendarDto;
using Microsoft.EntityFrameworkCore;
using EngineCoreProject.IRepository.ICalendarRepository;
using EngineCoreProject.Models;


namespace EngineCoreProject.Services.CalendarService
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;

        public CalendarRepository(EngineCoreDBContext EngineCoreDBContext)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
        }

        public async Task<int> AddCalendar(CalendarPostDto calendarPostDto, string lang)
        {
            ValidateCalendar(calendarPostDto, lang);
           // using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
            Calendar newCalendar = calendarPostDto.GetEntity();
            _EngineCoreDBContext.Calendar.Add(newCalendar);
            await  _EngineCoreDBContext.SaveChangesAsync();
           // await transaction.CommitAsync();
            return newCalendar.Id;
        }

        public async Task<int> UpdateCalendar(int rowId, CalendarPostDto calendarPostDto, string lang)
        {
            ValidateCalendar(calendarPostDto, lang);
            Calendar originalCalendar = await _EngineCoreDBContext.Calendar.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (originalCalendar == null)
            {
                return 0;
            }

            originalCalendar.Title = calendarPostDto.Title;
            originalCalendar.NotifyMe = calendarPostDto.NotifyMe;
            originalCalendar.Description = calendarPostDto.Description;
            originalCalendar.StartDate = calendarPostDto.StartDate;
            originalCalendar.EndDate = calendarPostDto.EndDate;
            originalCalendar.UserId = calendarPostDto.UserId;
            originalCalendar.MeetingId = calendarPostDto.MeetingId;
            _EngineCoreDBContext.Calendar.Update(originalCalendar);
            await _EngineCoreDBContext.SaveChangesAsync();

            return originalCalendar.Id;
        }

        public async Task<int> DeleteCalendar(int id)
        {
            int res = 0;
            Calendar calendar = await _EngineCoreDBContext.Calendar.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (calendar == null)
            {
                return res;
            }

            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
            _EngineCoreDBContext.Calendar.Remove(calendar);
            _EngineCoreDBContext.SaveChanges();
            transaction.Commit();
            res = calendar.Id;

            return res;
        }

        public async Task<List<CalendarGetDto>> GetCalendar(int userId)
        {
            List<CalendarGetDto> res = new List<CalendarGetDto>();
            var cal = await _EngineCoreDBContext.Calendar.Where(d => d.UserId == userId).ToListAsync();
            foreach (var calendar in cal)
            {
                res.Add(CalendarGetDto.GetDTO(calendar));
            }
            return res;
        }

        public async Task<List<CalendarGetDto>> GetCalendarByMeetingId(int MeetingId)
        {
            List<CalendarGetDto> res = new List<CalendarGetDto>();
            var calendars = await _EngineCoreDBContext.Calendar.Where(d => d.MeetingId == MeetingId).ToListAsync();
            foreach (var calendar in calendars)
            {
                res.Add(CalendarGetDto.GetDTO(calendar));
            }
            return res;
        }

        private void ValidateCalendar(CalendarPostDto calendarDto, string lang)
        {
            if (calendarDto.EndDate < calendarDto.StartDate)
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " for end date.");
            }

            if (calendarDto.MeetingId != null)
            {
                var meet = _EngineCoreDBContext.Meeting.Where(x => x.Id == calendarDto.MeetingId).SingleOrDefault();
                if (meet == null)
                {
                    throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " " + calendarDto.MeetingId);
                }

                if (meet.StartDate != calendarDto.StartDate)
                {
                    throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " " + calendarDto.StartDate);
                }

                if (meet.EndDate != calendarDto.EndDate)
                {
                    throw new InvalidOperationException(Constants.getMessage(lang, "wrongParameter") + " " + calendarDto.EndDate);
                }
            }
        }
    }
}
