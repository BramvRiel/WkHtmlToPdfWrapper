using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WkHtmlToPdf.Requests
{
    public abstract class WkHtmlRequest
    {
        public string Html { get; private set; }
        public WkHtmlConfiguration Configuration { get; set; }

        public WkHtmlRequest(string html, WkHtmlConfiguration cfg)
        {
            this.Html = html;

            if (cfg == null)
            {
                this.Configuration = new WkHtmlConfiguration();
            }
        }

        public WkHtmlRequest(string html)
        {
            this.Html = html;

            this.Configuration = new WkHtmlConfiguration();
        }
    }
}
