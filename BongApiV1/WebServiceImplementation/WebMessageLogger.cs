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
        private const int DaysToKeep = 1;  // accepted range 1..14

        private static string _logDir = null;

        internal static void InitializeLogging(string baseDirectory)
        {
            var deDe = new CultureInfo("de-DE");

            if (!string.IsNullOrWhiteSpace(baseDirectory) && Directory.Exists(baseDirectory))
            {
                var weblogBaseDir = Path.Combine(baseDirectory, "WebMessages");

                if (!Directory.Exists(weblogBaseDir))
                    Directory.CreateDirectory(weblogBaseDir);

                var oldestDayToKeep = DateTime.Today.AddDays((0 < DaysToKeep && DaysToKeep < 14) ? -DaysToKeep : -1);

                foreach (var directory in new DirectoryInfo(weblogBaseDir).GetDirectories())
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

                    if (date < oldestDayToKeep)
                    {
                        directory.Delete(true);
                    }
                }

                _logDir = Path.Combine(weblogBaseDir, String.Format("{0:yyyyMMdd}", DateTime.Today));

                if (!Directory.Exists(_logDir))
                    Directory.CreateDirectory(_logDir);
            }
        }

        internal static void WriteLogMessage(string title, string message)
        {
            if (_logDir == null) return;

            var filename = Path.Combine(_logDir, string.Format("{0:HHmmssffff} {1}.log", DateTime.Now, title));

            using (var sw = new StreamWriter(filename))
            {
                sw.Write(message);
                sw.Close();
            }
        }
    }
}
