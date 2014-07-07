using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongApiV1
{
    public class BongException : Exception
    {
        public BongException() { }
        public BongException(string message) : base(message) { }
    }

    public class BongAuthException : BongException
    {
        public BongAuthException() : base("You are not logged into the Bong service") { }
        public BongAuthException(string message) : base(message) { }
    }
}
