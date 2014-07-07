using System;
using System.Collections.Generic;
using BongApiV1.Internal;
using BongApiV1.WebServiceContract;
using BongApiV1.WebServiceImplementation;

namespace BongApiV1.Public
{
    public class BongSession
    {
        private readonly BongSessionImpl _session;

        public BongSession(string username, string password, IBongClient bongClient = null)
        {
            _session = new BongSessionImpl(bongClient ?? new BongClientDefault("http://www.bong.tv"));

            //var request = new WebClientRequest();
            //request.Url = "api/v1/user_sessions.json";
            //request.HttpVerb = "POST";
            //request.Parameters.Add("login", username);
            //request.Parameters.Add("password", password);

            //var response = new WebClientResponse();
            
            //_session.WebClientImpl.Execute(request, response);
            
            //if (response.HttpStatusCode != HttpStatusCode.OK)
            //    throw new BongAuthException();
        }

        public IEnumerable<Recording> Recordings 
        { 
            get
            {
                if (_session == null) throw new BongAuthException(); 
                return _session.Recordings.AsReadOnly();
            } 
        }

        public IEnumerable<Channel> Channels 
        {
            get
            {
                if (_session == null) throw new BongAuthException();
                return _session.Channels.AsReadOnly();
            }
        }

        public IEnumerable<Broadcast> FindBroadcasts(Channel channel, DateTime? date = null)
        {
            if (_session == null) throw new BongAuthException();
            return _session.Broadcasts.AsReadOnly();
        }
    }
}
