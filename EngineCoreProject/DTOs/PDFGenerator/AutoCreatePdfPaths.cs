using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.PDFGenerator
{
    public class AutoCreatePdfPaths
    {
        public string TransactionDoc { set; get; }
        public Dictionary<string, string> RecordPaths = new Dictionary<string, string>();
        public Dictionary<int, string> RecordIdPaths = new Dictionary<int, string>();
    }
}
