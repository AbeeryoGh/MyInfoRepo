using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.PDFDto
{
    public class InfoDoc
    {
       
        public List<string> ImagePaths { get; set; }

        public string TransactionNo { get; set; }

        public string RecQRUrl { get; set; }

        public string Title { get; set; }
        public string path { get; set; }// = "transactions\\";



    }
}
