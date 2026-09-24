using System;
using System.Text;

namespace HolmiumOS.Crypto
{
    public static class PasswordHasher
    {
        private static int SaltCounter;

        public static string CreateHash(string password)
        {
            string salt = GenerateSalt();

            string hash = ComputeHash(salt + password);

            return salt + ":" + hash;
        }

        public static bool Verify(string password, string storedValue)
        {
            if (string.IsNullOrEmpty(storedValue))
                return false;

            string[] split = storedValue.Split(':');

            if (split.Length != 2)
                return false;

            string salt = split[0];
            string storedHash = split[1];

            string hash = ComputeHash(salt + password);

            return hash.Equals(
                storedHash,
                StringComparison.OrdinalIgnoreCase);
        }

        private static string GenerateSalt()
        {
            SaltCounter++;

            string entropy =
                DateTime.UtcNow.Ticks.ToString() +
                ":" +
                SaltCounter.ToString();

            return ComputeHash(entropy).Substring(0, 32);
        }

        private static string ComputeHash(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);

            return Sha256.ComputeHash(bytes).ToLowerInvariant();
        }
    }
}