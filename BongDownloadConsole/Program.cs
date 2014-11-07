using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongDownloadConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // var conn = new EntityConnection("name=Entities");
            // conn.Open();

            using (var ctx = new BongRecordings())
            {
                foreach (var r in ctx.Recording)
                {
                   Console.WriteLine("[{0}] {1}", r.RecordingId, r.Title);
                }

                var neu = new Recording();

                neu.RecordingId = 1;
                neu.BongRecordingId = "1R";
                neu.BongBroadcastId = "1B";
                neu.Title = "Zugeschaut und mitgebaut";
                neu.StartTime = new DateTime(2014, 7, 11, 8, 15, 45);
                neu.EndTime = new DateTime(2014, 7, 11, 9, 50, 8);
                neu.BongChannelId = "1";
                neu.BongChannelName = "ARD";
                neu.ProcessingState = ProcessingStateEnum.Geloescht;

                ctx.Recording.Add(neu);

                // ctx.Recording.Remove(ctx.Recording.Find(1));

                ctx.SaveChanges();
            }
            
        }
    }
}
