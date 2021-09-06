using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Credential
{
    public class RequestCredentialsResDto
    {
        public claim claim { get; set; }
        public evidence evidence { get; set; }
        public keyHash keyHash { get; set; }
        public string credentialExpiryDate { get; set; }
    }
}

public class claim
{
    public string appNo { get; set; }
    public string appNoLabelAr { get; set; }
    public string appNoLabelEN { get; set; }
    public string contractNo { get; set; }
    public string contractNoLabelAr { get; set; }
    public string contractNoLabelEn { get; set; }
    public string contractDateLabelAr { get; set; }
    public string contractDateLabelEn { get; set; }
    public string contractDate { get; set; }
    public string contractStartDate { get; set; }
    public string contractStartDateLabelAr { get; set; }
    public string contractStartDateLabelEn { get; set; }
    public string contractEndtDate { get; set; }
    public string contractEndtDateLabelAr { get; set; }
    public string contractEndDateLabelEn { get; set; }
    public string templateNameAr { get; set; }
    public string templateNameEn { get; set; }
    public string templateNameLabelAr { get; set; }
    public string templateNameLabelEn { get; set; }
    public string fee { get; set; }
    public string feeLabelAr { get; set; }
    public string feeLabelEN { get; set; }
    public string invoiceNo { get;set; }
    public string invoiceNoLabelAr { get; set; }
    public string invoiceNoLabelEn { get; set; }
    public string invoiceDate { get; set; }
    public string invoiceDateLabelAr { get; set; }
    public string invoiceDateLabelEn { get; set; }
    public List<CredentialsParty> parties { get; set; }

}

public class keyHash
{
    public string appNo { get; set; }
    public string contractNo { get; set; }
    public string contractDate { get; set; }
    public string contractStartDate { get; set; }
    public string contractEndtDate { get; set; }
    public string templateNameAr { get; set; }
    public string templateNameEn { get; set; }
    public string fee { get; set; }
    public string invoiceNo { get; set; }
    public string invoiceDate { get; set; }
    // public List<CredentialsParty> credentialsParties { get; set; }
}

public class evidence
{
    public string file { get; set; }
    public string type { get; set; }
}

public class CredentialsParty
{
    public string partyName { get; set; }
    public string partyNameLabelAr { get; set; }
    public string partyNameLabelEn { get; set; }
    public string nationalityAr { get; set; }
    public string nationalityEn { get; set; }
    public string nationalityLabelAr { get; set; }
    public string nationalityLabelEn { get; set; }
    public string partyTypeAr { get; set; }
    public string partyTypeEn { get; set; }
    public string partyTypeLabelAr { get; set; }
    public string partyTypeLabelEn { get; set; }
    public string idNumber { get; set; }
    public string idNumberLabelEn { get; set; }
    public string idNumberLabelAr { get; set; }
    public string idDocTypeAr { get; set; }
    public string idDocTypeEn { get; set; }
    public string idDocTypeLabelAr { get; set; }
    public string idDocTypeLabelEn { get; set; }
}