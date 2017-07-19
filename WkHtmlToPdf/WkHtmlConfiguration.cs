using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WkHtmlToPdf
{
    public class WkHtmlConfiguration
    {
        public string OutputFilepath { get; set; }
        public string Url { get; set; }
        public int MarginTop { get; set; }
        public int MarginRight { get; set; }
        public int MarginBottom { get; set; }
        public int MarginLeft { get; set; }
        public int Dpi { get; set; }
        public bool Delete { get; set; }

        public WkHtmlConfiguration()
        {
            Dpi = 300;

            MarginTop = 15;
            MarginRight = 5;
            MarginBottom = 15;
            MarginLeft = 5;

            Url = "-";

            Delete = true;

            OutputFilepath = Path.Combine(Path.GetTempPath(), String.Format("{0}.pdf", Guid.NewGuid()));
        }

        public override string ToString()
        {
            StringBuilder configuration = new StringBuilder();

            configuration.AppendFormat("--dpi {0} ", Dpi);

            configuration.AppendFormat("--margin-top {0} ", MarginTop);
            configuration.AppendFormat("--margin-right {0} ", MarginRight);
            configuration.AppendFormat("--margin-bottom {0} ", MarginBottom);
            configuration.AppendFormat("--margin-left {0} ", MarginLeft);

            configuration.AppendFormat("--footer-right [page]/[topage] ", MarginLeft);

            configuration.Append("--page-size A4 ");
            configuration.AppendFormat("\"{0}\" \"{1}\"", Url, OutputFilepath);

            return configuration.ToString();
        }
    }
}
