using System;
using System.Collections.Generic;
using System.Diagnostics;
using BongApiV1.Public;

namespace ThalianaConsole
{
    public class DownloadManager
    {
        private readonly List<DownloadableRecording> _recordings = new List<DownloadableRecording>();
 
        public void DownloadFiles()
        {
            var session = new BongSession(Settings.BongUsername,
                                          Settings.BongPassword,
                                          waitMillisecondsBetweenCalls: Settings.WaitBetweenCallsMSec,
                                          loggingDirectory: Settings.LoggingDirectory);


            foreach (var recording in session.Recordings)
            {
                if (recording.Status == BongRecordingState.Recorded)    
                    _recordings.Add(new DownloadableRecording(recording));
            }

            foreach (var recording in _recordings)
            {
                try
                {
                    recording.Download();
                }
                catch (Exception e)
                {
                    Debug.Print("Exception beim Download: {0}", e.Message);                    
                }
            }

            session.Close();
        }
    }
}
