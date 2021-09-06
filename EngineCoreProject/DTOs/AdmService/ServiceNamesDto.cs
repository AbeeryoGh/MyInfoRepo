using System;

namespace EngineCoreProject.DTOs.AdmService
{
    public class ServiceNamesDto
    {
        public int? Id { get; set; }
        public string shortcut { get; set; }
        public string serviceName { get; set; }
        public int? ServiceKind { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastupdatedDate { get; set; }
        public int? fee { get; set; }
        public string recordStatus { get; set; }
        public int? UgId { get; set; }
        public string Icon { get; set; }
        public int? orderNo { get; set; }
        public int? TargetService { get; set; }

        public bool? Templated { get; set; }
        public int?  DefaultUser { get; set; }
        public bool? HasReason { get; set; }

        public bool? HasDocument { get; set; }
        public int?  ApprovalStage { get; set; }

        public int? TemplateId{ get; set; }
        public int? DocumentTypeId { get; set; }
        public bool? Delivery { get; set; }
        public bool? ShowApplication { get; set; }
        public bool? ShowTransaction { get; set; }
        public bool? LimitedTime { get; set; }
        public short? ExternalFileRequired { get; set; }
        public string GuidFilePathAr { get; set; }
        public string GuidFilePathEn { get; set; }
    }
}
