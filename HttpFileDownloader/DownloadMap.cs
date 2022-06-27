using System.Runtime.CompilerServices;

namespace HttpFileDownloader.Core
{
    public record Region
    {
        public long Start { get; set; }
        public long Length { get; set; }
        public State State { get; set; }

        public Region(long start, long length, State state)
        {
            this.Start = start;
            this.Length = length;   
            this.State = state;
        }
    };

    public enum State
    {
        Free, Planned, Downloaded, InProcess
    }

    public class DownloadMap : IDownloadMap
    {
        List<Region> regions;
        public DownloadMap(long size)
        {
            this.regions = new List<Region>();
            this.regions.Add(new Region(0, size, State.Free));
        }

        public DownloadMap(List<Region> regions)
        {
            this.regions = regions;
        }

        public List<Region> GetRegions()
        {
            return this.regions;
        }

        public bool IsDownloaded()
        {
            if (this.regions.Count == 1 && this.regions.First().State == State.Downloaded)
                return true;
            else return false;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void MarkRegion(long offset, long length, State state)
        {
            var region = this.regions.Find(x => x.Start == offset);

            if (region.Start == offset && region.Length == length)
            {
                region.State = state;
            }
            else
            {
                if (region != null)
                    this.regions.Remove(region);

                this.regions.Add(new Region(offset, length, state));
                this.regions.Add(new Region(offset + length, region.Length - length, State.Free));

                //this.regions = this.regions.OrderBy(x => x.Start).ToList();
            }
        }
    }
}
