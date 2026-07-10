using Cosmos.HAL;

namespace HolmiumOS.Crypto
{
    public static class PasswordHasher
    {
        private static int SaltCounter;

        public static string CreateHash(string password)
        {
            string salt = GenerateSalt();

            string hash = Sha256.hash(salt + password);

            return salt + ":" + hash;
        }

        public static bool Verify(string password, string storedValue)
        {
            string[] split = storedValue.Split(':');

            if (split.Length != 2)
                return false;

            string salt = split[0];
            string storedHash = split[1];

            string hash = Sha256.hash(salt + password);

            return hash.Equals(storedHash, System.StringComparison.Ordinal);
        }

        private static string GenerateSalt()
        {
            SaltCounter++;

            string entropy =
                RTC.Year.ToString() +
                RTC.Month.ToString() +
                RTC.DayOfTheMonth.ToString() +
                RTC.Hour.ToString() +
                RTC.Minute.ToString() +
                RTC.Second.ToString() +
                SaltCounter.ToString();

            return Sha256.hash(entropy).Substring(0, 32);
        }
    }
}