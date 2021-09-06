using System;
using System.ComponentModel.DataAnnotations;
using EngineCoreProject.Models;

namespace EngineCoreProject.DTOs.CalendarDto
{
    public class CalendarPostDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public int? NotifyMe { get; set; }
        public string Description { get; set; }
        [Required]
        public string Title { get; set; }
        public int? MeetingId { get; set; }

       public CalendarPostDto()
        {

        }
        public Calendar GetEntity()
        {
            Calendar queueProcess = new Calendar
            {
                Title = Title,
                Description = Description,
                StartDate = StartDate,
                EndDate = EndDate,
                UserId = UserId,
                NotifyMe = NotifyMe,
                MeetingId = MeetingId
            };

            return queueProcess;
        }

    }
}
