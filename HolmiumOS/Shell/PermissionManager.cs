using System;

namespace HolmiumOS.Shell
{
    public static class PermissionManager
    {
        public static bool IsElevated { get; set; }

        public static bool IsRoot =>
            UserManager.IsRoot || IsElevated;

        public static bool CanRead(string path)
        {
            path = Normalize(path);

            if (IsRoot)
                return true;

            if (IsShadow(path))
                return false;

            if (IsSystem(path))
                return false;

            if (IsOwnHome(path))
                return true;

            if (IsOtherHome(path))
                return false;

            return true;
        }

        public static bool CanWrite(string path)
        {
            path = Normalize(path);

            if (IsRoot)
                return true;

            if (IsSystem(path))
                return false;

            if (IsEtc(path))
                return false;

            if (IsOwnHome(path))
                return true;

            if (IsOtherHome(path))
                return false;

            return false;
        }

        public static bool CanDelete(string path)
        {
            return CanWrite(path);
        }

        public static bool CanCreate(string path)
        {
            return CanWrite(path);
        }

        public static bool CanEnter(string path)
        {
            path = Normalize(path);

            if (IsRoot)
                return true;

            if (IsSystem(path))
                return false;

            if (IsOwnHome(path))
                return true;

            if (IsOtherHome(path))
                return false;

            return true;
        }

        private static bool IsOwnHome(string path)
        {
            string home = Normalize($@"0:\home\{UserManager.CurrentUser}");

            return path.Equals(home, StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(home + @"\", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsOtherHome(string path)
        {
            path = Normalize(path);

            if (!path.StartsWith(@"0:\home", StringComparison.OrdinalIgnoreCase))
                return false;

            return !IsOwnHome(path);
        }

        private static bool IsSystem(string path)
        {
            return path.StartsWith(@"0:\system", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsEtc(string path)
        {
            return path.StartsWith(@"0:\etc", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsShadow(string path)
        {
            return path.Equals(@"0:\etc\shadow", StringComparison.OrdinalIgnoreCase);
        }

        private static string Normalize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return @"0:\";

            return path.TrimEnd('\\');
        }
    }
}