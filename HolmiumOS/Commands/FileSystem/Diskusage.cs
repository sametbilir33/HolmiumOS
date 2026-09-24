using System;
using Cosmos.Kernel.HAL.Interfaces.Devices;
using Cosmos.Kernel.System.Storage;

namespace HolmiumOS.Commands.FileSystem
{
    public class DiskUsage : ICommand
    {
        public string Name => "diskusage";
        public string Description => "Disk kullanim bilgilerini gosterir";
        public string Usage => "diskusage";

        public void Execute(string args)
        {
            try
            {
                if (!StorageManager.IsEnabled)
                {
                    Console.WriteLine("Storage sistemi etkin degil.");
                    return;
                }

                if (!StorageManager.IsInitialized)
                {
                    Console.WriteLine("Storage sistemi henuz baslatilmadi.");
                    return;
                }

                if (StorageManager.DeviceCount == 0)
                {
                    Console.WriteLine("Disk bulunamadi.");
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Disk Kullanimi");
                Console.WriteLine("-------------------------");

                ulong totalBytes = 0;

                for (int i = 0; i < StorageManager.Devices.Count; i++)
                {
                    IBlockDevice device = StorageManager.Devices[i];

                    if (device == null)
                        continue;

                    ulong deviceBytes =
                        device.BlockCount * device.BlockSize;

                    totalBytes += deviceBytes;

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(
                        $"Disk {i}      : {device.Name}");

                    Console.WriteLine(
                        $"Kapasite     : {FormatBytes(deviceBytes)}");

                    Console.WriteLine(
                        $"Blok Sayisi  : {device.BlockCount}");

                    Console.WriteLine(
                        $"Blok Boyutu  : {device.BlockSize} bytes");

                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(
                    $"Toplam Kapasite : {FormatBytes(totalBytes)}");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(
                    "Bos alan bilgisi Gen3 public Storage API'sinde mevcut degil.");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    $"Disk bilgisi alinamadi: {e.Message}");
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private string FormatBytes(ulong bytes)
        {
            if (bytes >= 1024UL * 1024UL * 1024UL * 1024UL)
            {
                double tb =
                    bytes /
                    (1024.0 * 1024.0 * 1024.0 * 1024.0);

                return $"{tb:F2} TB";
            }

            if (bytes >= 1024UL * 1024UL * 1024UL)
            {
                double gb =
                    bytes /
                    (1024.0 * 1024.0 * 1024.0);

                return $"{gb:F2} GB";
            }

            if (bytes >= 1024UL * 1024UL)
            {
                double mb =
                    bytes /
                    (1024.0 * 1024.0);

                return $"{mb:F2} MB";
            }

            if (bytes >= 1024UL)
            {
                double kb =
                    bytes / 1024.0;

                return $"{kb:F2} KB";
            }

            return $"{bytes} B";
        }
    }
}