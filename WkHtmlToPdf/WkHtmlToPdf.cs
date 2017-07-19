using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WkHtmlToPdf.Requests;

namespace WkHtmlToPdf
{
    public class WkHtmlToPdf
    {
        private static string _wkHtmlToPdfPath { get; set; }

        private static void FindWkHtmlToPdfPath()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "wkhtmltopdf.exe";
            if (File.Exists(filePath))
            {
                _wkHtmlToPdfPath = filePath;
                return;
            }

            string programFilesPath = Environment.GetEnvironmentVariable("ProgramFiles");
            filePath = Path.Combine(programFilesPath, @"wkhtmltopdf\wkhtmltopdf.exe");
            if (File.Exists(filePath))
            {
                _wkHtmlToPdfPath = filePath;
                return;
            }

            filePath = Path.Combine(programFilesPath, @"wkhtmltopdf\bin\wkhtmltopdf.exe");
            if (File.Exists(filePath))
            {
                _wkHtmlToPdfPath = filePath;
                return;
            }

            string programFilesx86Path = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            filePath = Path.Combine(programFilesx86Path, @"wkhtmltopdf\wkhtmltopdf.exe");
            if (File.Exists(filePath))
            {
                _wkHtmlToPdfPath = filePath;
                return;
            }
            filePath = Path.Combine(programFilesx86Path, @"wkhtmltopdf\bin\wkhtmltopdf.exe");
            if (File.Exists(filePath))
            {
                _wkHtmlToPdfPath = filePath;
                return;
            }

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                filePath = Path.Combine(path, @"wkhtmltopdf.exe");
                if (File.Exists(filePath))
                {
                    _wkHtmlToPdfPath = filePath;
                    return;
                }

                filePath = Path.Combine(path, @"wkhtmltopdf\wkhtmltopdf.exe");
                if (File.Exists(filePath))
                {
                    _wkHtmlToPdfPath = filePath;
                    return;
                }

                filePath = Path.Combine(path, @"wkhtmltopdf\bin\wkhtmltopdf.exe");
                if (File.Exists(filePath))
                {
                    _wkHtmlToPdfPath = filePath;
                    return;
                }
            }

            throw new FileNotFoundException("WkHtmlToPdf.exe not found.");
        }

        private static string WkHtmlToPdfPath
        {
            get
            {
                if (_wkHtmlToPdfPath == null)
                {
                    FindWkHtmlToPdfPath();
                }

                return _wkHtmlToPdfPath;
            }
        }

        public static void HtmlToStream(string html, Stream stream)
        {
            var request = new WkHtmlToStreamRequest(html, stream);
            GeneratePdf(request);
        }

        public static void HtmlToFile(string html, string filepath)
        {
            var request = new WkHtmlToFileRequest(html, filepath);
            GeneratePdf(request);
        }

        public static void GeneratePdf(WkHtmlRequest request)
        {
            try
            {
                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = WkHtmlToPdfPath;
                    process.StartInfo.Arguments = request.Configuration.ToString();
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardInput = true;

                    using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                    using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                    {
                        DataReceivedEventHandler outputHandler = (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                outputWaitHandle.Set();
                            }
                            else
                            {
                                output.AppendLine(e.Data);
                            }
                        };

                        DataReceivedEventHandler errorHandler = (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                errorWaitHandle.Set();
                            }
                            else
                            {
                                error.AppendLine(e.Data);
                            }
                        };

                        process.OutputDataReceived += outputHandler;
                        process.ErrorDataReceived += errorHandler;

                        try
                        {
                            process.Start();

                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();

                            if (request.Html != null)
                            {
                                using (var stream = process.StandardInput)
                                {
                                    byte[] buffer = Encoding.UTF8.GetBytes(request.Html);
                                    stream.BaseStream.Write(buffer, 0, buffer.Length);
                                    stream.WriteLine();
                                }
                            }

                            int timout = 60000;

                            if (process.WaitForExit(timout)
                                && outputWaitHandle.WaitOne(timout)
                                && errorWaitHandle.WaitOne(timout))
                            {
                                if (process.ExitCode != 0 && !File.Exists(request.Configuration.OutputFilepath))
                                {
                                    throw new Exception(string.Format("Error {0}", error));
                                }
                            }
                            else
                            {
                                if (!process.HasExited)
                                    process.Kill();

                                throw new Exception("WkHtmlToPdf timed out");
                            }
                        }
                        finally
                        {
                            process.OutputDataReceived -= outputHandler;
                            process.ErrorDataReceived -= errorHandler;
                        }
                    }
                }

                if (request is WkHtmlToStreamRequest)
                {
                    var streamRequest = request as WkHtmlToStreamRequest;
                    if (streamRequest.OutputStream != null)
                    {
                        using (Stream fs = new FileStream(request.Configuration.OutputFilepath, FileMode.Open))
                        {
                            byte[] buffer = new byte[32 * 1024];
                            int read;

                            while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                                streamRequest.OutputStream.Write(buffer, 0, read);
                        }
                    }
                }
            }
            finally
            {
                if (request.Configuration.Delete && File.Exists(request.Configuration.OutputFilepath))
                    File.Delete(request.Configuration.OutputFilepath);
            }
        }
    }
}
