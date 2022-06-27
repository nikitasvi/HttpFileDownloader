namespace HttpFileDownloader.Core
{
    public class FileWriter
    {
        private static string downloadFolder = Path.Combine(System.Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads");
        private static string filePath;
        public static void Write(byte[] data, int start)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var locker = new RWLock();
                using (locker.WriteLock())
                {
                    stream.Seek(start, SeekOrigin.Begin);
                    stream.Write(data, 0, data.Length);
                }
            }
        }

        public static void CreateFile(string fileName, string extension)
        {
            int count = 1;
            bool IsExist = true;

            filePath = Path.Combine(downloadFolder, fileName + extension);

            while (IsExist)
            {
                if (File.Exists(filePath))
                {
                    filePath = Path.Combine(downloadFolder, fileName + " (" + count + ")" + extension);
                    count++;
                }
                else
                {
                    IsExist = false;
                    File.Create(filePath);
                }
            }
        }
    }
}
