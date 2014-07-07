using System.Collections.Generic;

namespace BongApiV1
{
    public class WebClientRequest
    {
        public string Url { get; set; }
        public string HttpVerb { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public WebClientRequest()
        {
            Parameters = new Dictionary<string, string>();
        }
    }
}