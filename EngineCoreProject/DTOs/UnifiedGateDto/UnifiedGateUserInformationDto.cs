using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.UnifiedGateDto
{



    public class UnifiedGateUserInformationDto
    {
        public UnifiedGateUserInformationDto()
        {
            dtData = new Dtdata[1] { new Dtdata() };
        }

        public Dtdata[] dtData { get; set; }
    }

    public class Dtdata
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserPassword { get; set; }
        public int SecurityQuestionID { get; set; }
        public string SecurityQuestionAnswer { get; set; }
        //public DateTime? BirthdayDate { get; set; }

        public string DOB { get; set; }
        public string Gender { get; set; }
        public int NatID { get; set; }
        public string TelNo { get; set; }
        public string MobileNo { get; set; }
        public int Status { get; set; }
        public string Email_Lang { get; set; }
        public string SMS_Lang { get; set; }
        public int AreaID { get; set; }
        public int NotificationType { get; set; }
        public int ProfileStatus { get; set; }
        public string Address { get; set; }
        public string EmiratesID { get; set; }
    }


}

