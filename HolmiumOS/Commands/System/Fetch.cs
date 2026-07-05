using Cosmos.Core;
using System;
using Sys = Cosmos.System;

namespace HolmiumOS.Commands.System
{
    public class Fetch : ICommand
    {
        public string Name => "fetch";
        public string Description => "Sistem bilgilerini gosterir";
        public string Usage => "fetch";

        public void Execute(string args)
        {
            var fs = Kernel.fs;
            string driveId = "0:";

            var cpuName = CPU.GetCPUBrandString();
            uint ramMB = CPU.GetAmountOfRAM();
            double ramGB = ramMB / 1024.0;
            ulong ramKB = ramMB * 1024;

            ulong totalBytes = (ulong)fs.GetTotalSize(driveId);
            double totalGB = totalBytes / 1024.0 / 1024.0 / 1024.0;

            string vmType = Sys.VMTools.IsVMWare ? "VMWare" :
                            Sys.VMTools.IsVirtualBox ? "VirtualBox" :
                            Sys.VMTools.IsQEMU ? "QEMU" : "";

            string[] asciiLogo = new string[]
            {
                " __    __  ",
                "|  |  |  | ",
                "|  |__|  | ",
                "|   __   | ",
                "|  |  |  | ",
                "|__|  |__| ",
                "           "
            };

            string[] infoLabels = new string[]
            {
                "OS: ", "Surum: ", "CPU: ", "RAM: ", "HDD: ", "FS: ", "VM: "
            };

            string[] infoValues = new string[]
            {
                "HolmiumOS",
                Kernel.OSVERSION,
                cpuName,
                $"{ramGB:F2} GB / {ramMB} MB",
                $"{totalGB:F2} GB",
                fs.GetFileSystemType(driveId),
                vmType
            };

            ConsoleColor[] labelColors = new ConsoleColor[]
            {
                ConsoleColor.Cyan, ConsoleColor.Cyan, ConsoleColor.Cyan,
                ConsoleColor.Cyan, ConsoleColor.Cyan, ConsoleColor.Cyan, ConsoleColor.Cyan
            };

            ConsoleColor[] valueColors = new ConsoleColor[]
            {
                ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Gray,
                ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.Red
            };

            int infoIndex = 0;
            for (int i = 0; i < asciiLogo.Length; i++)
            {
                // Logo sol
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(asciiLogo[i].PadRight(15));

                // Sağda info
                if (infoIndex < infoLabels.Length)
                {
                    Console.ForegroundColor = labelColors[infoIndex];
                    Console.Write(infoLabels[infoIndex]);
                    Console.ForegroundColor = valueColors[infoIndex];
                    Console.Write(infoValues[infoIndex]);
                    infoIndex++;
                }

                Console.WriteLine();
            }

            Console.ResetColor();
        }
    }
}