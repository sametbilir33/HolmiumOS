using System;
using HolmiumOS.Shell;
using Syste = System;

namespace HolmiumOS.Commands.System
{
    public class Reset : ICommand
    {
        public string Name => "reset";
        public string Description => "Diski tamamen siler ve fabrika ayarlarina dondurur";
        public string Usage => "reset";

        private static bool _running;

        public void Execute(string args)
        {
            if (_running)
            {
                Console.WriteLine("Reset islemi zaten calisiyor.");
                return;
            }

            if (!UserManager.IsRoot && !PermissionManager.IsElevated)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bu islem icin root yetkisi gerekli. 'sudo reset' deneyin.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("!!! DIKKAT !!!");
            Console.WriteLine("Bu islem diskteki tum verileri geri donulemez sekilde silecektir.");
            Console.Write("Devam etmek icin EVET yazin: ");
            Console.ResetColor();

            if (Console.ReadLine() != "EVET")
            {
                Console.WriteLine("Islem iptal edildi.");
                return;
            }

            _running = true;

            try
            {
                var disk = Kernel.fs.Disks[0];

                Console.WriteLine("[1/5] Disk boyutu hesaplaniyor...");

                int sizeMB = (int)(disk.Size / 1024 / 1024);

                if (sizeMB > 8)
                    sizeMB -= 8;

                Console.WriteLine($"Kullanilacak boyut: {sizeMB} MB");

                Console.WriteLine("[2/5] Partitionlar siliniyor...");

                for (int i = disk.Partitions.Count - 1; i >= 0; i--)
                {
                    disk.DeletePartition(i);
                }

                Console.WriteLine("[3/5] Yeni partition olusturuluyor...");

                disk.CreatePartition(sizeMB);

                Console.WriteLine("[4/5] FAT32 format atiliyor...");

                disk.FormatPartition(0, "FAT32", true);

                Console.WriteLine("[5/5] Son islemler...");

                Syste.Threading.Thread.Sleep(1000);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Reset basarili.");
                Console.WriteLine("Sistem yeniden baslatiliyor...");
                Console.ResetColor();

                Syste.Threading.Thread.Sleep(1500);

                Cosmos.System.Power.Reboot();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Reset basarisiz!");
                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                _running = false;
            }
        }
    }
}