using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs
{

     //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Xml
    {
        [JsonProperty("@version")]
        public string Version { get; set; }
        [JsonProperty("@encoding")]
        public string Encoding { get; set; }
    }

    public class SpGetSMSSendListStatusBySourceSystemIdResult
    {
        public string MessageID { get; set; }
        public string PhoneNo { get; set; }
    }

    public class Records
    {
        public SpGetSMSSendListStatusBySourceSystemIdResult sp_getSMSSendListStatusBySourceSystemId_Result { get; set; }
    }

    public class SendSMSListResult
    {
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
        public string SuccessMessageCnt { get; set; }
        public string FailedMessageCnt { get; set; }
        public string MessageRequestId { get; set; }
        public Records Records { get; set; }
    }

    public class SendSMSListResponse
    {
        [JsonProperty("@xmlns")]
        public string Xmlns { get; set; }
        public SendSMSListResult sendSMSListResult { get; set; }
    }

    public class SoapBody
    {
        public SendSMSListResponse sendSMSListResponse { get; set; }
    }

    public class SoapEnvelope
    {
        [JsonProperty("@xmlns:soap")]
        public string XmlnsSoap { get; set; }
        [JsonProperty("@xmlns:xsi")]
        public string XmlnsXsi { get; set; }
        [JsonProperty("@xmlns:xsd")]
        public string XmlnsXsd { get; set; }
        [JsonProperty("soap:Body")]
        public SoapBody SoapBody { get; set; }
    }

    public class smsResponse
    {
        [JsonProperty("?xml")]
        public Xml Xml { get; set; }
        [JsonProperty("soap:Envelope")]
        public SoapEnvelope SoapEnvelope { get; set; }
    }

}
