using System;

namespace BongApiV1.WebServiceContract
{
    public interface IBongClient
    {
        WebClientResponse Execute(WebClientRequest request, Type responseContentType);



    }
}
