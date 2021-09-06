using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.UnifiedGateDto
{
    public class UnifiedGateLoginDto
    {

        //[Required]
        //public string StatusCode { set; get; }
        //[Required]
        //public string token { set; get; }
        //[Required]
        //public int? UserId { set; get; }
        //[Required]
        //public int? RoleId { set; get; }
        //[Required]
        //public string RoleEnName { set; get; }

        //[Required]
        //public string UserName { set; get; }
        //[Required]
        //public string FullName { set; get; }
        //[Required]
        //public string Email { set; get; }
        //[Required]
        //public string serviceId { set; get; }
        //[Required]
        //public string lang { set; get; }
        //[Required]
        //public string theme { set; get; }
        ////////////////add
        //[Required]
        //public string DateBirthday { set; get; }
        //[Required]
        //public string Gender { set; get; }
        //[Required]
        //public int? NatID { set; get; }
        //[Required]
        //public string TelNo { set; get; }
        //[Required]
        //public string MobileNo { set; get; }
        //[Required]
        //public int? status { set; get; }
        //[Required]
        //public string EmailLang { set; get; }
        //[Required]
        //public string SMSLang { set; get; }
        //[Required]
        //public int? AreaId { set; get; }
        //[Required]
        //public int? NotificationType { set; get; }
        //[Required]
        //public int? ProfileStatus { set; get; }
        //[Required]
        //public string Address { set; get; }
        //[Required]
        //public string EmiratesID { set; get; }
        //[Required]
        //public string ImageURL { set; get; }

        public string StatusCode { get; set; }
        public string token { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleEnName { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
        public string EmirateId { get; set; }

    }
}
