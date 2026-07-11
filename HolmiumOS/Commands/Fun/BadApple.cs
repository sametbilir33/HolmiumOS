using System;
using System.IO;
using System.Text;
using Cosmos.HAL;
using HolmiumOS.Commands;
using HolmiumOS.Shell;
using Syste = System;

namespace HolmiumOS.Commands.Fun
{
    public class BadApple : ICommand
    {
        public string Name => "badapple";
        public string Description => "Plays an ASCII video from a .bin file.";
        public string Usage => "badapple <path>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine($"Usage: {Usage}");
                return;
            }

            if (!File.Exists(args))
            {
                Console.WriteLine("File not found.");
                return;
            }

            using var reader = new BinaryReader(File.OpenRead(args));

            Console.CursorVisible = false;
            Console.Clear();

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                int length = reader.ReadInt32();
                byte[] data = reader.ReadBytes(length);

                Console.SetCursorPosition(0, 0);
                Console.Write(Encoding.ASCII.GetString(data));

                Syste.Threading.Thread.Sleep(24);
            }

            Console.CursorVisible = true;
            Console.Clear();
        }
    }
}