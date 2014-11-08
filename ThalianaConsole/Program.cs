using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BongApiV1.Public;

namespace ThalianaConsole
{
    internal static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var downloadManager = new DownloadManager();

            if (!Settings.ReadConfiguration()) return;

            LogWriteLine("{0:dd.MM.yyyy HH:mm:ss} Application starting", DateTime.Now);

            downloadManager.DownloadFiles();

            MoveFinishedDownloadsToPlex();

            LogWriteLine("{0:dd.MM.yyyy HH:mm:ss} Application finished", DateTime.Now);
        }

        private static void MoveFinishedDownloadsToPlex()
        {
            var di = new DirectoryInfo(Settings.DownloadDirectory);

            foreach (var sdi in di.EnumerateDirectories())
            {
                if (!File.Exists(Path.Combine(sdi.FullName, "Data.xml")) ||
                    File.Exists(Path.Combine(sdi.FullName, "workInProgress")))
                    continue;

                var target = Path.Combine(Settings.PlexDirectory, sdi.Name);
                if (!Directory.Exists(target))
                    sdi.MoveTo(target);
            }
        }

        public static void LogWriteLine(string format, params object[] args)
        {
            LogWriteInternal(format + "\n", args);
        }

        public static void LogWrite(string format, params object[] args)
        {
            LogWriteInternal(format, args);
        }

        private static void LogWriteInternal(string format, params object[] args)
        {
            if (Settings.LogFilePath == null) return;

            using (var sw = new StreamWriter(Settings.LogFilePath, true, Encoding.Default))
            {
                sw.Write(string.Format(format, args));
                sw.Close();
            }
        }
    }
}
