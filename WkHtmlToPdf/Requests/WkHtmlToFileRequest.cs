using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WkHtmlToPdf.Requests
{
    public class WkHtmlToFileRequest : WkHtmlRequest
    {
        public WkHtmlToFileRequest(string html, string filepath)
            : base(html)
        {
            base.Configuration.OutputFilepath = filepath;
            base.Configuration.Delete = false;
        }

        public WkHtmlToFileRequest(string html, string filepath, WkHtmlConfiguration cfg)
            : base(html, cfg)
        {
            base.Configuration.OutputFilepath = filepath;
            base.Configuration.Delete = false;
        }
    }
}
