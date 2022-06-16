using System.Net;

namespace HttpFileDownloader.Core
{
    public class HttpRequest
    {
        public string Url { get; set; }

        public IPAddress Host { get; set; }

        public string Method { get; set; }
    }
}
