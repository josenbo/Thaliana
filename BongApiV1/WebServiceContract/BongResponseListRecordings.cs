using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BongApiV1.Public;

namespace BongApiV1.WebServiceContract
{
    public class BongResponseListRecordings : BongResponse
    {
        public BongResponseListRecordings()
        {
            Recordings = new Dictionary<string, Recording>();
        }

        public Dictionary<string, Recording> Recordings { get; set; }
    }
}
