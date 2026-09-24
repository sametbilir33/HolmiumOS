using System;
using System.IO;

namespace HolmiumOS.Shell
{
    public static class FileSystemManager
    {
        private static string currentDirectory = "/";

        public static ShellContext ActiveContext { get; set; }

        public static string CurrentDirectory
        {
            get
            {
                if (ActiveContext != null)
                    return ActiveContext.CurrentDirectory;

                return currentDirectory;
            }

            set
            {
                if (ActiveContext != null)
                {
                    ActiveContext.CurrentDirectory = value;
                    return;
                }

                currentDirectory = string.IsNullOrWhiteSpace(value)
                    ? "/"
                    : value;
            }
        }

        private static readonly Random deviceRandom = new Random();

        private const string DevNull = "/dev/null";
        private const string DevZero = "/dev/zero";
        private const string DevRandom = "/dev/random";

        public static string ResolvePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return CurrentDirectory;

            path = path.Trim();

            if (path == "~")
                return UserManager.HomeDirectory;

            if (path.StartsWith("~/", StringComparison.Ordinal))
                return Path.Combine(
                    UserManager.HomeDirectory,
                    path.Substring(2));

            if (path == ".")
                return CurrentDirectory;

            if (path == "..")
                return GetParentDirectory(CurrentDirectory);

            if (path.StartsWith("../", StringComparison.Ordinal))
            {
                string resolved = CurrentDirectory;

                while (path.StartsWith("../", StringComparison.Ordinal))
                {
                    resolved = GetParentDirectory(resolved);
                    path = path.Substring(3);
                }

                if (path == "..")
                    return GetParentDirectory(resolved);

                if (path.Length == 0)
                    return resolved;

                return CombinePath(resolved, path);
            }

            if (path.StartsWith("/", StringComparison.Ordinal))
                return NormalizePath(path);

            return CombinePath(CurrentDirectory, path);
        }

        private static string CombinePath(string basePath, string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return NormalizePath(basePath);

            if (relativePath.StartsWith("/", StringComparison.Ordinal))
                return NormalizePath(relativePath);

            if (string.IsNullOrEmpty(basePath) || basePath == "/")
                return NormalizePath("/" + relativePath);

            return NormalizePath(basePath.TrimEnd('/') + "/" + relativePath);
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "/";

            string[] parts = path.Split(
                '/',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return "/";

            string[] normalized = new string[parts.Length];
            int count = 0;

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];

                if (part == ".")
                    continue;

                if (part == "..")
                {
                    if (count > 0)
                        count--;

                    continue;
                }

                normalized[count++] = part;
            }

            if (count == 0)
                return "/";

            string result = "/";

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    result += "/";

                result += normalized[i];
            }

            return result;
        }

        private static string GetParentDirectory(string path)
        {
            string normalized = NormalizePath(path);

            if (normalized == "/")
                return "/";

            int index = normalized.LastIndexOf('/');

            if (index <= 0)
                return "/";

            return normalized.Substring(0, index);
        }

        private static bool IsDeviceFile(string path)
        {
            return path.Equals(
                       DevNull,
                       StringComparison.OrdinalIgnoreCase)
                   || path.Equals(
                       DevZero,
                       StringComparison.OrdinalIgnoreCase)
                   || path.Equals(
                       DevRandom,
                       StringComparison.OrdinalIgnoreCase);
        }

        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(ResolvePath(path));
        }

        public static bool FileExists(string path)
        {
            string resolved = ResolvePath(path);

            if (IsDeviceFile(resolved))
                return true;

            return File.Exists(resolved);
        }

        public static bool ChangeDirectory(string path)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanEnter(path))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!Directory.Exists(path))
                return false;

            CurrentDirectory = NormalizePath(path);

            return true;
        }

        public static void CreateDirectory(string path)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanCreate(path))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (Directory.Exists(path) || File.Exists(path))
                throw new IOException(
                    "Ayni adda dosya veya klasor zaten var.");

            Directory.CreateDirectory(path);
        }

        public static void DeleteDirectory(
            string path,
            bool recursive = true)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanDelete(path))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();

            Directory.Delete(path, recursive);
        }

        public static void CreateFile(string path)
        {
            path = ResolvePath(path);

            if (IsDeviceFile(path))
                throw new IOException(
                    "Bu bir aygit dosyasidir, olusturulamaz.");

            if (!PermissionManager.CanCreate(path))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (File.Exists(path))
                throw new IOException("Dosya zaten var.");

            File.Create(path).Dispose();
        }

        public static void DeleteFile(string path)
        {
            path = ResolvePath(path);

            if (IsDeviceFile(path))
                throw new IOException(
                    "Bu bir aygit dosyasidir, silinemez.");

            if (!PermissionManager.CanDelete(path))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!File.Exists(path))
                throw new FileNotFoundException();

            File.Delete(path);
        }

        public static string ReadFile(string path)
        {
            path = ResolvePath(path);

            if (path.Equals(
                    DevNull,
                    StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if (path.Equals(
                    DevZero,
                    StringComparison.OrdinalIgnoreCase))
            {
                return new string('0', 64);
            }

            if (path.Equals(
                    DevRandom,
                    StringComparison.OrdinalIgnoreCase))
            {
                char[] chars = new char[32];

                for (int i = 0; i < chars.Length; i++)
                {
                    chars[i] =
                        (char)('0' + deviceRandom.Next(0, 10));
                }

                return new string(chars);
            }

            if (!PermissionManager.CanRead(path))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!File.Exists(path))
                throw new FileNotFoundException();

            return File.ReadAllText(path);
        }

        public static byte[] ReadBytes(string path)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanRead(path))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!File.Exists(path))
                throw new FileNotFoundException();

            return File.ReadAllBytes(path);
        }

        public static void WriteFile(string path, string content)
        {
            path = ResolvePath(path);

            if (IsDeviceFile(path))
                return;

            if (!PermissionManager.CanWrite(path))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            File.WriteAllText(path, content);
        }

        public static void AppendFile(string path, string content)
        {
            path = ResolvePath(path);

            if (IsDeviceFile(path))
                return;

            if (!PermissionManager.CanWrite(path))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            File.AppendAllText(path, content);
        }

        public static void CopyFile(
            string source,
            string destination,
            bool overwrite = true)
        {
            source = ResolvePath(source);
            destination = ResolvePath(destination);

            if (!PermissionManager.CanRead(source))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!PermissionManager.CanCreate(destination))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            File.Copy(source, destination, overwrite);
        }

        public static void MoveFile(
            string source,
            string destination)
        {
            source = ResolvePath(source);
            destination = ResolvePath(destination);

            if (!PermissionManager.CanDelete(source))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!PermissionManager.CanCreate(destination))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            File.Copy(source, destination, true);
            File.Delete(source);
        }

        public static string[] GetFiles(string path = null)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanRead(path))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            return Directory.GetFiles(path);
        }

        public static string[] GetDirectories(string path = null)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanRead(path))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            return Directory.GetDirectories(path);
        }

        public static string GetDisplayPath()
        {
            string path = NormalizePath(CurrentDirectory);
            string home = NormalizePath(UserManager.HomeDirectory);

            if (path.Equals(
                    home,
                    StringComparison.OrdinalIgnoreCase))
            {
                return "~";
            }

            if (path.StartsWith(
                    home + "/",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "~" + path.Substring(home.Length);
            }

            if (path == "/")
                return "/";

            return path;
        }
    }
}