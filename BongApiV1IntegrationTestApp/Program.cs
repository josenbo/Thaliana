using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BongApiV1;

namespace BongApiV1IntegrationTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var session = new BongSession("stein", "ambo4");
        }
    }
}
