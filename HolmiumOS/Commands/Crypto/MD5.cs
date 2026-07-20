using System;
using HolmiumOS.Crypto;

namespace HolmiumOS.Commands.Crypto
{
    public class Md5 : ICommand
    {
        public string Name => "md5";
        public string Description => "Verilen metnin MD5 hash degerini olusturur.\"";
        public string Usage => "md5 <metin>";

        public void Execute(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.WriteLine("Kullanım: " + Usage);
                return;
            }

            string hash = MD5.hash(args);

            Console.WriteLine(hash);
        }
    }
}