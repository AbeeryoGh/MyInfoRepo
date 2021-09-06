using EngineCoreProject.DTOs.ApplicationDtos.IdDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ApplicationInfoUpdate
    {
        public ApplicationWIdDto applicationDto { get; set; }
        public List<TargetApplicationWIdDto> targetApplicationDtos { get; set; }
        public List<ApplicationAttachmentWIdDto> applicationAttachmentDtos { get; set; }
        public List<ApplicationPartyWIdDto> applicationPartyDtos { get; set; }
        public List<ApplicationPartyExtraAttachmentWIdDto> applicationPartyExtraAttachmentWIds { get; set; }
        public TransactionWIdDto transactionDto { get; set; }
        public ApplicationTrackWIdDto  applicationTrackDto { get; set; }
        public List<AppRelatedContentWIdDto> relatedContentDtos { get; set; }
    }
}
