/*
 * HolmiumOS - A custom operating system project based on CosmosOS
 *
 * Copyright (C) 2026 Samet Bilir
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.
 */

using System;
using System.IO;

using Cosmos.Kernel.System.Filesystems.Fat;
using Cosmos.Kernel.System.Keyboard;
using Cosmos.Kernel.System.Keyboard.ScanMaps;
using Cosmos.Kernel.System.Storage;
using Cosmos.Kernel.System.Timer;
using Cosmos.Kernel.System.Vfs;

using Cosmos.Kernel.HAL.Vfs;
using Cosmos.Kernel.HAL.Interfaces.Devices;

using HolmiumOS.GUI;
using HolmiumOS.Network;
using HolmiumOS.Shell;

using FileSystemManager = HolmiumOS.Shell.FileSystemManager;
using Sys = Cosmos.Kernel.System;

namespace HolmiumOS
{
    public class Kernel : Sys.Kernel
    {
        public static readonly string OSVERSION = "0.4-beta";

        protected override void BeforeRun()
        {
            KeyboardManager.SetKeyLayout(new TRStandardLayout());

            /*
             * Önce storage hazırlanıyor.
             *
             * Disk yoksa burada sistem durur.
             * Disk var fakat partition yoksa kullanıcıdan
             * MBR/GPT ve partition kurulumu istenir.
             */
            if (!InitializeStorage())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("HolmiumOS baslatilamadi.");
                Console.ResetColor();

                HaltSystem();
                return;
            }

            /*
             * Artık /mnt kullanılabilir.
             */
            InitializeSystem();

            CommandManager.RegisterCommands();

            var mode = Boot.BootMenu.Show();

            if (mode == Boot.BootMode.CLI)
            {
                Init.isGuiLoopRunning = false;
            }

            if (mode == Boot.BootMode.GUI)
            {
                try
                {
                    GUI.Init.Start();
                    return;
                }
                catch (Exception ex)
                {
                    Console.ResetColor();
                    Console.Clear();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("GUI baslatilirken hata olustu:");
                    Console.WriteLine("Hata: " + ex.GetType().Name);
                    Console.WriteLine("Mesaj: " + ex.Message);
                    Console.ResetColor();
                }
            }

            Console.Clear();

            NetworkManager.Init();

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

            LoginScreen();

            Console.WriteLine();
            Console.WriteLine($"HolmiumOS Surum: {OSVERSION}");

            try
            {
                string motd = FileSystemManager.ReadFile("/mnt/boot/motd.txt");
                Console.WriteLine(motd);
            }
            catch
            {
                Console.WriteLine(
                    "CLI baslatildi. 'help' yazarak komutlari gorebilirsiniz.");
            }

            Console.WriteLine();
        }

        /*
         * ============================================================
         * STORAGE
         * ============================================================
         */

        private bool InitializeStorage()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== HolmiumOS Storage Manager ===");
            Console.ResetColor();

            /*
             * Cosmos'un storage katmanının birincil diskini al.
             */
            IBlockDevice? disk = StorageManager.PrimaryDevice;

            if (disk == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("Hicbir disk bulunamadi.");
                Console.ResetColor();

                Console.WriteLine();
                Console.WriteLine(
                    "QEMU kullaniyorsaniz disk image'inin bagli oldugundan emin olun.");
                Console.WriteLine(
                    "Ornek: cosmos run --disk disk.img");

                return false;
            }

            Console.WriteLine();
            Console.WriteLine("Disk bulundu.");
            Console.WriteLine("Disk: " + disk.Name);
            Console.WriteLine("Block size: " + disk.BlockSize);
            Console.WriteLine("Block count: " + disk.BlockCount);

