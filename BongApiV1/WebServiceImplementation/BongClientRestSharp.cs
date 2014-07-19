using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using BongApiV1.Public;
using BongApiV1.WebServiceContract;
using RestSharp;

namespace BongApiV1.WebServiceImplementation
{
    public class BongClientRestSharp : IBongClient
    {
        private const string BaseUrl = "http://www.bong.tv/api/v1";
        private readonly CultureInfo _deDe = new CultureInfo("de-DE");
        private readonly long _waitMillisecondsBetweenCalls;
        private DateTime _lastCall;

        private readonly RestClient _client;

        public string Username { get; set; }
        public string Password { get; set; }

        internal BongClientRestSharp(long waitMillisecondsBetweenCalls)
        {
            _client = new RestClient(BaseUrl) { CookieContainer = new System.Net.CookieContainer() };
            _waitMillisecondsBetweenCalls = waitMillisecondsBetweenCalls;
            _lastCall = DateTime.Today;
        }

        public BongResponseLoginUser LoginUser()
        {
            var request = new RestRequest("user_sessions.json", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new Dictionary<string, string> {{"login", Username}, {"password", Password}});

            WaitBetweenCalls();

            var response = _client.Execute<EmptyResponse>(request);

            StoreTimeOfLastCall();

            #region log request and response data
            var sb = new StringBuilder();
            sb.AppendLine("request.resource           = " + request.Resource);
            sb.AppendLine("request.method             = " + request.Method.ToString());
            sb.AppendLine("request.parameter.login    = " + Username);
            sb.AppendLine("request.parameter.password = " + Password);
            sb.AppendLine();
            sb.AppendLine("response.StatusCode        = " + response.StatusCode.ToString());
            sb.AppendLine("response.StatusCode (num)  = " + (int)response.StatusCode);
            sb.AppendLine("response.StatusDescription = " + response.StatusDescription);
            sb.AppendLine("response.ResponseStatus    = " + response.ResponseStatus);
            sb.AppendLine("response.ErrorMessage      = " + response.ErrorMessage);
            sb.AppendLine("response.ContentEncoding   = " + response.ContentEncoding);
            sb.AppendLine("response.ContentLength     = " + response.ContentLength);
            sb.AppendLine("response.ContentType       = " + response.ContentType);
            sb.AppendLine();
            sb.AppendLine("response.Content");
            sb.AppendLine(new string('-', 80));
            sb.AppendLine(JsonHelper.FormatJson(response.Content));
            sb.AppendLine(new string('-', 80));
            sb.AppendLine();

            WebMessageLogger.WriteLogMessage("user_session", sb.ToString());
            #endregion

            var retval = new BongResponseLoginUser();

            if (response.StatusCode == HttpStatusCode.Created)
            {
                retval.Success = true;
            }
            else
            {
                retval.Success = false;
                retval.ErrorMessage = response.ErrorMessage;
            }

            return retval;
        }

        public BongResponseListRecordings ListRecordings()
        {
            var request = new RestRequest("recordings.json", Method.GET);
            var response = _client.Execute<ListRecordingsResponse>(request);

            WaitBetweenCalls();

            var retval = new BongResponseListRecordings();

            StoreTimeOfLastCall();

            #region log request and response data
            var sb = new StringBuilder();
            sb.AppendLine("request.resource           = " + request.Resource);
            sb.AppendLine("request.method             = " + request.Method.ToString());
            sb.AppendLine();
            sb.AppendLine("response.StatusCode        = " + response.StatusCode.ToString());
            sb.AppendLine("response.StatusCode (num)  = " + (int)response.StatusCode);
            sb.AppendLine("response.StatusDescription = " + response.StatusDescription);
            sb.AppendLine("response.ResponseStatus    = " + response.ResponseStatus);
            sb.AppendLine("response.ErrorMessage      = " + response.ErrorMessage);
            sb.AppendLine("response.ContentEncoding   = " + response.ContentEncoding);
            sb.AppendLine("response.ContentLength     = " + response.ContentLength);
            sb.AppendLine("response.ContentType       = " + response.ContentType);
            sb.AppendLine();
            sb.AppendLine("response.Content");
            sb.AppendLine(new string('-', 80));
            sb.AppendLine(JsonHelper.FormatJson(response.Content));
            sb.AppendLine(new string('-', 80));
            sb.AppendLine();

            WebMessageLogger.WriteLogMessage("recordings", sb.ToString());
            #endregion

            if (response.StatusCode == HttpStatusCode.OK)
            {
                retval.Success = true;

                foreach (var recording in response.Data.Recordings)
                {
                    var rec = ExtractRecordingFromResponse(recording);
                    retval.Recordings.Add(rec.Id, rec);
                }
            }
            else
            {
                retval.Success = false;
                retval.ErrorMessage = response.ErrorMessage;
            }

            return retval;
        }

