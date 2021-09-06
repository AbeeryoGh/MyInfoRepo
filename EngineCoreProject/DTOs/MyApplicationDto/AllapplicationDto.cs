using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.MyApplicationDto
{
    public class AllapplicationDto
    {
        public DateTime LastDateReader { get; set; }
        public string applicationNo { get; set; }
        public int isOnlineStatus { get; set; }
        public string UserName { get; set; }
        public int percent { get; set; }
        public int periodLate { get; set; }
        public DateTime lastUpdateStage { get; set; }
        public string appStatusName { get; set; }
        public int? appStatusId { get; set; }
        public int? appid { get; set; }
        public int? stagetypeid { get; set; }
        public int? serviceid { get; set; }
        public string servicename { get; set; }
        public int? fee { get; set; }
        public DateTime? appstartdate { get; set; }
        public int? templateid { get; set; }
        public string templatename { get; set; }
        public int?doctypeid { get; set; }
        public string documentname { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string stagetypename { get; set; }
    }
}
