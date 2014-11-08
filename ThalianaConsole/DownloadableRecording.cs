using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using BongApiV1.Public;

namespace ThalianaConsole
{
    public class DownloadableRecording
    {
        //todo Testmodus ausschalten
        private bool isTestMode = true;
        private readonly Recording _recording;
        private string _targetDir;
        private string _fileBaseName;

        public DownloadableRecording(Recording recording)
        {
            _recording = recording;
        }

        public void Download()
        {
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            Program.LogWrite("download [{0} | {1:dd.MM.yyyy HH:mm} | {2}] ", 
                             RPad(_recording.ChannelName, 6, cutLongerText: true), 
                             _recording.StartsAt, 
                             RPad(_recording.Title, 30, cutLongerText: true));

            _fileBaseName = string.Format("TV_{0:yyyyMMdd_HHmm}_{1}", _recording.StartsAt, CleanName(_recording.ChannelName));

            var folderName = string.Format("{0:yyyyMMdd_HHmm}_{1}_{2}", _recording.StartsAt, CleanName(_recording.ChannelName), CamelCaseName(_recording.Title));
            
            _targetDir = Path.Combine(Settings.DownloadDirectory, folderName);

            if (Directory.Exists(_targetDir))
            {
                Program.LogWriteLine("skipped because folder {0} already exists", folderName);
                timer.Stop();
                return;
            }

            Directory.CreateDirectory(_targetDir);

            CreateFlagFile();

            CreateDataXml();
            
            CreatePlexMediaXml();

            DownloadImage();

            var downloadBytes = DownloadBestResolutionVideo();

            if (!isTestMode) _recording.Delete();

            RemoveFlagFile();

            timer.Stop();
            Program.LogWriteLine("{0:#,##0} Bytes in {1:hh\\:mm\\:ss}", downloadBytes, timer.Elapsed);
        }

        private long DownloadBestResolutionVideo()
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

            if (dlSelected == null) return 0;

            var fileName = String.Format("{0}.mp4", _fileBaseName);
            var filePath = Path.Combine(_targetDir, fileName);
            var webClient = new WebClient();

            if (isTestMode) return 1234567;

            webClient.DownloadFile(dlSelected.Url, filePath);

            var fi = new FileInfo(filePath);
            return fi.Length;
        }

        private void DownloadImage()
        {
            var dlSelected = _recording.Downloads.Values.FirstOrDefault(
                                  file => file.FileType == BongDownloadFileType.Image);

            if (dlSelected == null) return;

            var fileName = String.Format("{0}-poster.jpg", _fileBaseName);
            var filePath = Path.Combine(_targetDir, fileName);
            var webClient = new WebClient();

            webClient.DownloadFile(dlSelected.Url, filePath);
        }

        private void CreateDataXml()
        {
            using (var sw = new StreamWriter(Path.Combine(_targetDir, "Data.xml"), false, Encoding.UTF8))
            {
                var start = string.Format("{0:dd-MM-yyyy HH:mm}", _recording.StartsAt);
                var tsp = _recording.EndsAt.Subtract(_recording.StartsAt);
                var duration = string.Format("{0:hh\\:mm}", tsp);

                sw.Write("<?xml version='1.0' encoding='UTF-8'?>\n");
                sw.Write("<recording>\n");
                sw.Write("  {0}\n", CleanXmlElement("title", _recording.Title));
                sw.Write("  {0}\n", CleanXmlElement("subtitle", _recording.Subtitle));
                sw.Write("  {0}\n", CleanXmlElement("description", _recording.LongText ?? _recording.ShortText));
                sw.Write("  {0}\n", CleanXmlElement("channel", _recording.ChannelName));
                sw.Write("  {0}\n", CleanXmlElement("start", start));
                sw.Write("  {0}\n", CleanXmlElement("duration", duration));
                sw.Write("</recording>\n");
                sw.Close();
            }
        }

        private void CreatePlexMediaXml()
        {
            var fileName = String.Format("{0}.nfo", _fileBaseName);
            var filePath = Path.Combine(_targetDir, fileName);

            using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                var title = String.Format("{0}, {1:dd.MM.yyyy HH:mm}, {2}",
                                          _recording.Title,
                                          _recording.StartsAt,
                                          _recording.ChannelName);
                
                var tsp = _recording.EndsAt.Subtract(_recording.StartsAt);
                var duration = string.Format("{0}", tsp.TotalMinutes);

                var sb = new StringBuilder();

                var d = (_recording.LongText ?? _recording.ShortText) ?? _recording.Title;
                sb.Append(d.Trim());
                
                if (!String.IsNullOrWhiteSpace(_recording.Subtitle))
                    sb.AppendFormat(". Untertitel: {0}", _recording.Subtitle);
                
                sb.AppendFormat(". {0} vom {1:dd.MM.yyyy HH:mm}. Dauer {2} Minuten",
                                _recording.Title,
                                _recording.StartsAt,
                                tsp.TotalMinutes);

                sw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\n");
                sw.Write("<movie>\n");
                sw.Write("  {0}\n", CleanXmlElement("title", title));
                sw.Write("  {0}\n", CleanXmlElement("outline", sb.ToString()));
                sw.Write("  {0}\n", CleanXmlElement("runtime", duration));
                sw.Write("  {0}\n", CleanXmlElement("watched", "false"));
                sw.Write("</movie>\n");
                sw.Close();
            }
        }

        private string CleanXmlElement(string tagName, string tagContent)
        {
            if (tagContent == null) return String.Format("<{0} />", tagName);

            return String.Format("<{0}>{1}</{0}>", tagName, CleanXmlText(tagContent));
        }

        private string CleanXmlText(string text)
        {
            return text.Replace("& ", "&amp; ").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        private void CreateFlagFile()
        {
            using (var sw = new StreamWriter(Path.Combine(_targetDir, "workInProgress")))
            {
                sw.Write("{0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
                sw.Close();
            }
        }

        private void RemoveFlagFile()
        {
            var filePath = Path.Combine(_targetDir, "workInProgress");

            if (File.Exists(filePath))
                File.Delete(filePath);
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
    
        private static string LPad(string text, int paddingSize, char paddingChar = ' ', bool cutLongerText = false)
        {
            if (cutLongerText && paddingSize < text.Length)
                text = text.Substring(0, paddingSize);

            return text.PadLeft(paddingSize, paddingChar);
        }

        private static string RPad(string text, int paddingSize, char paddingChar = ' ', bool cutLongerText = false)
        {
            if (cutLongerText && paddingSize < text.Length)
                text = text.Substring(0, paddingSize);

            return text.PadRight(paddingSize, paddingChar);
        }
    }
}
