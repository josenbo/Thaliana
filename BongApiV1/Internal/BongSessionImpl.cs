using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BongApiV1.Public;
using BongApiV1.WebServiceContract;

namespace BongApiV1.Internal
{
    internal class BongSessionImpl
    {
        private readonly IBongClient _bongClientImpl;
        private readonly string _loggingDirectory;
        private StreamWriter _logFile;

        internal Dictionary<string, Recording> Recordings { get; set; }
        internal Dictionary<string, Channel> Channels { get; set; }

        internal string BongUsername { get { return _bongClientImpl.Username; } }
        internal string BongPassword { get { return _bongClientImpl.Password; } }

        internal BongSessionImpl(string username, string password, string loggingDirectory, IBongClient bongClient)
        {
            _bongClientImpl = bongClient;
            _bongClientImpl.Username = username;
            _bongClientImpl.Password = password;
            
            if (_loggingDirectory != null)
            {
                var filename = string.Format("Log_{0:yyyyMMdd_HHmm}_BongSessionImpl.log", DateTime.Now);
                _logFile = new StreamWriter(Path.Combine(_loggingDirectory, filename));

                _loggingDirectory = loggingDirectory;
            }

            var response = _bongClientImpl.LoginUser();

            if (!response.Success)
                throw new BongException(response.ErrorMessage);

            RefreshChannels();
            RefreshRecordings();
        }

        internal void Close()
        {
            if (_logFile == null) return;
            _logFile.Close();
            _logFile = null;
        }

        internal void RefreshRecordings()
        {
            var response = _bongClientImpl.ListRecordings();

            if (!response.Success) 
                throw new BongException(response.ErrorMessage);

            AddSessionInstance(response.Recordings);

            AddChannelNames(response.Recordings);

            Recordings = response.Recordings;
        }

        internal void RefreshChannels()
        {
            var response = _bongClientImpl.ListChannels();

            if (!response.Success)
                throw new BongException(response.ErrorMessage);

            AddSessionInstance(response.Channels);

            Channels = response.Channels;
        }

        internal void CreateRecording(string broadcastId)
        {
            var response = _bongClientImpl.CreateRecording(broadcastId);

            if (!response.Success)
                throw new BongException(response.ErrorMessage);

            RefreshRecordings();
        }

        internal void DeleteRecording(string recordingId)
        {
            var response = _bongClientImpl.DeleteRecording(recordingId);

            if (!response.Success)
                throw new BongException(response.ErrorMessage);

            RefreshRecordings();
        }

        internal Dictionary<string, Broadcast> GetTodaysBroadcastsByChannel(string channelId)
        {
            var response = _bongClientImpl.ListBroadcasts(channelId);

            if (!response.Success)
                throw new BongException(response.ErrorMessage);

            AddSessionInstance(response.Broadcasts);

            AddChannelNames(response.Broadcasts);

            return response.Broadcasts;
        }

        internal Dictionary<string, Broadcast> GetBroadcastsByChannelAndDate(string channelId, DateTime date)
        {
            var response = _bongClientImpl.ListBroadcasts(channelId, date);

            if (!response.Success)
                throw new BongException(response.ErrorMessage);

            AddSessionInstance(response.Broadcasts);

            AddChannelNames(response.Broadcasts);

            return response.Broadcasts;
        }

        internal Dictionary<string, Broadcast> GetAllBroadcastsByChannel(string channelId)
        {
            var currentDate = DateTime.Today;
            var retval = new Dictionary<string, Broadcast>();

            while (true)
            {
                var response = _bongClientImpl.ListBroadcasts(channelId, currentDate);

                if (!response.Success)
                    throw new BongException(response.ErrorMessage);

                if (!response.Broadcasts.Any())
                    break;

                foreach (var broadcast in response.Broadcasts.Values.Where(broadcast => !retval.ContainsKey(broadcast.Id)))
                {
                    retval.Add(broadcast.Id, broadcast);
                }

                currentDate = currentDate.AddDays(1);
            }

            AddSessionInstance(retval);

            AddChannelNames(retval);

            return retval;
        }

        internal Dictionary<string, Broadcast> GetBroadcastsByQueryString(string query)
        {
            var response = _bongClientImpl.SearchBroadcasts(query);

            if (!response.Success)
                throw new BongException(response.ErrorMessage);

            AddChannelNames(response.Broadcasts);

            return response.Broadcasts;
        }

        private void AddChannelNames(Dictionary<string, Recording> recordings)
        {
            foreach (var recording in recordings.Where(recording => string.IsNullOrWhiteSpace(recording.Value.ChannelName)))
            {
                if (Channels.ContainsKey(recording.Value.ChannelId))
                    recording.Value.ChannelName = Channels[recording.Value.ChannelId].Name;    
                else
                    recording.Value.ChannelName = String.Format("[{0}]", recording.Value.ChannelId);
            }
        }

        private void AddChannelNames(Dictionary<string, Broadcast> broadcasts)
        {
            foreach (var broadcast in broadcasts.Where(broadcast => string.IsNullOrWhiteSpace(broadcast.Value.ChannelName)))
            {
                if (Channels.ContainsKey(broadcast.Value.ChannelId))
                    broadcast.Value.ChannelName = Channels[broadcast.Value.ChannelId].Name;
                else
                    broadcast.Value.ChannelName = String.Format("[{0}]", broadcast.Value.ChannelId);
            }
        }

        private void AddSessionInstance(Dictionary<string, Recording> recordings)
        {
            foreach (var recording in recordings.Values)
            {
                recording.Session = this;
            }
        }

        private void AddSessionInstance(Dictionary<string, Broadcast> broadcasts)
        {
            foreach (var broadcast in broadcasts.Values)
            {
                broadcast.Session = this;
            }
        }

        private void AddSessionInstance(Dictionary<string, Channel> channels)
        {
            foreach (var channel in channels.Values)
            {
                channel.Session = this;
            }
        }

        internal void WriteLog(string format, params Object[] args)
        {
            if (_logFile != null) _logFile.Write(String.Format(format, args) + "\n");
        }

        #region some methods used to check out service behaviour
        //internal void TestBroadcasts2()
        //{
        //    var response = BongClientImpl.GetBroadcastDetails("2235713");

        //    if (!response.Success)
        //        throw new BongException(response.ErrorMessage);

        //    Broadcasts = response.Broadcasts;
        //}

        //internal void TestCreateRecording()
        //{
        //    // recordingID  broadcastId   Titel                                    Start
        //    //    13210463      2242505   "Lotto am Samstag"            12.07.2014 19:57
        //    //    13210489      2235721   "Die mit dem Bauch tanzen"    08.07.2014 22:45
        //    var response = BongClientImpl.CreateRecording( "2235721");

        //    if (!response.Success)
        //        throw new BongException(response.ErrorMessage);
        //}

        //internal void TestDeleteRecording()
        //{
        //    // recordingID  broadcastId   Titel                                    Start
        //    //    13210463      2242505   "Lotto am Samstag"            12.07.2014 19:57
        //    //    13210489      2235721   "Die mit dem Bauch tanzen"    08.07.2014 22:45
        //    var response = BongClientImpl.DeleteRecording("13210489");

        //    if (!response.Success)
        //        throw new BongException(response.ErrorMessage);
        //}
	    #endregion

    }
}
