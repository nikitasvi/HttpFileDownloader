using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Text.RegularExpressions;
using HttpFileDownloader.Core;

namespace HttpFileDownloader.Console
{
    public class HttpFileDownloader
    {
        public static void Main(string[] args)
        {
            string url = "http://212.183.159.230/5MB.zip";
            HttpProtocol protocol = new HttpProtocol();

            Regex regex = new Regex(@"^((http[s]?|ftp):\/)?\/?([^:\/\s]+)(:([^\/]*))?((\/\w+)*\/)([\w\-\.]+[^#?\s]+)(\?([^#]*))?(#(.*))?$");
            Match mat = regex.Match(url);

            var ipAddress = NetUtil.ResolveIpAddress(mat.Groups[3].Value);
            IPEndPoint endPoint = new(ipAddress, 80);

            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);

            HttpRequest HeadRequest = new HttpRequest
            {
                Method = "HEAD",
                Url = url,
                Host = ipAddress,
            };

            int responseLength = socket.Send(protocol.CreateHttpRequest(HeadRequest));

            byte[] buffer = new byte[2048];
            socket.Receive(buffer);
            socket.Close();
            HttpResponse response = protocol.GetHEADResponse(buffer);

            Regex reg = new Regex("\\\r\nContent-Length: (.*?)\\\r\n");
            Match m = reg.Match(response.Response);
            int contentLength = int.Parse(m.Groups[1].ToString());

            Socket sock = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(endPoint);

            HttpRequest GetRequest = new HttpRequest
            {
                Method = "GET",
                Url = url,
                Host = ipAddress,
            };

            sock.Send(protocol.CreateHttpRequest(GetRequest));

            byte[] bodyBuff  = new byte[contentLength];
            sock.Receive(bodyBuff, 0, contentLength, 0);

            using (var stream = new FileStream("file.zip", FileMode.Create))
            {
                stream.Write(bodyBuff, 0, bodyBuff.Length);
            }

            string body = Encoding.UTF8.GetString(bodyBuff);
            
            socket.Close();
        }
    }
}