using System.Collections.Generic;
using BongApiV1.Public;
using BongApiV1.WebServiceContract;

namespace BongApiV1.Internal
{
    internal class BongSessionImpl
    {
        public IBongClient BongClientImpl { get; set; }
        public List<Recording> Recordings { get; set; }
        public List<Channel> Channels { get; set; }
        public List<Broadcast> Broadcasts { get; set; }

        public BongSessionImpl(IBongClient bongClient)
        {
            BongClientImpl = bongClient;
            Recordings = new List<Recording>();
            Channels= new List<Channel>();
            Broadcasts = new List<Broadcast>();
        }
    }
}
