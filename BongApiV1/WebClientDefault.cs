using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace BongApiV1
{
    class WebClientDefault : IWebClient
    {
        private readonly RestClient _client;

        public WebClientDefault(string baseUrl)
        {
            _client = new RestClient(baseUrl) {CookieContainer = new System.Net.CookieContainer()};
        }

        public WebClientResponse Execute(WebClientRequest request, Type responseContentType)
        {
            RestSharp.Method method;

            switch (request.HttpVerb)
            {
                case "GET":
                    method = Method.GET;
                    break;
                case "POST":
                    method = Method.POST;
                    break;
                case "DELETE":
                    method = Method.DELETE;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var req = new RestRequest(request.Url, method);
            req.RequestFormat = DataFormat.Json;
            req.AddBody(request.Parameters);

            RestResponse<responseContentType> rsp = _client.Execute<responseContentType>(req);

            // response.HttpStatusCode = rsp.StatusCode
        }
    }
}
