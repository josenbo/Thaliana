using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongApiV1.WebServiceImplementation
{
    internal static class WebMessageLogger
    {
        private const string BaseDirectory = @"D:\Entwicklung\2014\Thaliana\Basisverzeichnis\Log\WebMessages";
        private const bool LoggingEnabled = true;

        private static readonly string LogDir;
        private static readonly bool Enabled ;

        static WebMessageLogger()
        {
            var ok = false;
            var deDe = new CultureInfo("de-DE");

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (LoggingEnabled)
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                if (!string.IsNullOrWhiteSpace(BaseDirectory) && Directory.Exists(BaseDirectory))
                {
                    var yesterday = DateTime.Today.AddDays(-1);

                    foreach (var directory in new DirectoryInfo(BaseDirectory).GetDirectories())
                    {
                        DateTime date;

                        try
                        {
                            date = DateTime.ParseExact(directory.Name, "yyyyMMdd", deDe);
                        }
                        catch (FormatException)
                        {
                            continue;
                        }

                        if (date < yesterday)
                        {
                            directory.Delete(true);
                        }
                    }

                    LogDir = Path.Combine(BaseDirectory, String.Format("{0:yyyyMMdd}", DateTime.Today));

                    if (!Directory.Exists(LogDir))
                        Directory.CreateDirectory(LogDir);

                    ok = true;
                }
            }

            Enabled = ok;
        }

        internal static void WriteLogMessage(string title, string message)
        {
            var filename = Path.Combine(LogDir, string.Format("{0:HHmmssffff} {1}.log", DateTime.Now, title));

            using (var sw = new StreamWriter(filename))
            {
                sw.Write(message);
                sw.Close();
            }
        }
    }
}
