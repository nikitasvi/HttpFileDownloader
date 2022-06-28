using System.Net;
using System.Net.Sockets;

namespace HttpFileDownloader.Core
{
    public class HttpDownloader
    {
        private int maxCount;
        private int minSize;
        private int maxSize;

        private string url;

        IPAddress iPAddress;
        IPEndPoint iPEndPoint;
        DownloadMap downloadMap;

        public HttpDownloader(int maxThreadsCount, int minPartSize, int maxPartSize)
        {
            this.maxCount = maxThreadsCount;
            this.minSize = minPartSize;
            this.maxSize = maxPartSize;
        }

        public void Download(string url)
        {
            this.url = url;

            // Get IP Address from url
            this.iPAddress = NetUtil.ResolveIpAddress(HttpHelper.GetDomain(url));
            this.iPEndPoint = new IPEndPoint(iPAddress, 80);

            // Create new connect
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(iPEndPoint);

            // Create new Head Request
            HttpRequest headRequest = new HttpRequest
            {
                Method = HttpMethod.Head,
                Url = url,
            };

            socket.Send(HttpProtocol.CreateHttpRequest(headRequest));

            // Get Head Response and parse Content-Length/Content Type
            HttpResponse response = HttpProtocol.GetResponse(socket);
            int contentLength = HttpHelper.GetContentLength(response);
            string contentType = HttpHelper.GetContentType(response);

            this.downloadMap = new DownloadMap(contentLength);

            FileWriter.CreateFile(HttpHelper.GetFileNameFromUrl(url), HttpHelper.GetExtension(contentType));

            while (!this.downloadMap.IsDownloaded())
            {
                DownloadStrategy.ChooseStrategy(this.maxSize, this.maxCount, this.downloadMap);

                List<Region> regions = this.downloadMap.GetRegions().FindAll(x => x.State == State.Planned);

                if (regions.Count > 0)
                {
                    foreach (Region region in regions)
                    {
                        this.downloadMap.MarkRegion(region.Start, region.Length, State.InProcess);
                        Thread thread = new Thread(() => DownloadPart(region));
                        thread.Start();

                        //DownloadPart(region);
                    }
               }
            }
        }

        private void DownloadPart(Region region)
        {
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(iPEndPoint);

            HttpRequest getRequest = new HttpRequest
            {
                Method = HttpMethod.Get,
                Url = this.url,
            };

            byte[] buffer = HttpProtocol.CreateHttpRequest(getRequest, region.Start, region.Start + region.Length);
            socket.Send(buffer);

            _ = HttpProtocol.GetResponse(socket);

            byte[] receiveBytes = new byte[region.Length];

            int bytesReceived = 0;
            int offset = 0;

            while (offset != region.Length)
            {
                bytesReceived = socket.Receive(receiveBytes, offset, (int)(region.Length - offset), SocketFlags.None);
                offset += bytesReceived;
            }
            //Console.Clear();
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("Download Speed:" + Math.Round((double)bytesReceived / 1024, 2) + "KB/s");
            FileWriter.Write(receiveBytes, (int)region.Start);
            this.downloadMap.MarkRegion(region.Start, region.Length, State.Downloaded);

            socket.Close();
        } 
    }
}
