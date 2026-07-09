using System;
using System.Text;
using HolmiumOS.Commands;
using HolmiumOS.Crypto;

namespace HolmiumOS.Commands.Crypto
{
    public class Sha256 : ICommand
    {
        public string Name => "sha256";
        public string Description => "Verilen metnin SHA-256 hash degerini olusturur.";
        public string Usage => "sha256 <metin>";
        
        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine("Kullanım: " + Usage);
                return;
            }

            string result = HolmiumOS.Crypto.Sha256.hash(args);

            Console.WriteLine(result);
        }
    }
}