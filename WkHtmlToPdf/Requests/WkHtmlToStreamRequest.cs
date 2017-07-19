using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WkHtmlToPdf.Requests
{
    public class WkHtmlToStreamRequest : WkHtmlRequest
    {
        public Stream OutputStream { get; set; }

        public WkHtmlToStreamRequest(string html, Stream stream)
            : base(html)
        {
            this.OutputStream = stream;
        }

        public WkHtmlToStreamRequest(string html, Stream stream, WkHtmlConfiguration cfg)
            : base(html, cfg)
        {
            this.OutputStream = stream;
        }
    }
}
