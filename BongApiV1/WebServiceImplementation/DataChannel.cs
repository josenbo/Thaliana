using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongApiV1.WebServiceImplementation
{
    public class DataChannel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Recordable { get; set; }
        public bool Hd { get; set; }
        public string HRef { get; set; }
        public string Group { get; set; }
        public string Position { get; set; }
    }
}
