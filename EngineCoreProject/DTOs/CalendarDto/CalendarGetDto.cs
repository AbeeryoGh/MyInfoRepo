using System;
using EngineCoreProject.Models;

namespace EngineCoreProject.DTOs.CalendarDto
{
    public class CalendarGetDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? NotifyMe { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public int? MeetingId { get; set; }
        public CalendarGetDto()
        {

        }

        public static CalendarGetDto GetDTO(Calendar calendar)
        {
            CalendarGetDto dto = new CalendarGetDto
            {
                Id = calendar.Id,
                UserId = calendar.UserId,
                StartDate = calendar.StartDate,
                EndDate = calendar.EndDate,
                Description = calendar.Description,
                NotifyMe = calendar.NotifyMe,
                Title = calendar.Title,
                MeetingId = calendar.MeetingId
            };

            return dto;
        }
    }
}
