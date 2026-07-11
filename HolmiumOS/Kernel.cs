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
 * See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.HAL.BlockDevice.Ports;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.ScanMaps;
using HolmiumOS.Drivers;
using HolmiumOS.Network;
using HolmiumOS.Shell;
using HolmiumOS.Sound;
using FileSystemManager = HolmiumOS.Shell.FileSystemManager;
using Sys = Cosmos.System;

namespace HolmiumOS
{
    public class Kernel : Sys.Kernel
    {
        public static CosmosVFS fs;

        public static readonly string OSVERSION = "0.4-beta";

        protected override void BeforeRun()
        {
            fs = new CosmosVFS();
            VFSManager.RegisterVFS(fs);

            AHCI_DISK ahci_load = new();

            ahci_load.Init();

            AHCI_DISK.Check();

            List<Disk> sataDisks = new();

            for (int i = 0; i < SATA.Devices.Count; i++)
            {
                Disk disk = new(SATA.Devices[i]);
                sataDisks.Add(disk);
            }

            try
            {
                var disks = fs.GetDisks();

                foreach (var disk in disks)
                {
                    disk.Mount();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Disk mount hatasi: {e.Message}", false);  //daha sonra loglara gelcek
            }

            Console.Clear();

            Sys.KeyboardManager.SetKeyLayout(new TRStandardLayout());

            string licenseFile = @"0:\boot\license.accepted";

            bool needAccept = true;

            if (File.Exists(licenseFile))
            {
                string licenseData = File.ReadAllText(licenseFile);

                if (licenseData.Contains($"Version: {OSVERSION}"))
                {
                    needAccept = false;
                }
            }

            if (needAccept)
            {
                while (true)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Clear();

                    Console.WriteLine("==============================================");
                    Console.WriteLine("              HOLMIUMOS LISANS UYARISI");
                    Console.WriteLine("==============================================");
                    Console.WriteLine();
                    Console.WriteLine("HolmiumOS gelistirme asamasinda olan bir");
                    Console.WriteLine("isletim sistemi projesidir.");
                    Console.WriteLine();
                    Console.WriteLine("Kullanim sirasinda veri kaybi, dosya bozulmasi");
                    Console.WriteLine("veya beklenmeyen sistem hatalari meydana gelebilir.");
                    Console.WriteLine();
                    Console.WriteLine("Gercek donanim uzerinde kullanmadan once");
                    Console.WriteLine("verilerinizi yedeklemeniz onerilir.");
                    Console.WriteLine("Sanal makine ortaminda test edilmesi tavsiye edilir.");
                    Console.WriteLine();
                    Console.WriteLine("Olusabilecek veri kaybi veya donanim sorunlarindan");
                    Console.WriteLine("HolmiumOS gelistiricileri sorumlu tutulamaz.");
                    Console.WriteLine();
                    Console.WriteLine($"Mevcut surum: {OSVERSION}");
                    Console.WriteLine();
                    Console.WriteLine("----------------------------------------------");
                    Console.WriteLine("[ENTER] Okudum ve kabul ediyorum");
                    Console.WriteLine("[ESC]   Cikis");
                    Console.WriteLine("==============================================");

                    ConsoleKey key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Enter)
                    {

                        string date =$"{RTC.Year + (RTC.Century * 100):0000}-" +$"{RTC.Month:00}-" +$"{RTC.DayOfTheMonth:00} " +$"{RTC.Hour:00}:" +$"{RTC.Minute:00}:" +$"{RTC.Second:00}";

                        File.WriteAllText(licenseFile, $"HolmiumOS License Accepted\nVersion: {OSVERSION}\nDate: {date}");

                        break;
                    }

                    if (key == ConsoleKey.Escape)
                    {
                        Console.ResetColor();
                        Console.Clear();
                        Console.WriteLine("HolmiumOS baslatilamadi. Lisans kabul edilmedi.");
                        Sys.Power.Shutdown();
                    }
                }

                Console.ResetColor();
                Console.Clear();
            }

            var mode = Boot.BootMenu.Show();

            if (mode == Boot.BootMode.GUI)
            {
                GUI.Init.Start();
                return;
            }

            Console.Clear();

            NetworkManager.Init();

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
            string path = FileSystemManager.GetDisplayPath();

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