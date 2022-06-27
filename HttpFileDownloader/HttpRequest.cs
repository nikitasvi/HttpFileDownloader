using System.Net;

namespace HttpFileDownloader.Core
{
    public class HttpRequest
    {
        public string Url { get; set; }

        public HttpMethod Method { get; set; }
    }
}
