using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class AppObjectionDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }

        public string Nationality { get; set; }

        public string Gender { get; set; }

        public string EmiratesId { get; set; }
        public string Address { get; set; }

        public int EmaraId { get; set; }
        public string City { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }

        public List<string> Attachments { get; set; }
    }



    public class ApplicationObjectionDTO
    {
        public int ApplicationId { get; set; }
        public string Reason { get; set; }
    }
}
