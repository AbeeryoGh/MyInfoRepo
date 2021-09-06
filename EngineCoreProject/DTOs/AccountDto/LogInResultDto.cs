using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AccountDto
{
    public class LogInResultDto
    {       
        public string StatusCode { get; set; }
        public string token { get; set; }
        public int UserId { get; set; }
        public int? GenderId { get; set; }
        public int? ServiceId { get; set; }
        public int? ApplicationId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? BirthdayDate { get; set; }
        public string EmirateId{ get; set; }
        public List<int> RolesId { get; set; }
        public IList<string> RolesName { get; set; }

        public string Address { get; set; }
        public int? CountryId { get; set; }

        public int? AreaId { get; set; }

        public DateTime? LastLogin { get; set; }
    }
}
