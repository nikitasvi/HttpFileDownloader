namespace HttpFileDownloader.Console
{
    using HttpFileDownloader.Core;
    using System;
    using System.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Enter Url of File: ");
            }

            HttpDownloader httpDownloader = new HttpDownloader(
                Convert.ToInt32(ConfigurationManager.AppSettings["MaxThreadsCount"]),
                Convert.ToInt32(ConfigurationManager.AppSettings["MinSize"]),
                Convert.ToInt32(ConfigurationManager.AppSettings["MaxSize"]));

            try
            {
                Console.WriteLine("Start Downloading...  ");
                httpDownloader.Download(args[0]);
            }
            finally
            {

                Console.WriteLine("\nDownload Complete!");
            }
        }
    }
}