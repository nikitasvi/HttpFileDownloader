using HttpFileDownloader.Core;
using NUnit.Framework;
using System.Collections.Generic;
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
            HttpDownloader downloader = new HttpDownloader(10, 1024, 2097152);

            //downloader.Download("http://212.183.159.230/5MB.zip");
            downloader.Download("https://www.sample-videos.com/img/Sample-jpg-image-30mb.jpg");
            //downloader.Download("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQsXd43Poo6umRXlkbgmIoJhe6hsHNAhPQjzQ&usqp=CAU");
        }

        [Test]
        public void MarkRegionTest()
        {
            var regionsTest = new List<Region>();
            regionsTest.Add(new Region(0, 200, State.Planned));
            regionsTest.Add(new Region(200, 300, State.Free));

            var dm = new DownloadMap(500);
            dm.MarkRegion(0, 200, State.Planned);
            Assert.AreEqual(regionsTest, dm.GetRegions());
        }

        [Test]
        public void ExtensionTest()
        {
            HttpHelper httpHelper = new HttpHelper();
            var ext = HttpHelper.GetExtension("application/zip");
            Assert.AreEqual(".zip", ext);
        }

        [Test]
        public void FileNameTest()
        {
            HttpHelper httpHelper = new HttpHelper();
            string fileName = HttpHelper.GetFileNameFromUrl("https://www.sample-videos.com/img/Sample-jpg-image-30mb.jpg");
            Assert.AreEqual("Sample-jpg-image-30mb", fileName);
        }
    }
}