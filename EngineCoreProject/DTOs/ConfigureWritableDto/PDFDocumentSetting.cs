using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.ConfigureWritableDto
{

    public class Pdfdocumentsetting
    {
        public Globalsettings GlobalSettings { get; set; }
        public Pagessettings PagesSettings { get; set; }
        public SignutureSettings SignutureSettings { get; set; }

        public RecordsSettings RecordsSettings { get; set; }

        public string BaseURL { get; set; }
        public string ADVBaseURL { get; set; }
    }

    public class Globalsettings
    {
        public string Out { get; set; }
        public string OutPdf { get; set; } 
        public string Root { get; set; } 

    }

    public class Pagessettings
    {
        public Headersettings HeaderSettings { get; set; }
        public Footersettings FooterSettings { get; set; }
    }

    public class SignutureSettings
    {
        public Headersettings HeaderSettings { get; set; }
        public Footersettings FooterSettings { get; set; }
    }

    public class RecordsSettings
    {
        public Headersettings HeaderSettings { get; set; }
        public Footersettings FooterSettings { get; set; }
    }

    public class Headersettings
    {
        public string HtmUrl { get; set; }
    }

    public class Footersettings
    {
        public string HtmUrl { get; set; }
    }

}
