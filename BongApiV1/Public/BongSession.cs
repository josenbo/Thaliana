using System;
using System.Collections.Generic;
using BongApiV1.Internal;
using BongApiV1.WebServiceContract;
using BongApiV1.WebServiceImplementation;

namespace BongApiV1.Public
{
    /// <summary>
    /// Distinguish between downloadable shows already recorded and those waiting for broadcast time
    /// </summary>
    public enum BongRecordingState
    {
        Recorded,       // Meine Aufnahmen
        Queued,         // Vorschau des Serienmanagers
        Scheduled       // Programmierte Sendungen
    }

    /// <summary>
    /// File type for downloadable items
    /// </summary>
    public enum BongDownloadFileType
    {
        Video,
        Image
    }


    /// <summary>
    /// Entry point for data and methods made available 
    /// by the Bong.tv recording service
    /// </summary>
    public class BongSession
    {
        private readonly BongSessionImpl _session;

        /// <summary>
        /// Initializes a session with the Bong.tv web service  
        /// and refreshes the list of recordings
        /// </summary>
        /// <param name="username">The Bong.tv user's account name</param>
        /// <param name="password">The Bong.tv user's password</param>
        /// <param name="bongClient">usually omitted or null to use the default Bong REST-Client. 
        /// For testing purposes a mock interface implementation may be passed in</param>
        /// <param name="waitMillisecondsBetweenCalls"></param>
        /// <exception cref="BongApiV1.Public.BongException">
        /// Thrown when authentication fails or request cannot be processed by the web service
        /// </exception>
        public BongSession(string username, string password, IBongClient bongClient = null, long waitMillisecondsBetweenCalls = 0)
        {
            _session = new BongSessionImpl(username, password, bongClient ?? new BongClientRestSharp(waitMillisecondsBetweenCalls));
        }

        public IEnumerable<Recording> Recordings { get{ return _session.Recordings.Values; }}

        public IEnumerable<Channel> Channels { get{ return _session.Channels.Values; }}

        public IEnumerable<Broadcast> SearchBroadcasts(string query)
        {
            return _session.GetBroadcastsByQueryString(query).Values;
        }
    }
}
