using System;
using Cosmos.Kernel.HAL.Interfaces.Devices;
using Cosmos.Kernel.System.Storage;

namespace HolmiumOS.Commands.System
{
    public class Fetch : ICommand
    {
        public string Name => "fetch";

        public string Description =>
            "Sistem bilgilerini gosterir";

        public string Usage => "fetch";

        public void Execute(string args)
        {
            string cpu =
                Environment.Is64BitProcess
                    ? "x86_64"
                    : "x86";

            string architecture =
                Environment.Is64BitProcess
                    ? "64-bit"
                    : "32-bit";

            string runtime =
                Environment.Version.ToString();

            string kernel =
                Kernel.OSVERSION;

            string storage =
                GetStorageInfo();

            string partitions =
                StorageManager.Partitions.Count.ToString();

            string primaryDisk =
                GetPrimaryDisk();

            string[] asciiLogo =
            {
                " __    __  ",
                "|  |  |  | ",
                "|  |__|  | ",
                "|   __   | ",
                "|  |  |  | ",
                "|__|  |__| ",
                "           "
            };

            string[] infoLabels =
            {
                "OS: ",
                "Kernel: ",
                "CPU: ",
                "Arch: ",
                "Runtime: ",
                "Disk: ",
                "Primary: "
            };

            string[] infoValues =
            {
                "HolmiumOS",
                kernel,
                cpu,
                architecture,
                runtime,
                storage,
                primaryDisk
            };

            ConsoleColor[] valueColors =
            {
                ConsoleColor.Green,
                ConsoleColor.Yellow,
                ConsoleColor.Gray,
                ConsoleColor.Cyan,
                ConsoleColor.Magenta,
                ConsoleColor.Yellow,
                ConsoleColor.Green
            };

            int infoIndex = 0;

            for (
                int i = 0;
                i < asciiLogo.Length;
                i++)
            {
                Console.ForegroundColor =
                    ConsoleColor.Cyan;

                Console.Write(
                    asciiLogo[i].PadRight(15));

                if (infoIndex < infoLabels.Length)
                {
                    Console.ForegroundColor =
                        ConsoleColor.Cyan;

                    Console.Write(
                        infoLabels[infoIndex]);

                    Console.ForegroundColor =
                        valueColors[infoIndex];

                    Console.Write(
                        infoValues[infoIndex]);

                    infoIndex++;
                }

                Console.WriteLine();
            }

            Console.ForegroundColor =
                ConsoleColor.DarkGray;

            Console.WriteLine();
            Console.WriteLine(
                $"Devices: {StorageManager.DeviceCount} | " +
                $"Partitions: {partitions}");

            Console.ResetColor();
        }

        private string GetStorageInfo()
        {
            if (!StorageManager.IsEnabled)
            {
                return "Disabled";
            }

            if (!StorageManager.IsInitialized)
            {
                return "Not initialized";
            }

            if (StorageManager.DeviceCount == 0)
            {
                return "No disk";
            }

            ulong totalBytes = 0;

            for (
                int i = 0;
                i < StorageManager.Devices.Count;
                i++)
            {
                IBlockDevice device =
                    StorageManager.Devices[i];

                if (device == null)
                {
                    continue;
                }

                totalBytes +=
                    device.BlockCount *
                    device.BlockSize;
            }

            return FormatBytes(totalBytes);
        }

        private string GetPrimaryDisk()
        {
            IBlockDevice primary =
                StorageManager.PrimaryDevice;

            if (primary == null)
            {
                return "None";
            }

            return primary.Name;
        }

        private string FormatBytes(
    ulong bytes)
{
    if (bytes >=
        1024UL * 1024UL * 1024UL * 1024UL)
    {
        double tb =
            bytes /
            (1024.0 * 1024.0 * 1024.0 * 1024.0);

        return $"{tb:F2} TB";
    }

    if (bytes >=
        1024UL * 1024UL * 1024UL)
    {
        double gb =
            bytes /
            (1024.0 * 1024.0 * 1024.0);

        return $"{gb:F2} GB";
    }

    if (bytes >=
        1024UL * 1024UL)
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