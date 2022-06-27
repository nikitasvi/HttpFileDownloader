using System.Net;
using System.Text.RegularExpressions;
using HttpFileDownloader.Core;

namespace HttpFileDownloader.Console
{
    public class HttpFileDownloader
    {
        public static void Main(string[] args)
        {
            HttpDownloader httpDownloader = new HttpDownloader(
                ConfigurationManager.AppSettings[]);
        }
    }
}