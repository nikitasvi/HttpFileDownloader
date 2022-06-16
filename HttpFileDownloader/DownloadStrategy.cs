using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpFileDownloader.Core
{
    public class DownloadStrategy : IDownloadStrategy
    {
        public int maxCount;
        public int minSize;
        public int maxSize;

        HttpProtocol protocol = new();
        readonly Regex HttpRegex = new(@"^((http[s]?|ftp):\/)?\/?([^:\/\s]+)(:([^\/]*))?((\/\w+)*\/)([\w\-\.]+[^#?\s]+)(\?([^#]*))?(#(.*))?$");
        readonly Regex ContentLengthRegex = new("\\\r\nContent-Length: (.*?)\\\r\n");

        public DownloadStrategy(int maxThreadsCount, int minPartSize, int maxPartSize)
        {
            this.maxCount = maxThreadsCount;
            this.minSize = minPartSize;
            this.maxSize = maxPartSize;
        }

        public void Download(string url)
        { 
            Match mat = HttpRegex.Match(url);

            var ipAddress = NetUtil.ResolveIpAddress(mat.Groups[3].Value);
            IPEndPoint endPoint = new(ipAddress, 80);

            byte[] buffer = SendHEADRequest(endPoint, url, ipAddress, protocol);

            HttpResponse response = protocol.GetHEADResponse(buffer);

            
            Match m = ContentLengthRegex.Match(response.Response);
            int contentLength = int.Parse(m.Groups[1].ToString());

            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);

            HttpRequest GetRequest = new HttpRequest
            {
                Method = "GET",
                Url = url,
                Host = ipAddress,
            };

            socket.Send(protocol.CreateHttpRequest(GetRequest));

            byte[] bodyBuff = new byte[contentLength];
            socket.Receive(bodyBuff, 0, contentLength, 0);

            using (var stream = new FileStream("file.zip", FileMode.Create))
            {
                stream.Write(bodyBuff, 0, bodyBuff.Length);
            }

            Encoding.UTF8.GetString(bodyBuff);

            socket.Close();
        }


        private byte[] SendHEADRequest(IPEndPoint endPoint, string url, IPAddress ipAddress, HttpProtocol protocol)
        {
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);

            HttpRequest HeadRequest = new HttpRequest
            {
                Method = "HEAD",
                Url = url,
                Host = ipAddress,
            };

            socket.Send(protocol.CreateHttpRequest(HeadRequest));

            byte[] buffer = new byte[2048];

            socket.Receive(buffer);
            socket.Close();

            return buffer;
        }
    }
}
