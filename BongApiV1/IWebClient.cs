using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongApiV1
{
    public interface IWebClient
    {
        WebClientResponse Execute(WebClientRequest request, Type responseContentType);
    }
}
