using System.Text;

namespace HttpFileDownloader.Core
{
    public class HttpProtocol
    {
        public byte[] CreateHttpRequest(HttpRequest request)
        {
            return Encoding.UTF8.GetBytes($"{request.Method} {request.Url} HTTP/1.1\r\nHost: {request.Host}\r\n\r\n");
        }

        public HttpResponse GetHEADResponse(byte[] buffer)
        { 
            HttpResponse response = new HttpResponse
            {
                Response = string.Empty,
            };

            for (int i = 0; i < buffer.Length; i++)
            {
                if (response.Response.Contains("\r\n\r\n"))
                {
                    break;
                }

                response.Response += Encoding.UTF8.GetString(buffer, i, 1);
            }

            return response;
        }
    }
}
