using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongApiV1
{
    internal class BongSessionImpl
    {
        public IWebClient WebClientImpl { get; set; }
        public List<Recording> Recordings { get; set; }
        public List<Channel> Channels { get; set; }
        public List<Broadcast> Broadcasts { get; set; }

        public BongSessionImpl(IWebClient webClient)
        {
            WebClientImpl = webClient;
            Recordings = new List<Recording>();
            Channels= new List<Channel>();
            Broadcasts = new List<Broadcast>();
        }
    }
}
