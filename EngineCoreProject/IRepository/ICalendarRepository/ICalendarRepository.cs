using EngineCoreProject.DTOs.CalendarDto;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace EngineCoreProject.IRepository.ICalendarRepository
{
    public interface ICalendarRepository
    {
        Task<int> AddCalendar(CalendarPostDto calendarPostDto, string lang);

        Task<int> UpdateCalendar(int rowId, CalendarPostDto calendarPostDto, string lang);

        Task<int> DeleteCalendar(int id);

        Task <List<CalendarGetDto>> GetCalendar(int userId);
        Task<List<CalendarGetDto>> GetCalendarByMeetingId(int MeetingId);

    }
}
