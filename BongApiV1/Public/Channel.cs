using System;
using System.Collections.Generic;
using BongApiV1.Internal;

namespace BongApiV1.Public
{
    public class Channel
    {
        internal BongSessionImpl Session;

        public string Id { get; set; }
        public string Name { get; set; }
        public bool Recordable { get; set; }
        public bool Hd { get; set; }

        public IEnumerable<Broadcast> GetTodaysBroadcasts()
        {
            return Session.GetTodaysBroadcastsByChannel(Id).Values;
        }

        public IEnumerable<Broadcast> GetBroadcastsByDate(DateTime date)
        {
            return Session.GetBroadcastsByChannelAndDate(Id, date).Values;
        }

        public IEnumerable<Broadcast> GetAllAvailableBroadcasts()
        {
            return Session.GetAllBroadcastsByChannel(Id).Values;
        }
    }
}
