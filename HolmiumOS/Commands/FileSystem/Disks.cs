using System;
using Cosmos.System.FileSystem.VFS;
using HolmiumOS.Commands;
using Cosmos.HAL.BlockDevice;

namespace HolmiumOS.Commands.FileSystem
{
    public class Disks : ICommand
    {
        public string Name => "disks";
        public string Description => "Sistemdeki diskleri ve turlerini listeler";
        public string Usage => "disks";

        public void Execute(string args)
        {
            var disks = VFSManager.GetDisks();

            Console.WriteLine();
            Console.WriteLine($"Diskler | Total: {disks.Count}");
            Console.WriteLine();

            for (int i = 0; i < disks.Count; i++)
            {
                var disk = disks[i];
                string typeStr = disk.Type switch
                {
                    BlockDeviceType.HardDrive => "Sabit Disk",
                    BlockDeviceType.Removable => "cikarilabilir",
                    BlockDeviceType.RemovableCD => "cikarilabilir CD",
                    _ => "Bilinmeyen"
                };

                ulong sizeMB = (disk.Host.BlockCount * disk.Host.BlockSize) / 1024 / 1024;
                Console.WriteLine($"Disk {i} | Tur: {typeStr} | Size: {sizeMB} MB");
            }

            Console.WriteLine();
        }
    }
}