        public BongResponseCreateRecording CreateRecording(string broadcastId)
        {
            var request = new RestRequest("recordings.json", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new Dictionary<string, string> { { "broadcast_id", broadcastId } });

            WaitBetweenCalls();

            var response = _client.Execute<EmptyResponse>(request);

            StoreTimeOfLastCall();

            #region log request and response data
            var sb = new StringBuilder();
            sb.AppendLine("request.resource           = " + request.Resource);
            sb.AppendLine("request.method             = " + request.Method.ToString());
            sb.AppendLine("request.param.broadcast-id = " + broadcastId);
            sb.AppendLine();
            sb.AppendLine("response.StatusCode        = " + response.StatusCode.ToString());
            sb.AppendLine("response.StatusCode (num)  = " + (int)response.StatusCode);
            sb.AppendLine("response.StatusDescription = " + response.StatusDescription);
            sb.AppendLine("response.ResponseStatus    = " + response.ResponseStatus);
            sb.AppendLine("response.ErrorMessage      = " + response.ErrorMessage);
            sb.AppendLine("response.ContentEncoding   = " + response.ContentEncoding);
            sb.AppendLine("response.ContentLength     = " + response.ContentLength);
            sb.AppendLine("response.ContentType       = " + response.ContentType);
            sb.AppendLine();
            sb.AppendLine("response.Content");
            sb.AppendLine(new string('-', 80));
            sb.AppendLine(JsonHelper.FormatJson(response.Content));
            sb.AppendLine(new string('-', 80));
            sb.AppendLine();

            WebMessageLogger.WriteLogMessage("recordings-create", sb.ToString());
            #endregion


            var retval = new BongResponseCreateRecording();

            if (response.StatusCode == HttpStatusCode.Created)
            {
                retval.Success = true;
            }
            else
            {
                retval.Success = false;
                retval.ErrorMessage = response.ErrorMessage;
            }

            return retval;
        }

        public BongResponseDeleteRecording DeleteRecording(string recordingId)
        {
            var request = new RestRequest(String.Format("recordings/{0}.json", recordingId), Method.DELETE);

            WaitBetweenCalls();

            var response = _client.Execute<EmptyResponse>(request);

            StoreTimeOfLastCall();

            #region log request and response data
            var sb = new StringBuilder();
            sb.AppendLine("request.resource           = " + request.Resource);
            sb.AppendLine("request.method             = " + request.Method.ToString());
            sb.AppendLine();
            sb.AppendLine("response.StatusCode        = " + response.StatusCode.ToString());
            sb.AppendLine("response.StatusCode (num)  = " + (int)response.StatusCode);
            sb.AppendLine("response.StatusDescription = " + response.StatusDescription);
            sb.AppendLine("response.ResponseStatus    = " + response.ResponseStatus);
            sb.AppendLine("response.ErrorMessage      = " + response.ErrorMessage);
            sb.AppendLine("response.ContentEncoding   = " + response.ContentEncoding);
            sb.AppendLine("response.ContentLength     = " + response.ContentLength);
            sb.AppendLine("response.ContentType       = " + response.ContentType);
            sb.AppendLine();
            sb.AppendLine("response.Content");
            sb.AppendLine(new string('-', 80));
            sb.AppendLine(JsonHelper.FormatJson(response.Content));
            sb.AppendLine(new string('-', 80));
            sb.AppendLine();

            WebMessageLogger.WriteLogMessage("recordings-delete", sb.ToString());
            #endregion

            var retval = new BongResponseDeleteRecording();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                retval.Success = true;
            }
            else
            {
                retval.Success = false;
                retval.ErrorMessage = response.ErrorMessage;
            }

