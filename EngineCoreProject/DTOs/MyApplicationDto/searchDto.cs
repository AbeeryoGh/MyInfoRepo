using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.MyApplicationDto
{
    public class searchDto
    {
        public string TransactionID { get; set;}
        public bool onlyMyApps { get; set; }
        public int AppStateId { get; set; }
        public string serviceNo { get; set; }
        public bool isLate { get; set; }
        public string stagetypeid { get; set; }
        public string Id { get; set;}
        public string Name { get; set;}
        public string Phone { get; set;}
        public string Email { get; set;}
        public string EmirateId { get; set; }
        public string sDate { get; set; }
        public string eDate { get; set; }

    }
}
