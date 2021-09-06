using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.PDFDto
{
    public class PdfPageSetting
    {
        public string HeaderUrl { set; get; }
        public string FooterUrl { set; get; }
        public string BodyDoc { set; get; }
    }
}
