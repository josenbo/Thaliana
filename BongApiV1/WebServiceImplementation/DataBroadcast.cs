using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongApiV1.WebServiceImplementation
{
    public class DataBroadcast
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ShortText { get; set; }
        public string LongText { get; set; }
        public string Subtitle { get; set; }
        public string Country { get; set; }
        public string ProductionYear { get; set; }
        public bool Hd { get; set; }

        public string ChannelId { get; set; }
        public string ChannelName { get; set; }

        public string StartsAtMs { get; set; }
        public string StartsAtDate { get; set; }
        public string StartsAtTime { get; set; }
        public string EndsAtTime { get; set; }
        public string EndsAtMs { get; set; }
        public string Duration { get; set; }

        public List<DataCategory> Categories { get; set; }

        public string SerieId { get; set; }
        public DataBroadcastSerie Serie { get; set; }

        public DataImage Image { get; set; }
    }
}
