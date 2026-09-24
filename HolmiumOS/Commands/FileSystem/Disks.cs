using System;
using Cosmos.Kernel.System.Storage;
using Cosmos.Kernel.HAL.Interfaces.Devices;

namespace HolmiumOS.Commands.FileSystem
{
    public class Disks : ICommand
    {
        public string Name => "disks";

        public string Description =>
            "Sistemdeki diskleri, kapasitelerini ve bolumlerini listeler";

        public string Usage => "disks";

        public void Execute(string args)
        {
            if (!StorageManager.IsEnabled)
            {
                Console.WriteLine(
                    "Storage: Devre disi");

                Console.WriteLine();
                return;
            }

            if (!StorageManager.IsInitialized)
            {
                Console.WriteLine(
                    "Storage: Baslatilmadi");

                Console.WriteLine();
                return;
            }

            int deviceCount =
                StorageManager.DeviceCount;

            int partitionCount =
                StorageManager.Partitions.Count;

            Console.WriteLine(
                "Diskler     : " +
                deviceCount);

            Console.WriteLine(
                "Bolumler    : " +
                partitionCount);

            Console.WriteLine(
                "Primary Disk: " +
                GetPrimaryDeviceName());

            Console.WriteLine();

            if (deviceCount == 0)
            {
                Console.WriteLine(
                    "Herhangi bir block device bulunamadi.");

                Console.WriteLine();
                return;
            }

            for (
                int i = 0;
                i < StorageManager.Devices.Count;
                i++)
            {
                IBlockDevice device =
                    StorageManager.Devices[i];

                PrintDevice(
                    device,
                    i);
            }

            Console.WriteLine();
            Console.WriteLine(
                "Not: Gen3 StorageManager tarafinda " +
                "AHCI ve NVMe cihazlari kaydedilir.");

            Console.WriteLine(
                "USB / SD Card destegi, ilgili Gen3 " +
                "storage driver'i mevcut oldugunda listelenebilir.");

            Console.WriteLine();
        }

        private void PrintDevice(
            IBlockDevice device,
            int index)
        {
            if (device == null)
            {
                return;
            }

            ulong totalBytes =
                device.BlockCount *
                device.BlockSize;

            bool isPrimary =
                ReferenceEquals(
                    device,
                    StorageManager.PrimaryDevice);

            Console.WriteLine(
                "----------------------------------------");

            Console.WriteLine(
                "Disk " +
                index +
                (isPrimary ? " [PRIMARY]" : ""));

            Console.WriteLine(
                "Name        : " +
                device.Name);

            Console.WriteLine(
                "Capacity    : " +
                FormatBytes(totalBytes));

            Console.WriteLine(
                "Blocks      : " +
                device.BlockCount);

            Console.WriteLine(
                "Block Size  : " +
                device.BlockSize +
                " bytes");

            Console.WriteLine(
                "Bus/Type    : " +
                DetectDeviceType(device));

            var partitions =
                StorageManager.GetPartitions(device);

            Console.WriteLine(
                "Partitions  : " +
                partitions.Count);

            if (partitions.Count == 0)
            {
                Console.WriteLine(
                    "  -> Partition bulunamadi.");
            }
            else
            {
                for (
                    int p = 0;
                    p < partitions.Count;
                    p++)
                {
                    var partition =
                        partitions[p];

                    ulong partitionBytes =
                        partition.BlockCount *
                        partition.Host.BlockSize;

                    Console.WriteLine(
                        "  Partition " +
                        p +
                        " | " +
                        FormatBytes(partitionBytes));

                    Console.WriteLine(
                        "    Blocks: " +
                        partition.BlockCount);

                    Console.WriteLine(
                        "    Size  : " +
                        FormatBytes(partitionBytes));
                }
            }

            Console.WriteLine();
        }

        private string GetPrimaryDeviceName()
        {
            IBlockDevice primary =
                StorageManager.PrimaryDevice;

            if (primary == null)
            {
                return "Yok";
            }

            return primary.Name;
        }

        private string DetectDeviceType(
            IBlockDevice device)
        {
            if (device == null)
            {
                return "Unknown";
            }

            string name =
                device.Name ?? string.Empty;

            string lower =
                name.ToLowerInvariant();

            if (lower.Contains("nvme"))
            {
                return "NVMe";
            }

            if (lower.Contains("ahci"))
            {
                return "AHCI";
            }

            if (lower.Contains("sata"))
            {
                return "SATA";
            }

            if (lower.Contains("usb"))
            {
                return "USB";
            }

            if (lower.Contains("sd"))
            {
                return "SD Card";
            }

            return "Block Device";
        }

        private string FormatBytes(
            ulong bytes)
        {
            if (bytes >=
                1024UL *
                1024UL *
                1024UL *
                1024UL)
            {
                ulong tb =
                    bytes /
                    (1024UL *
                     1024UL *
                     1024UL *
                     1024UL);

                return tb + " TB";
            }

            if (bytes >=
                1024UL *
                1024UL *
                1024UL)
            {
                ulong gb =
                    bytes /
                    (1024UL *
                     1024UL *
                     1024UL);

                return gb + " GB";
            }

            if (bytes >=
                1024UL *
                1024UL)
            {
                ulong mb =
                    bytes /
                    (1024UL *
                     1024UL);

                return mb + " MB";
            }

            if (bytes >= 1024UL)
            {
                ulong kb =
                    bytes /
                    1024UL;

                return kb + " KB";
            }

            return bytes + " B";
        }
    }
}