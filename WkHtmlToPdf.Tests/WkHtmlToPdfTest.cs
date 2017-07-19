using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WkHtmlToPdf.Tests
{
    [TestFixture]
    public class WkHtmlToPdfTest
    {
        [Test]
        public void HtmlToFilepathTest()
        {
            string htmlString = "<html></html>";

            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\test.pdf";

            if (!File.Exists(filePath))
                File.Create(filePath);

            WkHtmlToPdf.HtmlToFile(htmlString, filePath);

            Assert.IsTrue(File.Exists(filePath));
        }

        [Test]
        public void HtmlToStreamTest()
        {
            string htmlString = "<html></html>";

            Stream ms = new MemoryStream();

            WkHtmlToPdf.HtmlToStream(htmlString, ms);

            Assert.AreNotEqual(0, ms.Length);

            ms.Close();
            ms.Dispose();
        }
    }
}
