using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BongApiV1
{
    public class BongSession
    {
        private readonly BongSessionImpl _session;

        public BongSession(string username, string password, IWebClient webClient = null)
        {
            _session = new BongSessionImpl(webClient ?? new WebClientDefault("http://www.bong.tv"));

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
