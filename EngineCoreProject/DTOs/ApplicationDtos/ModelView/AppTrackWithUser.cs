using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos.ModelView
{
    public class AppTrackWithUser //-------------Used in Preview stage to show track with user name
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int ApplicationId { get; set; }
        public int UserId { get; set; }
        public int? StageId { get; set; }
        public int? NextStageId { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }

   


    }
}
