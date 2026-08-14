using System;
using System.IO;
using HolmiumOS.HE;
using HolmiumOS.Shell;

namespace HolmiumOS.Commands.Executable
{
    public class Run : ICommand
    {
        public string Name => "run";
        public string Description => "HE script calistirir.";
        public string Usage => "run <dosya.he>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine("Kullanim: run <dosya.he>");
                return;
            }

            string file = FileSystemManager.ResolvePath(args.Trim());

            if (!string.Equals(Path.GetExtension(file), ".he", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Sadece .he uzantili dosyalar calistirilabilir.");
                return;
            }

            if (!PermissionManager.CanRead(file))
            {
                Console.WriteLine("Bu dosyayi calistirma/okuma yetkiniz yok.");
                return;
            }

            if (!File.Exists(file))
            {
                Console.WriteLine("Dosya bulunamadi.");
                return;
            }

            var interpreter = new HeInterpreter();
            interpreter.Run(file);
        }
    }
}