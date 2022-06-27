namespace HttpFileDownloader.Core
{
    public interface IDownloadMap
    {

        public void MarkRegion(long offset, long length, State state);

        List<Region> GetRegions();

        bool IsDownloaded();
    }
}
