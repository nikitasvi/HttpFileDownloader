namespace HttpFileDownloader.Core
{
    public class DownloadStrategy
    {
        public static void ChooseStrategy(int maxSize, int maxCount, DownloadMap downloadMap)
        {
            List<Region> regions = downloadMap.GetRegions();
            
            while (regions.FindAll(x => x.State == State.Planned).Count + regions.FindAll(x => x.State == State.InProcess).Count < maxCount)
            {
                Region freeBlock = regions.Find(x => x.State == State.Free);

                if (freeBlock != null)
                {
                    if (freeBlock.Length < maxSize)
                    {
                        freeBlock.State = State.Planned;
                    }
                    else
                    {
                        if (freeBlock.Length > 2 * maxSize)
                        {
                            downloadMap.MarkRegion(freeBlock.Start, maxSize, State.Planned);
                        }
                        else
                        {
                            downloadMap.MarkRegion(freeBlock.Start, freeBlock.Length / 2, State.Planned);
                        }
                    }
                    regions = downloadMap.GetRegions();
                }
                else
                {
                    break;
                }   
            }
            ConcatDownloadedRegions(downloadMap);
        }

        private static void ConcatDownloadedRegions(DownloadMap downloadMap)
        {
            var regions = downloadMap.GetRegions();
            var downloadedRegions = downloadMap.GetRegions().FindAll(x => x.State == State.Downloaded);

            for (int i = 1; i < downloadedRegions.Count; i++)
            {
                if (downloadedRegions[i].Start == (downloadedRegions[i - 1].Start + downloadedRegions[i - 1].Length))
                {
                    downloadedRegions[i - 1].Length += downloadedRegions[i].Length;

                    regions.Remove(downloadedRegions[i]);
                    downloadedRegions.Remove(downloadedRegions[i]);
                    //downloadMap.MarkRegion(downloadedRegions[i].Start, downloadedRegions[i].Length, State.Downloaded);

                    i--;
                    regions = downloadMap.GetRegions();
                }
            }
        }
    }
}
