using System;
using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.ScanMaps;
using HolmiumOS.Shell;
using Sys = Cosmos.System;

namespace HolmiumOS
{
    public class Kernel : Sys.Kernel
    {
        public static CosmosVFS fs;

        public static readonly string OSVERSION = "0.3-alpha";

        protected override void BeforeRun()
        {
            Console.Clear();

            try
            {
                fs = new CosmosVFS();
                VFSManager.RegisterVFS(fs);
                BootStatus("VFS baslatildi", true);
            }
            catch (Exception)
            {
                BootStatus("VFS baslatildi", false);
            }

            try
            {
                var disks = fs.GetDisks();

                foreach (var disk in disks)
                {
                    disk.Mount();
                }

                BootStatus("Diskler mount edildi", true);
            }
            catch (Exception e)
            {
                BootStatus($"Disk mount hatasi: {e.Message}", false);
            }

            try
            {
                Sys.KeyboardManager.SetKeyLayout(new TRStandardLayout());
                BootStatus("Klavye layout TR yapildi", true);
            }
            catch
            {
                BootStatus("Klavye layout ayarlanamadi", false);
            }

            CheckResources();

            Console.ResetColor();

            string[] asciiArt = new string[]
            {
                " _   _       _           _                  ___  ____  ",
                "| | | | ___ | |_ __ ___ (_)_   _ _ __ ___  / _ \\/ ___| ",
                "| |_| |/ _ \\| | '_ ` _ \\| | | | | '_ ` _ \\| | | \\___ \\ ",
                "|  _  | (_) | | | | | | | | |_| | | | | | | |_| |___)|",
                "|_| |_|\\___/|_|_| |_| |_|_|\\__,_|_| |_| |_|\\___/|____/ "
            };

            ConsoleColor[] colors = new ConsoleColor[]
            {
                ConsoleColor.Red,
                ConsoleColor.Green,
                ConsoleColor.Yellow,
                ConsoleColor.Cyan,
                ConsoleColor.Magenta
            };

            for (int i = 0; i < asciiArt.Length; i++)
            {
                Console.ForegroundColor = colors[i % colors.Length];
                Console.WriteLine(asciiArt[i]);
            }

            Console.ResetColor();

            CommandManager.RegisterCommands();

            Console.WriteLine();
            Console.WriteLine($"HolmiumOS Surum: {OSVERSION}");
            Console.WriteLine("CLI baslatildi. 'help' yazarak komutlari gorebilirsiniz.");
            Console.WriteLine();
        }

        private void BootStatus(string message, bool ok)
        {
            if (ok)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[ OK ] ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[FAILED] ");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }

        private void CheckResources()
        {
            uint ramMB = CPU.GetAmountOfRAM();
            ulong totalBytes = (ulong)fs.GetTotalSize(@"0:\");
            double vfsMB = totalBytes / 1024.0 / 1024.0;

            if (ramMB < 512 && vfsMB < 512)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"UYARI: RAM ({ramMB} MB) ve toplam hafiza ({vfsMB:F2} MB) 512 MB'den az!");
            }
            else if (ramMB < 512)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"UYARI: RAM ({ramMB} MB) 512 MB'den az!");
            }
            else if (vfsMB < 512)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"UYARI: Toplam hafiza ({vfsMB:F2} MB) 512 MB'den az!");
            }

            Console.ResetColor();
        }

        protected override void Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("HolmiumOS");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(":");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Shell.FileSystemManager.CurrentDirectory);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("> ");

            string input = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(input))
                return;

            CommandManager.ExecuteCommand(input);
            Heap.Collect();
        }
    }
}