using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Credential
{
    public class GetVcIDDto
    {
        public string transactionRefNo { get; set; }
        public string documentType { get; set; }
        public evidenceBlockChain evidence { get; set; }
        public claimBlockChain claims { get; set; }
        public documentMetaData documentMetaData { get; set; }
    }
}


public class claimBlockChain
{
    public string applicationId { get; set; }
    public string documentId { get; set; }

}

public class documentMetaData
{
    public string documentId { get; set; }
    public string applicationId { get; set; }
}

public class evidenceBlockChain
{
    public string file { get; set; }
    public string fileType { get; set; }
}

