using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongApiV1.WebServiceImplementation
{
    public class DataRecording
    {
        public string Id { get; set; }
        public string BroadcastId { get; set; }
        public string Title { get; set; }

        public string StartsAtMs { get; set; }
        public string StartsAtDate { get; set; }
        public string StartsAtTime { get; set; }
        public string EndsAtTime { get; set; }
        
        public string ChannelId { get; set; }
        public string Status { get; set; }
        public string Quality { get; set; }
        public string Version { get; set; }
        
        public bool BroadcastWasDeleted { get; set; }

        public List<DataRecordingFile> Files { get; set; }
        
        public DataImage Image { get; set; }
        
        public List<DataCategory> Categories { get; set; }
        
        public DataBroadcast Broadcast { get; set; }
    }
}
