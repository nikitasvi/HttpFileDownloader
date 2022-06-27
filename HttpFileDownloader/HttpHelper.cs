using MimeKit;
using System.Text.RegularExpressions;

namespace HttpFileDownloader.Core
{
    public class HttpHelper
    {
        static readonly Regex HttpRegex = new(@"^((http[s]?|ftp):\/)?\/?([^:\/\s]+)(:([^\/]*))?((\/\w+)*\/)([\w\-\.]+[^#?\s]+)(\?([^#]*))?(#(.*))?$");
        static readonly Regex ContentLengthRegex = new("\\\r\nContent-Length: (.*?)\\\r\n");
        static readonly Regex ContentTypeRegex = new("\\\r\nContent-Type: (.*?)\\\r\n");

        public static string GetDomain(string url)
        {
            Match match = HttpRegex.Match(url);
            return match.Groups[3].Value;
        }

        public static string GetFileNameFromUrl(string url)
        {
            Match match = HttpRegex.Match(url);
            return match.Groups[8].Value.Substring(0, match.Groups[8].Value.IndexOf('.'));
        }
        public static int GetContentLength(HttpResponse response)
        {
            Match match = ContentLengthRegex.Match(response.Response);
            return int.Parse(match.Groups[1].Value);
        }

        public static string GetContentType(HttpResponse response)
        {
            Match m = ContentTypeRegex.Match(response.Response);
            return m.Groups[1].Value;
        }

        public static string GetPath(string url)
        {
            Match match = HttpRegex.Match(url);
            return (match.Groups[6].Value + match.Groups[8].Value);
        }

        public static string GetExtension(string contentType)
        {
            MimeTypes.TryGetExtension(contentType, out string extension);
            return extension;
        }
    }
}
