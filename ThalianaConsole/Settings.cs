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
        public static string DownloadDirectory { get; private set; }

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

            WaitBetweenCallsMSec = 100;

            if (keyValue.ContainsKey("WaitBetweenCallsMSec"))
            {

                var w = long.Parse(keyValue["WaitBetweenCallsMSec"]);

                if (49 < w && w < 6001)
                    WaitBetweenCallsMSec = w;
            }

            if (keyValue.ContainsKey("LoggingDirectory"))
            {
                var p = keyValue["LoggingDirectory"];

                if (Directory.Exists(p))
                    LoggingDirectory = p;
                else
                    return false;
            }
            else LoggingDirectory = null;

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
