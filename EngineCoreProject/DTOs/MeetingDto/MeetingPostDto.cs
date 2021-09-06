using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using EngineCoreProject.Models;

namespace EngineCoreProject.DTOs.MeetingDto
{
    public class MeetingPostDto
    {
        [Required]
        public string Topic { get; set; }
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string TimeZone { get; set; }

        [Required]
        public string Password { get; set; }
        public bool? PasswordReq { get; set; }

        [Required]
        public int Status { get; set; }

        public string OrderNo { get; set; }

        // [Required]  TODO 
        // public List<int> Participants { get; set; }

        public MeetingPostDto()
        {
           // Participants = new List<int>();
        }
        public Meeting GetEntity()
        {
            Meeting meeting = new Meeting
            {
                Topic = Topic,
                Description = Description,
                StartDate = StartDate,
                EndDate = EndDate,
                TimeZone = TimeZone,
                Password = Password,
                PasswordReq = PasswordReq,
                Status = Status,
                OrderNo = OrderNo
            };

            return meeting;
        }

    }
}