            return retval;
        }

        public BongResponseListChannels ListChannels()
        {
            var request = new RestRequest("channels.json", Method.GET);

            WaitBetweenCalls();

            var response = _client.Execute<ListChannelsResponse>(request);

            StoreTimeOfLastCall();

            #region log request and response data
            var sb = new StringBuilder();
            sb.AppendLine("request.resource           = " + request.Resource);
            sb.AppendLine("request.method             = " + request.Method.ToString());
            sb.AppendLine();
            sb.AppendLine("response.StatusCode        = " + response.StatusCode.ToString());
            sb.AppendLine("response.StatusCode (num)  = " + (int)response.StatusCode);
            sb.AppendLine("response.StatusDescription = " + response.StatusDescription);
            sb.AppendLine("response.ResponseStatus    = " + response.ResponseStatus);
            sb.AppendLine("response.ErrorMessage      = " + response.ErrorMessage);
            sb.AppendLine("response.ContentEncoding   = " + response.ContentEncoding);
            sb.AppendLine("response.ContentLength     = " + response.ContentLength);
            sb.AppendLine("response.ContentType       = " + response.ContentType);
            sb.AppendLine();
            sb.AppendLine("response.Content");
            sb.AppendLine(new string('-', 80));
            sb.AppendLine(JsonHelper.FormatJson(response.Content));
            sb.AppendLine(new string('-', 80));
            sb.AppendLine();

            WebMessageLogger.WriteLogMessage("channels", sb.ToString());
            #endregion

            var retval = new BongResponseListChannels();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                retval.Success = true;

                foreach (var channel in response.Data.Channels)
                {
                    var chn = new Channel();

                    chn.Id = channel.Id;
                    chn.Name = channel.Name;
                    chn.Recordable = channel.Recordable;
                    chn.Hd = channel.Hd;

                    retval.Channels.Add(chn.Id, chn);
                }
            }
            else
            {
                retval.Success = false;
                retval.ErrorMessage = response.ErrorMessage;
            }

