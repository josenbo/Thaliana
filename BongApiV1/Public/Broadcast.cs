using System;
using System.Collections.Generic;
using BongApiV1.Internal;

namespace BongApiV1.Public
{
    public class Broadcast
    {
        internal BongSessionImpl Session;

        public Broadcast()
        {
            Downloads = new Dictionary<string, Download>();
        }

        public string Id { get; set; }

        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ShortText { get; set; }
        public string LongText { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string ProductionYear { get; set; }

        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }

        public string ChannelId { get; set; }
        public string ChannelName { get; set; }

        public string SerieId { get; set; }
        public string SerieSeason { get; set; }
        public string SerieEpisode { get; set; }
        public string SerieEpisodeCount { get; set; }

        public Dictionary<string, Download> Downloads { get; set; }

        public void Record()
        {
            Session.CreateRecording(Id);    
        }
    }
}
