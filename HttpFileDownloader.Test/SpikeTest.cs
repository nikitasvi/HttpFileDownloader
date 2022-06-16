using HttpFileDownloader.Core;
using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace HttpFileDownloader.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            DownloadStrategy downloadStrategy = new DownloadStrategy(10, 2048, 8096);

            downloadStrategy.Download("http://212.183.159.230/5MB.zip");
        }

    }
}