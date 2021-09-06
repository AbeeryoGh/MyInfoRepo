using EngineCoreProject.DTOs.RoleDto;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AccountDto
{
    public class UserDto
    {
        public virtual string PhoneNumber { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string Email { get; set; }
        public virtual string UserName { get; set; }
        public virtual int Id { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthdayDate { get; set; }
        public string Gender { get; set; }
        public int? NatId { get; set; }
        public string NationalityName { get; set; }
        public string TelNo { get; set; }
        public string EmailLang { get; set; }
        public string SmsLang { get; set; }
        public int? AreaId { get; set; }
        public int ProfileStatus { get; set; }
        public string Address { get; set; }
        public string EmiratesId { get; set; }
        public List<RoleGetDto> RolesName { get; set; }
        public string Image { get; set; }
        public string Sign { get; set; }

        public string Emarit { get; set; }
        public int Location { get; set; }
        public string LocationName { get; set; }

    }
}
