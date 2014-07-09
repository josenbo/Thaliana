using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongApiV1.Public
{
    public class Download
    {
        public string Id { get; set; }
        public BongDownloadFileType FileType { get; set; }
        public string Url { get; set; }
        public string Quality { get; set; }
    }
}
