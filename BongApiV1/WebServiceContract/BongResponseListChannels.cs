using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BongApiV1.Public;

namespace BongApiV1.WebServiceContract
{
    public class BongResponseListChannels : BongResponse
    {
        public BongResponseListChannels()
        {
            Channels = new Dictionary<string, Channel>();
        }

        public Dictionary<string, Channel> Channels { get; set; }
    }
}
