using System;
using System.Collections.Generic;
// using System.Diagnostics;
using System.IO;
using System.Text;

namespace ThalianaConsole
{
    internal static class Settings
    {
        public static string BongUsername { get; private set; }
        public static string BongPassword { get; private set; }
        public static long WaitBetweenCallsMSec { get; private set; }
        public static string LoggingDirectory { get; private set; }
        public static string LogFilePath { get; private set; }
        public static string DownloadDirectory { get; private set; }
        public static string PlexDirectory { get; private set; }

        public static bool ReadConfiguration()
        {
            var args = Environment.GetCommandLineArgs();
            string configFilePath = (1 < args.Length) ? args[1] : null;

            if (string.IsNullOrWhiteSpace(configFilePath) || !File.Exists(configFilePath)) return false;

            var keyValue = ExtractKeyValuePairs(configFilePath);

            //  foreach (var key in keyValue.Keys)
            //  {
            //      Debug.WriteLine("'{0}' = '{1}'", key, keyValue[key]);
            //  }

            if (keyValue.ContainsKey("BongUsername"))
            {
                BongUsername = keyValue["BongUsername"];
            }
            else return false;

            if (keyValue.ContainsKey("BongPassword"))
            {
                BongPassword = keyValue["BongPassword"];
            }
            else return false;

            if (keyValue.ContainsKey("DownloadDirectory"))
            {
                var p = keyValue["DownloadDirectory"];

                if (Directory.Exists(p))
                    DownloadDirectory = p;
                else
                    return false;
            }
            else return false;

            if (keyValue.ContainsKey("PlexDirectory"))
            {
                var p = keyValue["PlexDirectory"];

                if (Directory.Exists(p))
                    PlexDirectory = p;
                else
                    return false;
            }
            else return false;

            WaitBetweenCallsMSec = 100;

            if (keyValue.ContainsKey("WaitBetweenCallsMSec"))
            {

                var w = long.Parse(keyValue["WaitBetweenCallsMSec"]);

                if (49 < w && w < 6001)
                    WaitBetweenCallsMSec = w;
            }

            LoggingDirectory = null;
            LogFilePath = null;

            if (keyValue.ContainsKey("LoggingDirectory"))
            {
                var p = keyValue["LoggingDirectory"];

                if (Directory.Exists(p))
                {
                    var logFileCurrent = Path.Combine(p, "ThalianaConsole.log");
                    var logFileOld = Path.Combine(p, "ThalianaConsole.old.log");

                    if (File.Exists(logFileCurrent))
                    {
                        var fi = new FileInfo(logFileCurrent);
                        if (4000000 < fi.Length)
                        {
                            if (File.Exists(logFileOld))
                                File.Delete(logFileOld);

                            File.Move(logFileCurrent, logFileOld);
                        }
                    }

                    LoggingDirectory = p;
                    LogFilePath = logFileCurrent;

                }
               else return false;
            }

            return true;
        }

        private static Dictionary<string, string> ExtractKeyValuePairs(string configFilePath)
        {
            var retval = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

            var commentChar = "#";
            var delimiters = new[] {"="};

            if (!string.IsNullOrWhiteSpace(configFilePath) && File.Exists(configFilePath))
            {
                using (var sr = new StreamReader(configFilePath, Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        var s = sr.ReadLine();

                        if (s == null) continue;

                        s = s.Trim();

                        if (String.IsNullOrWhiteSpace(s) || s.StartsWith(commentChar)) continue;

                        var parts = s.Split(delimiters, 2, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 2)
                        {
                            var name = parts[0].Replace("\t", "").Replace(" ", "").Trim();
                            var value = parts[1].Trim();

                            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value)) continue;

                            retval.Add(name, value);
                        }
                    }

                    sr.Close();
                }
            }

            return retval;
        }

    }
}
