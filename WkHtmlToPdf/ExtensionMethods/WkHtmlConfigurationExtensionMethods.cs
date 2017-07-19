using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WkHtmlToPdf.ExtensionMethods
{
    public static class WkHtmlConfigurationExtensionMethods
    {
        public static WkHtmlConfiguration SetOutputFilepath(this WkHtmlConfiguration cfg, string outputFilepath)
        {
            cfg.OutputFilepath = outputFilepath;
            return cfg;
        }
    }
}