            return retval;
        }

        public BongResponseListBroadcasts ListBroadcasts(string channelId)
        {
            var resourceQuery = String.Format("broadcasts.json?channel_id={0}", channelId);
            return ListBroadcastsInternal(resourceQuery);
        }

        public BongResponseListBroadcasts ListBroadcasts(string channelId, DateTime date)
        {
            var resourceQuery = String.Format("broadcasts.json?channel_id={0}&date={1:dd-MM-yyyy}", channelId, date);
            return ListBroadcastsInternal(resourceQuery);
        }

        public BongResponseGetBroadcastDetails GetBroadcastDetails(string broadcastId)
        {
            var resourceQuery = String.Format("broadcasts/{0}.json", broadcastId);
            var request = new RestRequest(resourceQuery, Method.GET);

            WaitBetweenCalls();

            var response = _client.Execute<GetBroadcastDetailResponse>(request);

            StoreTimeOfLastCall();

            #region log request and response data
            var sb = new StringBuilder();
            sb.AppendLine("request.resource           = " + request.Resource);
            sb.AppendLine("request.method             = " + request.Method.ToString());
            sb.AppendLine();
            sb.AppendLine("response.StatusCode        = " + response.StatusCode.ToString());
            sb.AppendLine("response.StatusCode (num)  = " + (int)response.StatusCode);
            sb.AppendLine("response.StatusDescription = " + response.StatusDescription);
            sb.AppendLine("response.ResponseStatus    = " + response.ResponseStatus);
            sb.AppendLine("response.ErrorMessage      = " + response.ErrorMessage);
            sb.AppendLine("response.ContentEncoding   = " + response.ContentEncoding);
            sb.AppendLine("response.ContentLength     = " + response.ContentLength);
            sb.AppendLine("response.ContentType       = " + response.ContentType);
            sb.AppendLine();
            sb.AppendLine("response.Content");
            sb.AppendLine(new string('-', 80));
            sb.AppendLine(JsonHelper.FormatJson(response.Content));
            sb.AppendLine(new string('-', 80));
            sb.AppendLine();

            WebMessageLogger.WriteLogMessage("broadcasts-detail", sb.ToString());
            #endregion

            var retval = new BongResponseGetBroadcastDetails();

            if (response.StatusCode == HttpStatusCode.OK && response.Data.Broadcast != null)
            {
                retval.Success = true;

                var brc = ExtractBroadcastFromResponse(response.Data.Broadcast);
                retval.Broadcasts.Add(brc.Id, brc);
            }
            else
            {
                retval.Success = false;
                retval.ErrorMessage = response.ErrorMessage;
            }

            return retval;
        }

        public BongResponseSearchBroadcasts SearchBroadcasts(string searchPattern)
        {
            var request = new RestRequest("broadcasts/search.json", Method.GET);
            request.AddParameter("query", searchPattern);

            WaitBetweenCalls();

            var response = _client.Execute<ListBroadcastsResponse>(request);

            StoreTimeOfLastCall();

            #region log request and response data
            var sb = new StringBuilder();
            sb.AppendLine("request.resource           = " + request.Resource);
            sb.AppendLine("request.method             = " + request.Method.ToString());
            sb.AppendLine("request.query-string       = " + searchPattern);
            sb.AppendLine();
            sb.AppendLine("response.StatusCode        = " + response.StatusCode.ToString());
            sb.AppendLine("response.StatusCode (num)  = " + (int)response.StatusCode);
            sb.AppendLine("response.StatusDescription = " + response.StatusDescription);
            sb.AppendLine("response.ResponseStatus    = " + response.ResponseStatus);
            sb.AppendLine("response.ErrorMessage      = " + response.ErrorMessage);
            sb.AppendLine("response.ContentEncoding   = " + response.ContentEncoding);
            sb.AppendLine("response.ContentLength     = " + response.ContentLength);
            sb.AppendLine("response.ContentType       = " + response.ContentType);
            sb.AppendLine();
            sb.AppendLine("response.Content");
            sb.AppendLine(new string('-', 80));
            sb.AppendLine(JsonHelper.FormatJson(response.Content));
            sb.AppendLine(new string('-', 80));
            sb.AppendLine();

            WebMessageLogger.WriteLogMessage("broadcasts-search", sb.ToString());
            #endregion

            var retval = new BongResponseSearchBroadcasts();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                retval.Success = true;

                foreach (var broadcast in response.Data.Broadcasts)
                {
                    var brc = ExtractBroadcastFromResponse(broadcast);
                    retval.Broadcasts.Add(brc.Id, brc);
                }

                AddBroadcastDetails(retval.Broadcasts.Values);
            }
            else
            {
                retval.Success = false;
                retval.ErrorMessage = response.ErrorMessage;
            }

            return retval;
        }

        private Recording ExtractRecordingFromResponse(DataRecording recordingResponse)
        {
            var retval = new Recording();

            retval.Id = recordingResponse.Id;
            retval.BroadcastId = recordingResponse.BroadcastId;

            BongRecordingState state;
            if (Enum.TryParse(recordingResponse.Status, true, out state))
                retval.Status = state;
            else
                throw new BongException(String.Format("Don't know how to handle unknown recordingResponse status '{0}'.", recordingResponse.Status));

            retval.Title = recordingResponse.Title;
            retval.Subtitle = recordingResponse.Broadcast == null ? null : recordingResponse.Broadcast.Subtitle;
            var shortText = recordingResponse.Broadcast == null ? null : recordingResponse.Broadcast.ShortText;
            var longText = recordingResponse.Broadcast == null ? null : recordingResponse.Broadcast.LongText;
            retval.Description = ((shortText == null ? 0 : shortText.Length) < (longText == null ? 0 : longText.Length)) ? longText : shortText;
            retval.Country = recordingResponse.Broadcast == null ? null : recordingResponse.Broadcast.Country;
            retval.ProductionYear = recordingResponse.Broadcast == null ? null : recordingResponse.Broadcast.ProductionYear;

            retval.StartsAt = DateTime.ParseExact(recordingResponse.StartsAtDate + " " + recordingResponse.StartsAtTime,
                                               "dd.MM.yyyy HH:mm", _deDe, DateTimeStyles.AllowWhiteSpaces);
            retval.EndsAt = DateTime.ParseExact(recordingResponse.StartsAtDate + " " + recordingResponse.EndsAtTime,
                                             "dd.MM.yyyy HH:mm", _deDe, DateTimeStyles.AllowWhiteSpaces);
            if (retval.EndsAt < retval.StartsAt)
                retval.EndsAt = retval.EndsAt.AddDays(1);

            retval.ChannelId = recordingResponse.ChannelId;
            retval.ChannelName = recordingResponse.Broadcast == null ? null : recordingResponse.Broadcast.ChannelName;

            retval.Quality = recordingResponse.Quality;

            if (recordingResponse.Broadcast != null && recordingResponse.Broadcast.Serie != null)
            {
                retval.SerieId = recordingResponse.Broadcast.SerieId;
                retval.SerieSeason = recordingResponse.Broadcast.Serie.Season;
                retval.SerieEpisode = recordingResponse.Broadcast.Serie.Episode;
                retval.SerieEpisodeCount = recordingResponse.Broadcast.Serie.TotalEpisodes;
            }
            else
            {
                retval.SerieId = null;
                retval.SerieSeason = null;
                retval.SerieEpisode = null;
                retval.SerieEpisodeCount = null;
            }

            foreach (var file in recordingResponse.Files)
            {
                var dn = new Download();

                dn.Id = file.Id;
                dn.FileType = BongDownloadFileType.Video;
                dn.Url = file.HRef;
                dn.Quality = file.Quality;

                retval.Downloads.Add(dn.Id, dn);
            }

            if (recordingResponse.Image != null)
            {
                var dn = new Download();

                dn.Id = recordingResponse.Image.Id;
                dn.FileType = BongDownloadFileType.Image;
                dn.Url = "http://www.bong.tv" + recordingResponse.Image.HRef;
                dn.Quality = null;

                retval.Downloads.Add(dn.Id, dn);
            }

            return retval;
        }

        private Broadcast ExtractBroadcastFromResponse(DataBroadcast broadcastResponse)
        {
            var retval = new Broadcast();

            retval.Id = broadcastResponse.Id;

            retval.Title = broadcastResponse.Title;
            retval.Subtitle = broadcastResponse.Subtitle;
            var shortText = broadcastResponse.ShortText;
            var longText = broadcastResponse.LongText;
            retval.Description = ((shortText == null ? 0 : shortText.Length) < (longText == null ? 0 : longText.Length)) ? longText : shortText;
            retval.Country = broadcastResponse.Country;
            retval.ProductionYear = broadcastResponse.ProductionYear;

            retval.StartsAt = DateTime.ParseExact(broadcastResponse.StartsAtDate + " " + broadcastResponse.StartsAtTime,
                                               "dd.MM.yyyy HH:mm", _deDe, DateTimeStyles.AllowWhiteSpaces);
            retval.EndsAt = DateTime.ParseExact(broadcastResponse.StartsAtDate + " " + broadcastResponse.EndsAtTime,
                                             "dd.MM.yyyy HH:mm", _deDe, DateTimeStyles.AllowWhiteSpaces);
            if (retval.EndsAt < retval.StartsAt)
                retval.EndsAt = retval.EndsAt.AddDays(1);

            retval.ChannelId = broadcastResponse.ChannelId;
            retval.ChannelName = broadcastResponse.ChannelName;

            if (broadcastResponse.Serie != null)
            {
                retval.SerieId = broadcastResponse.SerieId;
                retval.SerieSeason = broadcastResponse.Serie.Season;
                retval.SerieEpisode = broadcastResponse.Serie.Episode;
                retval.SerieEpisodeCount = broadcastResponse.Serie.TotalEpisodes;
            }
            else
            {
                retval.SerieId = null;
                retval.SerieSeason = null;
                retval.SerieEpisode = null;
                retval.SerieEpisodeCount = null;
            }

            if (broadcastResponse.Image != null)
            {
                var dn = new Download();

                dn.Id = broadcastResponse.Image.Id;
                dn.FileType = BongDownloadFileType.Image;
                dn.Url = "http://www.bong.tv" + broadcastResponse.Image.HRef;
                dn.Quality = null;

                retval.Downloads.Add(dn.Id, dn);
            }

            return retval;
        }

        private BongResponseListBroadcasts ListBroadcastsInternal(string resourceQuery)
        {
            var request = new RestRequest(resourceQuery, Method.GET);

            WaitBetweenCalls();

            var response = _client.Execute<ListBroadcastsResponse>(request);

            StoreTimeOfLastCall();

            #region log request and response data
            var sb = new StringBuilder();
            sb.AppendLine("request.resource           = " + request.Resource);
            sb.AppendLine("request.method             = " + request.Method.ToString());
            sb.AppendLine();
            sb.AppendLine("response.StatusCode        = " + response.StatusCode.ToString());
            sb.AppendLine("response.StatusCode (num)  = " + (int)response.StatusCode);
            sb.AppendLine("response.StatusDescription = " + response.StatusDescription);
            sb.AppendLine("response.ResponseStatus    = " + response.ResponseStatus);
            sb.AppendLine("response.ErrorMessage      = " + response.ErrorMessage);
            sb.AppendLine("response.ContentEncoding   = " + response.ContentEncoding);
            sb.AppendLine("response.ContentLength     = " + response.ContentLength);
            sb.AppendLine("response.ContentType       = " + response.ContentType);
            sb.AppendLine();
            sb.AppendLine("response.Content");
            sb.AppendLine(new string('-', 80));
            sb.AppendLine(JsonHelper.FormatJson(response.Content));
            sb.AppendLine(new string('-', 80));
            sb.AppendLine();

            WebMessageLogger.WriteLogMessage("broadcasts", sb.ToString());
            #endregion

            var retval = new BongResponseListBroadcasts();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                retval.Success = true;

                foreach (var broadcast in response.Data.Broadcasts)
                {
                    var brc = ExtractBroadcastFromResponse(broadcast);
                    retval.Broadcasts.Add(brc.Id, brc);
                }

                AddBroadcastDetails(retval.Broadcasts.Values);
            }
            else
            {
                retval.Success = false;
                retval.ErrorMessage = response.ErrorMessage;
            }

            return retval;
        }

        private void AddBroadcastDetails(IEnumerable<Broadcast> broadcasts)
        {
            foreach (var broadcast in broadcasts)
            {
                var details = GetBroadcastDetails(broadcast.Id);

                if (details.Success && details.Broadcasts.Count == 1)
                {
                    var longText = details.Broadcasts[broadcast.Id].Description;

                    if ((broadcast.Description == null ? 0 : broadcast.Description.Length) < (longText == null ? 0 : longText.Length))
                        broadcast.Description = longText;
                }
            }
        }

        private void WaitBetweenCalls()
        {
            var span = DateTime.Now - _lastCall;
            int waitMs = (int)(_waitMillisecondsBetweenCalls - span.TotalMilliseconds);

            if (0 < waitMs)
            {
                System.Threading.Thread.Sleep(waitMs);
            }
        }

        private void StoreTimeOfLastCall()
        {
            _lastCall = DateTime.Now;
        }
    }
}
