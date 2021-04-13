using System;
using System.Collections.Generic;
using System.Text;

namespace Altsource
{
    public class AppSetting
    {
        public string ApplicationName { get; set; }
        public string Version { get; set; }
        public string Headless { get; set; }
        public string RecipientsEmail { get; set; }
        public string ReportEmail { get; set; }
        public string ReportEmailPassword { get; set; }
        public string SMTPHost { get; set; }
        public string SMTPPort { get; set; }
    }
}
