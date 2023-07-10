namespace FileWatcher
{
    internal class Program
    {
        private static readonly FileSystemWatcher FsWatcher = new();

        private static string SourceDir = "";
        private static string DistDir = "";
        private static string Extension = "";

        static async Task Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("参数错误");
                return;
            }

            SourceDir = args[0];
            DistDir = args[1];
            Extension = args[2];

            if (!Directory.Exists(SourceDir))
            {
                Console.WriteLine("源目录不存在");
                return;
            }
            if (!Directory.Exists(DistDir))
            {
                Console.WriteLine("目标目录不存在");
                return;
            }

            Console.WriteLine($"FileWatcher of {Extension} files started.");

            OnFsChanges();

            await Task.Delay(TimeSpan.FromDays(7));

        }



        private static void OnFsChanges()
        {
            FsWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime;
            FsWatcher.IncludeSubdirectories = false;
            FsWatcher.Path = SourceDir;
            FsWatcher.Created += new FileSystemEventHandler(FsWatcher_Created);
            FsWatcher.EnableRaisingEvents = true;
        }

        private static void FsWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                Thread.Sleep(1000 * 3);
                FileInfo fi = new(e.FullPath);
                if (!fi.Exists || !fi.Extension.Contains(Extension)) { return; }

                var dist = Path.Combine(DistDir, fi.Name);
                File.Move(fi.FullName, dist);
                Console.WriteLine($"Move {fi.FullName} --> {dist}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}