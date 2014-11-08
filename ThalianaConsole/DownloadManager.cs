using System;
using System.Collections.Generic;
using System.Diagnostics;
using BongApiV1.Public;

namespace ThalianaConsole
{
    public class DownloadManager
    {
 
        public bool DownloadFiles()
        {
            try
            {
                DownloadRecordingsInternal();
                return true;
            }
            catch (Exception e)
            {
                Program.LogWriteLine("Process terminating due to exception {0} from {1} with message '{2}'", e.GetType().Name, e.Source, e.Message);
                return false;
            }
        }

        private void DownloadRecordingsInternal()
        {
            var downloadableRecordings = new List<DownloadableRecording>();

            BongSession session;

            try
            {
                session = new BongSession(Settings.BongUsername,
                                          Settings.BongPassword,
                                          waitMillisecondsBetweenCalls: Settings.WaitBetweenCallsMSec,
                                          loggingDirectory: Settings.LoggingDirectory);

                Program.LogWriteLine("Connected to Bong.tv as {0}", Settings.BongUsername);
            }
            catch (BongException)
            {
                Program.LogWriteLine("Could not connected to Bong.tv as {0}", Settings.BongUsername);
                throw;
            }

            try
            {
                foreach (var recording in session.Recordings)
                {
                    if (recording.Status == BongRecordingState.Recorded)
                        downloadableRecordings.Add(new DownloadableRecording(recording));
                }

                Program.LogWriteLine("Found {0} recordings waiting for download", downloadableRecordings.Count);

                foreach (var recording in downloadableRecordings)
                {
                    recording.Download();
                }

            }
            finally
            {
                session.Close();
            }
        }
    }
}
