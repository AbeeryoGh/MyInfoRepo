using EngineCoreProject.DTOs.ApplicationDtos.IdDtos;
using System.Collections.Generic;


namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class ApplicationInfo
    {
        public ApplicationWIdDto applicationDto { get; set; }//  it was ApplicationDto

        public List<TargetApplicationDto> targetApplicationDtos { get; set; }
       // public List<int> targetApplication { get; set; }
        public List<ApplicationAttachmentDto> applicationAttachmentDtos { get; set; }
        public List<ApplicationPartyWithExtraDto> applicationPartyDtos { get; set; }
        public TransactionDto transactionDto { get; set; }
        public ApplicationTrackDto applicationTrackDto { get; set; }
    }
}
