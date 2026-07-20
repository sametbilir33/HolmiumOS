using System;
using System.Collections.Generic;
using System.IO;
using HolmiumOS.Crypto;

namespace HolmiumOS.Shell
{
    public static class UserManager
    {
        private const string PasswdFile = @"0:\etc\passwd";
        private const string ShadowFile = @"0:\etc\shadow";

        public static string CurrentUser { get; private set; }
        public static bool IsRoot => CurrentUser.Equals("root", StringComparison.OrdinalIgnoreCase);
        public static bool IsLoggedIn => !string.IsNullOrWhiteSpace(CurrentUser);
        public static string HomeDirectory => $@"0:\home\{CurrentUser}";

        public static void Logout()
        {
            CurrentUser = string.Empty;
        }

        public static bool Login(string username, string password)
        {
            username = username.ToLower();

            if (!VerifyPassword(username, password))
                return false;

            SwitchUser(username);
            return true;
        }

        public static bool VerifyPassword(string username, string password)
        {
            Dictionary<string, string> passwords = LoadPasswords();

            return passwords.TryGetValue(username.ToLower(), out string storedPassword)
                && PasswordHasher.Verify(password, storedPassword);
        }

        public static void SwitchUser(string username)
        {
            CurrentUser = username.ToLower();
        }

        public static bool UserExists(string username)
        {
            username = username.ToLower();

            if (!File.Exists(PasswdFile))
                return false;

            foreach (string line in File.ReadAllLines(PasswdFile))
            {
                if (line.Trim().Equals(username, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public static bool CreateUser(string username, string password)
        {
            username = username.ToLower();

            if (!IsValidUsername(username, out _))
                return false;

            if (UserExists(username))
                return false;

            Directory.CreateDirectory(@"0:\etc");
            Directory.CreateDirectory(@"0:\home");
            Directory.CreateDirectory($@"0:\home\{username}");

            File.AppendAllText(PasswdFile, username + Environment.NewLine);
            File.AppendAllText(ShadowFile, username + ":" + PasswordHasher.CreateHash(password) + Environment.NewLine);

            return true;
        }

        public static bool DeleteUser(string username)
        {
            username = username.ToLower();

            if (username == "root")
                return false;

            if (!UserExists(username))
                return false;

            List<string> passwd = new List<string>();

            foreach (string line in File.ReadAllLines(PasswdFile))
            {
                if (!line.Equals(username, StringComparison.OrdinalIgnoreCase))
                    passwd.Add(line);
            }

            File.WriteAllLines(PasswdFile, passwd);

            List<string> shadow = new List<string>();

            foreach (string line in File.ReadAllLines(ShadowFile))
            {
                if (!line.StartsWith(username + ":", StringComparison.OrdinalIgnoreCase))
                    shadow.Add(line);
            }

            File.WriteAllLines(ShadowFile, shadow);

            string home = $@"0:\home\{username}";

            if (Directory.Exists(home))
                Directory.Delete(home, true);

            return true;
        }

        public static bool ChangePassword(string username, string newPassword)
        {
            username = username.ToLower();

            if (!File.Exists(ShadowFile))
                return false;

            bool found = false;
            List<string> lines = new List<string>();

            foreach (string line in File.ReadAllLines(ShadowFile))
            {
                string[] split = line.Split(':');

                if (split.Length != 3)
                    continue;

                if (split[0].Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    lines.Add(username + ":" + PasswordHasher.CreateHash(newPassword));
                    found = true;
                }
                else
                {
                    lines.Add(line);
                }
            }

            if (!found)
                return false;

            File.WriteAllLines(ShadowFile, lines);

            return true;
        }

        public static bool CreateRoot(string password)
        {
            if (UserExists("root"))
                return false;

            return CreateUser("root", password);
        }

        public static IEnumerable<string> GetUsers()
        {
            if (!File.Exists(PasswdFile))
                yield break;

            foreach (string line in File.ReadAllLines(PasswdFile))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    yield return line.Trim();
            }
        }

        public static bool IsValidUsername(string username, out string error)
        {
            error = "";

            if (string.IsNullOrWhiteSpace(username))
            {
                error = "Kullanici adi bos olamaz.";
                return false;
            }

            username = username.Trim().ToLower();

            if (username.Length < 3)
            {
                error = "Kullanici adi en az 3 karakter olmalidir.";
                return false;
            }

            if (username.Length > 32)
            {
                error = "Kullanici adi en fazla 32 karakter olabilir.";
                return false;
            }

            if (username == "." || username == "..")
            {
                error = "Gecersiz kullanici adi.";
                return false;
            }

            foreach (char c in username)
            {
                bool valid =
                    (c >= 'a' && c <= 'z') ||
                    (c >= '0' && c <= '9') ||
                    c == '_' ||
                    c == '-';

                if (!valid)
                {
                    error = "Kullanici adi sadece a-z, 0-9, '_' ve '-' karakterlerini icerebilir.";
                    return false;
                }
            }

            return true;
        }

        private static Dictionary<string, string> LoadPasswords()
        {
            Dictionary<string, string> users = new Dictionary<string, string>();

            if (!File.Exists(ShadowFile))
                return users;

            foreach (string line in File.ReadAllLines(ShadowFile))
            {
                string[] split = line.Split(':');

                if (split.Length != 3)
                    continue;

                users[split[0].ToLower()] = split[1] + ":" + split[2];
            }

            return users;
        }
    }
}