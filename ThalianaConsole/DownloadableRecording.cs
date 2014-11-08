using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BongApiV1.Public;

namespace ThalianaConsole
{
    public class DownloadableRecording
    {
        private readonly Recording _recording;
        private string _targetDir;

        public DownloadableRecording(Recording recording)
        {
            _recording = recording;
        }

        public void Download()
        {
            var folderName = string.Format("{0:yyyyMMdd_HHmm}_{1}_{2}", _recording.StartsAt, CleanName(_recording.ChannelName), CamelCaseName(_recording.Title));
            
            _targetDir = Path.Combine(Settings.DownloadDirectory, folderName);
            
            if (!Directory.Exists(_targetDir))
                Directory.CreateDirectory(_targetDir);

            Debug.Print("download {0}", _recording.Title);

            DownloadBestResolutionVideo();
        }

        private void DownloadBestResolutionVideo()
        {
            Download dlSelected = null;

            foreach (var file in _recording.Downloads.Values)
            {
                if (file.FileType == BongDownloadFileType.Video)
                {
                    switch (file.Quality)
                    {
                        case "hd":
                            dlSelected = file;
                            break;
                        case "hq":
                            if (dlSelected == null || dlSelected.Quality != "hd")
                                dlSelected = file;
                            break;
                        default:
                            if (dlSelected == null)
                                dlSelected = file;
                            break;
                    }
                }
            }

            if (dlSelected != null)
            {
                var fileName = String.Format("video_{0}.mp4", dlSelected.Quality);
                var filePath = Path.Combine(_targetDir, fileName);
                var webClient = new WebClient();

                // webClient.DownloadFile(dlSelected.Url, filePath);
            }
        }

        private string CleanName(string text)
        {
            text = RemoveDiacritics(text);

            var sb = new StringBuilder();

            foreach (var c in text)
            {

                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        private string CamelCaseName(string text)
        {
            var newWord = true;

            text = RemoveDiacritics(text);

            var sb = new StringBuilder();

            foreach (var c in text)
            {
                if (char.IsLetterOrDigit(c))
                {
                    string ch = c.ToString(CultureInfo.InvariantCulture);

                    if (newWord)
                    {
                        sb.Append(ch.ToUpperInvariant());
                        newWord = false;
                    }
                    else sb.Append(ch.ToLowerInvariant());
                }
                else newWord = true;
            }

            return sb.ToString();
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
