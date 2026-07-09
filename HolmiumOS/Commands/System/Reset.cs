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

        public void Execute(string args)
        {
            if (!UserManager.IsRoot && !PermissionManager.IsElevated)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bu islem icin root yetkisi gerekli. 'sudo reset' deneyin.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("!!! DIKKAT !!!");
            Console.WriteLine("Bu islem diskteki TUM verileri geri donulemez sekilde silecek.");
            Console.WriteLine("Onaylamak icin 'EVET' yazin:");
            Console.ResetColor();

            string confirm = Console.ReadLine();

            if (confirm != "EVET")
            {
                Console.WriteLine("Islem iptal edildi.");
                return;
            }

            try
            {
                var disk = Kernel.fs.Disks[0];

                Console.WriteLine("[DEBUG] Disk boyutu aliniyor...");

                int sizeMB = (int)(Kernel.fs.GetTotalSize(@"0:\") / 1024 / 1024);

                Console.WriteLine($"[DEBUG] Disk boyutu: {sizeMB} MB");

                Console.WriteLine("[DEBUG] Partitionlar siliniyor...");

                while (disk.Partitions.Count > 0)
                {
                    disk.DeletePartition(0);
                }

                Console.WriteLine("[DEBUG] Disk temizleniyor...");

                disk.Clear();

                Console.WriteLine("[DEBUG] Yeni partition olusturuluyor...");

                disk.CreatePartition(sizeMB);

                Console.WriteLine("[DEBUG] FAT32 format atiliyor...");

                disk.FormatPartition(0, "FAT32", true);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Disk sifirlandi. Sistem yeniden baslatiliyor...");
                Console.ResetColor();

                Syste.Threading.Thread.Sleep(2000);

                Cosmos.System.Power.Reboot();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Hata: {ex.GetType().Name} - {ex.Message}");
                Console.ResetColor();

                Console.WriteLine("Devam etmek icin bir tusa basin...");
                Console.ReadKey(true);
            }
        }
    }
}