            /*
             * Partition listesi bos mu?
             *
             * Bos olması:
             *   - disk var
             *   - fakat partition bulunmuyor
             *
             * anlamına gelebilir.
             */
            if (StorageManager.Partitions == null ||
                StorageManager.Partitions.Count == 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Bu diskte partition bulunamadi.");
                Console.ResetColor();

                if (!CreatePartitionLayout(disk))
                {
                    return false;
                }
            }
            else
            {
                /*
                 * Mevcut partitionlar var.
                 * Kullanıcıya hangisinin kullanılacağını sor.
                 */
                if (!SelectExistingPartition())
                {
                    return false;
                }
            }

            /*
             * FAT filesystem sürücüsünü kaydet.
             */
            if (!RegisterAndMountFilesystem())
            {
                return false;
            }

            return true;
        }

        /*
         * ============================================================
         * PARTITION SETUP
         * ============================================================
         */

        private bool CreatePartitionLayout(IBlockDevice disk)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== Disk Bolumlendirme ===");
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine("Disk: " + disk.Name);

            ulong diskBytes = disk.BlockSize * disk.BlockCount;
            ulong diskMiB = diskBytes / (1024UL * 1024UL);

            Console.WriteLine("Boyut: " + diskMiB + " MiB");

            Console.WriteLine();

            /*
             * Eğer zaten GPT veya MBR varsa kullanıcıya bilgi ver.
             */
            bool isGpt = false;
            bool isMbr = false;

            try
            {
                isGpt = Gpt.IsGpt(disk);
            }
            catch
            {
                isGpt = false;
            }

            try
            {
                isMbr = Mbr.IsMbr(disk);
            }
            catch
            {
                isMbr = false;
            }

            if (isGpt)
            {
                Console.WriteLine("Mevcut partition tablosu: GPT");
            }
            else if (isMbr)
            {
                Console.WriteLine("Mevcut partition tablosu: MBR");
            }
            else
            {
                Console.WriteLine("Partition tablosu bulunamadi.");
            }

            /*
             * Partition scheme seçimi.
             */
            Console.WriteLine();
            Console.WriteLine("[1] GPT");
            Console.WriteLine("[2] MBR");
            Console.WriteLine("[3] Iptal");
            Console.WriteLine();

            int schemeChoice = ReadNumber(
                "Partition scheme secin: ",
                1,
                3);

            if (schemeChoice == 3)
            {
                return false;
            }

            /*
             * DİKKAT:
             *
             * Gpt.Create / Mbr.Create mevcut partition tablosunu
             * değiştirebilir / silebilir.
             *
             * Kullanıcıdan açık onay alıyoruz.
             */
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("UYARI!");
            Console.ResetColor();

            Console.WriteLine(
                "Bu islem diskin partition tablosunu yeniden olusturabilir.");
            Console.WriteLine(
                "Mevcut veriler kaybolabilir.");
            Console.WriteLine();

            if (!AskYesNo("Devam etmek istiyor musunuz? [y/N]: "))
            {
                Console.WriteLine("Islem iptal edildi.");
                return false;
            }

            /*
             * Partition tablosunu oluştur.
             */
            try
            {
                if (schemeChoice == 1)
                {
                    Console.WriteLine();
                    Console.WriteLine("GPT partition tablosu olusturuluyor...");

                    Gpt.Create(disk);

                    Console.WriteLine("GPT olusturuldu.");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("MBR partition tablosu olusturuluyor...");

                    Mbr.Create(disk);

                    Console.WriteLine("MBR olusturuldu.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    "Partition tablosu olusturulamadi.");
                Console.WriteLine(
                    "Hata: " + ex.Message);
                Console.ResetColor();

                return false;
            }

            /*
             * İlk 2048 sector'u boş bırakıyoruz.
             *
             * 512 byte sector varsayımıyla:
             *
             * 2048 * 512 = 1 MiB
             *
             * Böylece partition 1 MiB hizalı başlar.
             */
            ulong startSector = 2048;

            if (disk.BlockCount <= startSector)
            {
                Console.WriteLine(
                    "Disk partition olusturmak icin cok kucuk.");
                return false;
            }

            ulong sectorCount = disk.BlockCount - startSector;

            /*
             * Çok küçük bir disk için taşmayı önle.
             */
            if (sectorCount == 0)
            {
                Console.WriteLine(
                    "Partition icin yeterli alan yok.");
                return false;
            }

            Console.WriteLine();
            Console.WriteLine("Partition olusturuluyor...");
            Console.WriteLine("Baslangic sector: " + startSector);
            Console.WriteLine("Sector sayisi: " + sectorCount);

            try
            {
                bool created = PartitionManager.Create(
                    disk,
                    startSector: startSector,
                    sectorCount: sectorCount,
                    mbrSystemId: 0x0C,
                    gptType: Gpt.BasicDataPartitionType);

                if (!created)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Partition olusturulamadi.");
                    Console.ResetColor();

                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Partition olusturma hatasi:");
                Console.WriteLine(ex.Message);
                Console.ResetColor();

                return false;
            }

            /*
             * ÇOK ÖNEMLİ:
             *
             * Yeni partition StorageManager.Partitions'a
             * otomatik olarak gelmez.
             *
             * Rescan gerekiyor.
             */
            Console.WriteLine("Partition tablosu yeniden taraniyor...");

            try
            {
                StorageManager.RescanPartitions(disk);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Partition rescan basarisiz.");
                Console.WriteLine(ex.Message);
                Console.ResetColor();

                return false;
            }

            if (StorageManager.Partitions == null ||
                StorageManager.Partitions.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    "Partition olusturuldu ancak Cosmos partition'i goremedi.");
                Console.ResetColor();

                return false;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine(
                "Partition basariyla olusturuldu.");
            Console.ResetColor();

            /*
             * Yeni partition'u seç.
             */
            return true;
        }

        /*
         * ============================================================
         * EXISTING PARTITION SELECT
         * ============================================================
         */

        private bool SelectExistingPartition()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Partition Secimi ===");
            Console.ResetColor();

            Console.WriteLine();

            int count = StorageManager.Partitions.Count;

            for (int i = 0; i < count; i++)
            {
                var partition = StorageManager.Partitions[i];

                Console.WriteLine(
                    $"[{i}] {partition}");
            }

            Console.WriteLine();

            int selected = ReadNumber(
                "Kullanilacak partition: ",
                0,
                count - 1);

            /*
             * Seçilen partition'ı listenin başına taşımıyoruz.
             *
             * Bunun yerine Mount sırasında doğrudan seçilen
             * partition nesnesini kullanacağız.
             */
            SelectedPartitionIndex = selected;

            Console.WriteLine();
            Console.WriteLine(
                $"Partition [{selected}] secildi.");

            return true;
        }

        private int SelectedPartitionIndex = 0;

        /*
         * ============================================================
         * FILESYSTEM
         * ============================================================
         */

        private bool RegisterAndMountFilesystem()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== FAT Filesystem ===");
            Console.ResetColor();

            FatFilesystemType fat = new();

            if (!VfsManager.RegisterFilesystem("fat", fat))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    "FAT filesystem kaydi basarisiz.");
                Console.ResetColor();

                return false;
            }

            if (StorageManager.Partitions == null ||
                StorageManager.Partitions.Count == 0)
            {
                Console.WriteLine(
                    "Mount edilecek partition bulunamadi.");

                return false;
            }

            if (SelectedPartitionIndex < 0 ||
                SelectedPartitionIndex >= StorageManager.Partitions.Count)
            {
                /*
                 * Yeni partition oluşturduysak normalde [0]
                 * olacaktır.
                 */
                SelectedPartitionIndex = 0;
            }

            var partition =
                StorageManager.Partitions[SelectedPartitionIndex];

            Console.WriteLine();
            Console.WriteLine(
                $"Partition [{SelectedPartitionIndex}] mount ediliyor...");

            /*
             * Önce mevcut filesystem'in mount edilebilir olup
             * olmadığını deniyoruz.
             */
            if (VfsManager.TryMount(
                "fat",
                partition,
                MountFlags.None,
                "/mnt",
                out VfsManager.VfsMount? mount))
            {
                if (mount != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine(
                        "Mounted " +
                        mount.Name +
                        " at " +
                        mount.MountPoint);

                    Console.ResetColor();

                    return true;
                }

                Console.WriteLine(
                    "Mount basarili ancak mount bilgisi alinamadi.");

                return false;
            }

            /*
             * Mount başarısızsa partition muhtemelen FAT formatlı
             * değildir.
             *
             * Burada kullanıcıya formatlama seçeneği sunuyoruz.
             */
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine(
                "Partition FAT olarak mount edilemedi.");
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine(
                "Partition'i FAT32 olarak formatlamak ister misiniz?");
            Console.WriteLine(
                "UYARI: Partition'daki mevcut veriler silinebilir.");
            Console.WriteLine();

            if (!AskYesNo("Formatla? [y/N]: "))
            {
                return false;
            }

            FatFormatOptions options = new()
            {
                Type = FatType.Fat32,
                VolumeLabel = "HOLMIUMOS"
            };

            Console.WriteLine();
            Console.WriteLine("FAT32 formatlama baslatiliyor...");

            if (!VfsManager.TryFormat(
                "fat",
                partition,
                options))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    "FAT32 formatlama basarisiz.");
                Console.ResetColor();

                return false;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
                "FAT32 formatlama tamamlandi.");
            Console.ResetColor();

            /*
             * Format sonrası tekrar mount et.
             */
            if (!VfsManager.TryMount(
                "fat",
                partition,
                MountFlags.None,
                "/mnt",
                out VfsManager.VfsMount? formattedMount))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    "Format sonrasi mount basarisiz.");
                Console.ResetColor();

                return false;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
                "Mounted " +
                formattedMount?.Name +
                " at " +
                formattedMount?.MountPoint);

            Console.ResetColor();

            return true;
        }

        /*
         * ============================================================
         * SYSTEM INITIALIZATION
         * ============================================================
         */

        private void InitializeSystem()
        {
            Directory.CreateDirectory("/mnt/bin");
            Directory.CreateDirectory("/mnt/boot");
            Directory.CreateDirectory("/mnt/dev");
            Directory.CreateDirectory("/mnt/etc");
            Directory.CreateDirectory("/mnt/home");
            Directory.CreateDirectory("/mnt/root");
            Directory.CreateDirectory("/mnt/tmp");

            if (!File.Exists("/mnt/dev/null"))
                File.Create("/mnt/dev/null").Dispose();

            if (!File.Exists("/mnt/dev/zero"))
                File.Create("/mnt/dev/zero").Dispose();

            if (!File.Exists("/mnt/dev/random"))
                File.Create("/mnt/dev/random").Dispose();

            if (!File.Exists("/mnt/boot/motd.txt"))
            {
                File.WriteAllText(
                    "/mnt/boot/motd.txt",
                    "CLI baslatildi. 'help' yazarak komutlari gorebilirsiniz.");
            }

            if (UserManager.UserExists("root"))
                return;

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== Ilk Kurulum ===");
            Console.ResetColor();

            string password;

            while (true)
            {
                Console.Write("Root sifresi: ");
                password = PasswordReader.ReadPassword();

                Console.Write("Tekrar: ");
                string confirm = PasswordReader.ReadPassword();

                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Sifre bos olamaz.");
                    continue;
                }

                if (password != confirm)
                {
                    Console.WriteLine("Sifreler eslesmiyor.");
                    continue;
                }

                break;
            }

            UserManager.CreateRoot(password);

            Console.WriteLine();

            while (true)
            {
                Console.Write("Ilk kullanici adi: ");

                string username =
                    Console.ReadLine()?.Trim().ToLower() ?? "";

                if (!UserManager.IsValidUsername(
                    username,
                    out string error))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(error);
                    Console.ResetColor();

                    continue;
                }

                Console.Write("Parola: ");
                string pass1 = PasswordReader.ReadPassword();

                Console.Write("Tekrar: ");
                string pass2 = PasswordReader.ReadPassword();

                if (pass1 != pass2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        "Parolalar eslesmiyor.");
                    Console.ResetColor();

                    continue;
                }

                UserManager.CreateUser(username, pass1);
                break;
            }

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(
                "Ilk kurulum basariyla tamamlandi!");

            Console.WriteLine(
                "Sistem 3 saniye icinde yeniden baslatiliyor...");

            Console.ResetColor();

            TimerManager.Wait(3000);

            Sys.Power.Reboot();
        }

        /*
         * ============================================================
         * LOGIN
         * ============================================================
         */

        private void LoginScreen()
        {
            while (!UserManager.IsLoggedIn)
            {
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("HolmiumOS Login");
                Console.ResetColor();

                Console.Write("Username: ");

                string username =
                    Console.ReadLine()?.Trim() ?? "";

                Console.Write("Password: ");

                string password =
                    PasswordReader.ReadPassword();

                if (!UserManager.Login(
                    username,
                    password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        "Hatali kullanici adi veya parola.");
                    Console.ResetColor();

                    continue;
                }

                FileSystemManager.CurrentDirectory =
                    UserManager.HomeDirectory;

                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(
                    $"Hosgeldin, {UserManager.CurrentUser}.");

                Console.ResetColor();
            }
        }

        /*
         * ============================================================
         * SHELL PROMPT
         * ============================================================
         */

        private void WritePrompt()
        {
            string path =
                FileSystemManager.GetDisplayPath();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(UserManager.CurrentUser);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("@");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("HolmiumOS");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(":");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(path);

            Console.ForegroundColor =
                PermissionManager.IsRoot
                    ? ConsoleColor.Red
                    : ConsoleColor.White;

            Console.Write(
                PermissionManager.IsRoot
                    ? "# "
                    : "$ ");

            Console.ResetColor();
        }

        /*
         * ============================================================
         * RUN
         * ============================================================
         */

        protected override void Run()
        {
            if (!UserManager.IsLoggedIn)
            {
                LoginScreen();
                return;
            }

            if (Init.isGuiLoopRunning == false)
            {
                WritePrompt();

                string input =
                    InputReader
                        .ReadLineWithHistory(WritePrompt)
                        .Trim();

                if (string.IsNullOrWhiteSpace(input))
                    return;

                CommandHistory.Add(input);

                CommandManager.ExecuteCommand(input);
            }
        }

        /*
         * ============================================================
         * HELPERS
         * ============================================================
         */

        private int ReadNumber(
            string prompt,
            int min,
            int max)
        {
            while (true)
            {
                Console.Write(prompt);

                string input =
                    Console.ReadLine()?.Trim() ?? "";

                if (int.TryParse(
                    input,
                    out int value))
                {
                    if (value >= min &&
                        value <= max)
                    {
                        return value;
                    }
                }

                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(
                    $"Gecerli bir secim yapin ({min}-{max}).");

                Console.ResetColor();
            }
        }

        private bool AskYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                string input =
                    Console.ReadLine()?.Trim().ToLower() ?? "";

                if (input == "y" ||
                    input == "yes" ||
                    input == "e" ||
                    input == "evet")
                {
                    return true;
                }

                if (input == "" ||
                    input == "n" ||
                    input == "no" ||
                    input == "h" ||
                    input == "hayir")
                {
                    return false;
                }

                Console.WriteLine(
                    "Lutfen y veya n girin.");
            }
        }

        private void HaltSystem()
        {
            Console.WriteLine();
            Console.WriteLine(
                "Sistem durduruldu.");

            while (true)
            {
                TimerManager.Wait(1000);
            }
        }
    }
}