using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpFileDownloader.Core
{
    public class HttpProtocol
    {
        public static byte[] CreateHttpRequest(HttpRequest request)
        {
            IPAddress ip = NetUtil.ResolveIpAddress(HttpHelper.GetDomain(request.Url));
            return Encoding.ASCII.GetBytes($"{request.Method} {request.Url} HTTP/1.1\r\nHost: {ip}\r\n\r\n");
        }

        public static byte[] CreateHttpRequest(HttpRequest request, long start, long end)
        {
            IPAddress ip = NetUtil.ResolveIpAddress(HttpHelper.GetDomain(request.Url));
            return Encoding.ASCII.GetBytes($"{request.Method} {HttpHelper.GetPath(request.Url)} HTTP/1.1\r\nHost: {ip}\r\nRange: bytes={start}-{end}\r\n\r\n");
        }


        public static HttpResponse GetResponse(Socket socket)
        { 
            HttpResponse response = new HttpResponse
            {
                Response = string.Empty,
            };

            var sb = new StringBuilder();

            do
            {
                byte[] buffer = new byte[1];
                socket.Receive(buffer, 0, 1, 0);
                sb.Append(Encoding.ASCII.GetString(buffer));
            }
            while (!sb.ToString().Contains("\r\n\r\n"));

            response.Response = sb.ToString();

            return response;
        }
    }
}
