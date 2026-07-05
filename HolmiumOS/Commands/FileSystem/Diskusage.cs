using System;

namespace HolmiumOS.Commands.FileSystem
{
    public class DiskUsage : ICommand
    {
        public string Name => "diskusage";
        public string Description => "Disk Kullanim bilgilerini gosterir";
        public string Usage => "diskusage";

        public void Execute(string args)
        {
            try
            {
                var fs = Kernel.fs;

                string drive = @"0:\";

                ulong totalBytes = (ulong)fs.GetTotalSize(drive);
                ulong freeBytes = (ulong)fs.GetAvailableFreeSpace(drive);
                ulong usedBytes = totalBytes - freeBytes;

                double totalMB = totalBytes / 1024.0 / 1024.0;
                double usedMB = usedBytes / 1024.0 / 1024.0;
                double freeMB = freeBytes / 1024.0 / 1024.0;

                double percent = totalBytes == 0
                    ? 0
                    : (usedBytes * 100.0) / totalBytes;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Disk Kullanimi");
                Console.WriteLine("-------------------------");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Disk        : {drive}");
                Console.WriteLine($"Toplam Alan : {totalMB:F2} MB");
                Console.WriteLine($"Kullanilan  : {usedMB:F2} MB");
                Console.WriteLine($"Bos Alan    : {freeMB:F2} MB");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Kullanim %  : {percent:F2}%");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Disk bilgisi alinamadi: {e.Message}");
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}