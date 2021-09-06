using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.MyApplicationDto
{
    public class ApplicationCountDto
    {
        public int autoCancelledStageId { get; set; }
        public int autoCancelledCount { get; set; }
        public int RejectedStageTypeId { get; set; }
        public int AutoCancelledId { get; set; }
        public int rejectedCount { get; set; }
        public int islateReview { get; set; }
        public int reviewCount { get; set; }
        public int StageTypeId { get; set; }
        public int returnedCount { get; set; }
        public int count { get; set; }
        public List<Apps> Applications { get; set; }
        public List<ApplicationByStage> AppBySatge { get; set; }
    }
}


public class Apps
{
    public int isOnlineStatus { get; set; }
    public string UserName { get; set; }
    public int CurrentStageId { get; set; }
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
    public int? doctypeid { get; set; }
    public string documentname { get; set; }
    public string fullname { get; set; }
    public string email { get; set; }
    public string mobile { get; set; }
    public string stagetypename { get; set; }
    public bool islate { get; set; }
    public bool islocked { get; set; }
}

public class ApplicationByStage
{
    public int StageTypeId { get; set; }
    public string StageTypeName { get; set; }
    public int Count { get; set; }
    public List<Apps> Appstage { get; set; }

}