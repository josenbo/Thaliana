using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BongApiV1.Public;

namespace BongApiV1.WebServiceContract
{
    public class BongResponseSearchBroadcasts : BongResponse
    {
        public BongResponseSearchBroadcasts()
        {
            Broadcasts = new Dictionary<string, Broadcast>();
        }

        public Dictionary<string, Broadcast> Broadcasts { get; set; }
    }
}
