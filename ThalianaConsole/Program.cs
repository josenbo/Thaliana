using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BongApiV1.Public;

namespace ThalianaConsole
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var downloadManager = new DownloadManager();

            if (!Settings.ReadConfiguration()) return;

            downloadManager.DownloadFiles();

            Settings.ReadConfiguration();
        }
    }
}
