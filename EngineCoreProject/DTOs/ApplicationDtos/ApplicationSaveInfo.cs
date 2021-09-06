using EngineCoreProject.DTOs.ApplicationDtos.RelatedContent;
using EngineCoreProject.DTOs.TransactionFeeDto;
using System.Collections.Generic;


namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ApplicationSaveInfo : ApplicationInfo
    {
        public List<ApplicationPartyExtraAttachmentDto> applicationPartyExtraAttachmentWIds { get; set; }
        public List<TransactionFeeOutput> calculatedFeesDto { get; set; }
        public List<AppRelatedContentDto>    relatedContentDtos { get; set; }
    }
}
