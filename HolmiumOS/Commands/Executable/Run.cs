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

            string file = ResolvePath(args.Trim());

            if (Path.GetExtension(file).ToLower() != ".he")
            {
                Console.WriteLine("Sadece .he uzantili dosyalar calistirilabilir.");
                return;
            }

            var interpreter = new HeInterpreter();
            interpreter.Run(file);
        }

        private string ResolvePath(string path)
        {
            if (path.Contains(":\\"))
                return path;

            string current = FileSystemManager.CurrentDirectory;

            if (!current.EndsWith("\\"))
                current += "\\";

            return current + path;
        }
    }
}