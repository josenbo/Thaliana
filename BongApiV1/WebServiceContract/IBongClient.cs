using System;
using System.Collections.Generic;

namespace BongApiV1.WebServiceContract
{
    /// <summary>
    /// This interfaces defines methods provided by the Bong REST web service. 
    /// The REST Client implementation can be replaced by a mock implementation 
    /// for (unit) testing purposes.
    /// </summary>
    public interface IBongClient
    {
        /// <summary>
        /// The web service user account name
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// The web service user password
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Log on to the web service using the name and password property values
        /// </summary>
        /// <returns>Response object returning server supplied information upon success or error message upon failure</returns>
        BongResponseLoginUser LoginUser();

        /// <summary>
        /// Log off the web service and free resources
        /// </summary>
        void Close();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        BongResponseListRecordings ListRecordings();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="broadcastId"></param>
        /// <returns></returns>
        BongResponseCreateRecording CreateRecording(string broadcastId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordingId"></param>
        /// <returns></returns>
        BongResponseDeleteRecording DeleteRecording(string recordingId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        BongResponseListChannels ListChannels();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        BongResponseListBroadcasts ListBroadcasts(string channelId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        BongResponseListBroadcasts ListBroadcasts(string channelId, DateTime date);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="broadcastId"></param>
        /// <returns></returns>
        BongResponseGetBroadcastDetails GetBroadcastDetails(string broadcastId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        BongResponseSearchBroadcasts SearchBroadcasts(string searchPattern);
    }
}
