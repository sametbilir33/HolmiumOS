using System;
using HolmiumOS.Commands;
using HolmiumOS.Crypto;

namespace HolmiumOS.Commands.Crypto
{
    public class VerifySha256 : ICommand
    {
        public string Name => "verifysha256";
        public string Description => "Metnin SHA-256 hash degerini dogrular.";
        public string Usage => "verifysha256 <metin> <hash>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine("Kullanim: " + Usage);
                return;
            }

            string[] parts = args.Split(' ');

            if (parts.Length < 2)
            {
                Console.WriteLine("Kullanim: " + Usage);
                return;
            }

            string text = parts[0];
            string expectedHash = parts[1].ToUpperInvariant();

            string currentHash = HolmiumOS.Crypto.Sha256.hash(text);

            if (currentHash == expectedHash)
            {
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("FAILED");
            }
        }
    }
}