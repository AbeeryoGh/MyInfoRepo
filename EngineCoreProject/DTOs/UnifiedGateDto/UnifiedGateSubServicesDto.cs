using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.UnifiedGateDto
{
    public class UnifiedGateSubServicesDto
    {
        public Subservice[] SubServices { get; set; }
    }



    public class Subservice
    {
        public int MainServiceCode { get; set; }
        public int SubServiceCode { get; set; }
        public object PortalID { get; set; }
        public string CustomerAr { get; set; }
        public string CustomerEn { get; set; }
        public string DepartmentAr { get; set; }
        public string DepartmentEn { get; set; }
        public string FullSequenceCode { get; set; }
        public bool HasDependency { get; set; }
        public bool PriorityService { get; set; }
        public object DigitisationLevel { get; set; }
        public object NoApprovals { get; set; }
        public object NoOfTransactions { get; set; }
        public int? NoOfProcessTasks { get; set; }
        public object NoOfRequiredDocuments { get; set; }
        public object NoOfVisitsRequired { get; set; }
        public int? Service_RequirementLevelID { get; set; }
        public object RelSCList { get; set; }
        public string SectorAr { get; set; }
        public string SectorEn { get; set; }
        public string ServiceClassification { get; set; }
        public string ServiceClassificationAr { get; set; }
        public string ServiceClassificationEn { get; set; }
        public string ServiceDescriptionAr { get; set; }
        public string ServiceDescriptionEn { get; set; }
        public string ServiceRequirementAr { get; set; }
        public string ServiceRequirementEn { get; set; }
        public int ServiceSectorID { get; set; }
        public string ServiceStandardAr { get; set; }
        public string ServiceStandardEn { get; set; }
        public string ServiceStepAr { get; set; }
        public string ServiceStepEn { get; set; }
        public object ServiceTypeAr { get; set; }
        public string ServiceTypeEn { get; set; }
        public object ServiceURL { get; set; }
        public object ShortServiceNameAr { get; set; }
        public object ShortServiceNameEn { get; set; }
        public int StatusID { get; set; }
        public int SubSequenceID { get; set; }
        public string SubServiceNameAr { get; set; }
        public string SubServiceNameEn { get; set; }
        public string SubServiceManualAr { get; set; }
        public string SubServiceManualEn { get; set; }
        public string SubServiceTypeAr { get; set; }
        public string SubServiceTypeEn { get; set; }
        public int SubServiceTypeID { get; set; }
        public int Active { get; set; }
    }

}




