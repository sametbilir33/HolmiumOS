using System;
using System.IO;
using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.ScanMaps;
using HolmiumOS.Shell;
using HolmiumOS.Sound;
using IL2CPU.API.Attribs;
using FileSystemManager = HolmiumOS.Shell.FileSystemManager;
using Sys = Cosmos.System;

namespace HolmiumOS
{
    public class Kernel : Sys.Kernel
    {
        public static CosmosVFS fs;

        public static readonly string OSVERSION = "0.4-beta";

        [ManifestResourceStream(ResourceName = "HolmiumOS.muzik.wav")]
        public static byte[] Startup;

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

            InitializeSystem();
            LoginScreen();
            CommandManager.RegisterCommands();

            Console.WriteLine();
            Console.WriteLine($"HolmiumOS Surum: {OSVERSION}");

            try
            {
            string motd = FileSystemManager.ReadFile(@"0:\boot\motd.txt");
            Console.WriteLine(motd);
            }
            catch
            {
             Console.WriteLine("CLI baslatildi. 'help' yazarak komutlari gorebilirsiniz.");
            }

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

        private void InitializeSystem()
        {
            Directory.CreateDirectory(@"0:\bin");
            Directory.CreateDirectory(@"0:\boot");
            Directory.CreateDirectory(@"0:\dev");
            Directory.CreateDirectory(@"0:\etc");
            Directory.CreateDirectory(@"0:\home");
            Directory.CreateDirectory(@"0:\root");
            Directory.CreateDirectory(@"0:\tmp");

            if (!File.Exists(@"0:\dev\null"))
                File.Create(@"0:\dev\null").Dispose();

            if (!File.Exists(@"0:\dev\zero"))
                File.Create(@"0:\dev\zero").Dispose();

            if (!File.Exists(@"0:\dev\random"))
                File.Create(@"0:\dev\random").Dispose();

            if (!File.Exists(@"0:\boot\motd.txt"))
            {
                File.WriteAllText(@"0:\boot\motd.txt",
                    "CLI baslatildi. 'help' yazarak komutlari gorebilirsiniz.");
            }

            if (UserManager.UserExists("root"))
                return;

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
                string username = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (!UserManager.IsValidUsername(username, out string error))
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
                    Console.WriteLine("Parolalar eslesmiyor.");
                    Console.ResetColor();
                    continue;
                }

                UserManager.CreateUser(username, pass1);
                break;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Ilk kurulum tamamlandi.");
            Console.ResetColor();
        }
        private void LoginScreen()
        {
            while (!UserManager.IsLoggedIn)
            {
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("HolmiumOS Login");
                Console.ResetColor();

                Console.Write("Username: ");
                string username = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Password: ");
                string password = PasswordReader.ReadPassword();

                if (!UserManager.Login(username, password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Hatali kullanici adi veya parola.");
                    Console.ResetColor();
                    continue;
                }

                FileSystemManager.CurrentDirectory = UserManager.HomeDirectory;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Hosgeldin, {UserManager.CurrentUser}.");
                Console.ResetColor();
            }
        }
        private void WritePrompt()
        {
            string path = FileSystemManager.CurrentDirectory;

            if (path.StartsWith(UserManager.HomeDirectory, StringComparison.OrdinalIgnoreCase))
            {
                path = "~" + path.Substring(UserManager.HomeDirectory.Length);
                if (path == "~")
                    path = "~/";
            }

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

            Console.ForegroundColor = PermissionManager.IsRoot ? ConsoleColor.Red : ConsoleColor.White;
            Console.Write(PermissionManager.IsRoot ? "# " : "$ ");

            Console.ResetColor();
        }

        protected override void Run()
        {
            if (!UserManager.IsLoggedIn)
            {
                LoginScreen();
                return;
            }

            AudioManager.Update();

            WritePrompt();

            string input = InputReader.ReadLineWithHistory(WritePrompt).Trim();

            if (string.IsNullOrWhiteSpace(input))
                return;

            CommandHistory.Add(input);
            CommandManager.ExecuteCommand(input);

            Heap.Collect();
        }
    }
}