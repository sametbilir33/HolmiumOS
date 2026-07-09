using System;
using HolmiumOS.Commands;
using HolmiumOS.Crypto;

namespace HolmiumOS.Commands.Crypto
{
    public class VeridyMD5 : ICommand
    {
        public string Name => "md5verify";
        public string Description => "Metnin MD5 hash degerini dogrular.";
        public string Usage => "md5verify <metin> <hash>";

        public void Execute(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                Console.WriteLine("Kullanım: " + Usage);
                return;
            }

            string[] parts = args.Split(' ');

            if (parts.Length < 2)
            {
                Console.WriteLine("Kullanım: " + Usage);
                return;
            }

            string text = parts[0];
            string expectedHash = parts[1];

            string calculatedHash = MD5.hash(text);

            if (calculatedHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Gecerli");
            }
            else
            {
                Console.WriteLine("Gecersiz");
            }
        }
    }
}