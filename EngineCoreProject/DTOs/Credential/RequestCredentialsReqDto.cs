using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Credential
{
    public class RequestCredentialsReqDto
    {
        public string TransactionRefNo { get; set; }
        public string documentType { get; set; }
        public RequestDta requestedData { get; set; }
        public string VCID { get; set; }
        public string dataOnly { get; set; }
    }
}

public class RequestDta
{
    //public string appNo { get; set; }
    public string contractNo { get; set; }
    public string contractDate { get; set; }
}