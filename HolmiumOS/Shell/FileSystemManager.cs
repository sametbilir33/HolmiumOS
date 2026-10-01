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

                currentDirectory = string.IsNullOrWhiteSpace(value) ? "/" : NormalizePath(value);
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
                return NormalizePath(UserManager.HomeDirectory);

            if (path.StartsWith("~/", StringComparison.Ordinal))
                return CombinePath(UserManager.HomeDirectory, path.Substring(2));

            if (path == ".")
                return NormalizePath(CurrentDirectory);

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

            string[] parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

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
            return path.Equals(DevNull, StringComparison.OrdinalIgnoreCase) ||
                   path.Equals(DevZero, StringComparison.OrdinalIgnoreCase) ||
                   path.Equals(DevRandom, StringComparison.OrdinalIgnoreCase);
        }

        public static bool DirectoryExists(string path)
        {
            string virtualPath = ResolvePath(path);
            return Directory.Exists(virtualPath);
        }

        public static bool FileExists(string path)
        {
            string virtualPath = ResolvePath(path);

            if (IsDeviceFile(virtualPath))
                return true;

            return File.Exists(virtualPath);
        }

        public static bool ChangeDirectory(string path)
        {
            string virtualPath = ResolvePath(path);

            if (!PermissionManager.CanEnter(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!Directory.Exists(virtualPath))
                return false;

            CurrentDirectory = virtualPath;
            return true;
        }

        public static void CreateDirectory(string path)
        {
            string virtualPath = ResolvePath(path);

            if (!PermissionManager.CanCreate(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (Directory.Exists(virtualPath) || File.Exists(virtualPath))
                throw new IOException("Ayni adda dosya veya klasor zaten var.");

            Directory.CreateDirectory(virtualPath);
        }

        public static void DeleteDirectory(string path, bool recursive = true)
        {
            string virtualPath = ResolvePath(path);

            if (!PermissionManager.CanDelete(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!Directory.Exists(virtualPath))
                throw new DirectoryNotFoundException();

            Directory.Delete(virtualPath, recursive);
        }

        public static void CreateFile(string path)
        {
            string virtualPath = ResolvePath(path);

            if (IsDeviceFile(virtualPath))
                throw new IOException("Bu bir aygit dosyasidir, olusturulamaz.");

            if (!PermissionManager.CanCreate(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (File.Exists(virtualPath))
                throw new IOException("Dosya zaten var.");

            File.Create(virtualPath).Dispose();
        }

        public static void DeleteFile(string path)
        {
            string virtualPath = ResolvePath(path);

            if (IsDeviceFile(virtualPath))
                throw new IOException("Bu bir aygit dosyasidir, silinemez.");

            if (!PermissionManager.CanDelete(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!File.Exists(virtualPath))
                throw new FileNotFoundException();

            File.Delete(virtualPath);
        }

        public static string ReadFile(string path)
        {
            string virtualPath = ResolvePath(path);

            if (virtualPath.Equals(DevNull, StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            if (virtualPath.Equals(DevZero, StringComparison.OrdinalIgnoreCase))
                return new string('0', 64);

            if (virtualPath.Equals(DevRandom, StringComparison.OrdinalIgnoreCase))
            {
                char[] chars = new char[32];

                for (int i = 0; i < chars.Length; i++)
                    chars[i] = (char)('0' + deviceRandom.Next(0, 10));

                return new string(chars);
            }

            if (!PermissionManager.CanRead(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!File.Exists(virtualPath))
                throw new FileNotFoundException();

            return File.ReadAllText(virtualPath);
        }

        public static byte[] ReadBytes(string path)
        {
            string virtualPath = ResolvePath(path);

            if (!PermissionManager.CanRead(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!File.Exists(virtualPath))
                throw new FileNotFoundException();

            return File.ReadAllBytes(virtualPath);
        }

        public static void WriteFile(string path, string content)
        {
            string virtualPath = ResolvePath(path);

            if (IsDeviceFile(virtualPath))
                return;

            if (!PermissionManager.CanWrite(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            File.WriteAllText(virtualPath, content);
        }

        public static void WriteBytes(string path, byte[] data)
        {
            string virtualPath = ResolvePath(path);

            if (!PermissionManager.CanWrite(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            File.WriteAllBytes(virtualPath, data);
        }

        public static void AppendFile(string path, string content)
        {
            string virtualPath = ResolvePath(path);

            if (IsDeviceFile(virtualPath))
                return;

            if (!PermissionManager.CanWrite(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            File.AppendAllText(virtualPath, content);
        }

        public static void CopyFile(string source, string destination, bool overwrite = true)
        {
            string sourceVirtual = ResolvePath(source);
            string destinationVirtual = ResolvePath(destination);

            if (!PermissionManager.CanRead(sourceVirtual))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!PermissionManager.CanCreate(destinationVirtual))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            File.Copy(sourceVirtual, destinationVirtual, overwrite);
        }

        public static void MoveFile(string source, string destination)
        {
            string sourceVirtual = ResolvePath(source);
            string destinationVirtual = ResolvePath(destination);

            if (!PermissionManager.CanDelete(sourceVirtual))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            if (!PermissionManager.CanCreate(destinationVirtual))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            File.Copy(sourceVirtual, destinationVirtual, true);
            File.Delete(sourceVirtual);
        }

        public static string[] GetFiles(string path = null)
        {
            string virtualPath = ResolvePath(path);

            if (!PermissionManager.CanRead(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            return Directory.GetFiles(virtualPath);
        }

        public static string[] GetDirectories(string path = null)
        {
            string virtualPath = ResolvePath(path);

            if (!PermissionManager.CanRead(virtualPath))
                throw new UnauthorizedAccessException("Erisim reddedildi.");

            return Directory.GetDirectories(virtualPath);
        }

        public static string GetDisplayPath()
        {
            string path = NormalizePath(CurrentDirectory);
            string home = NormalizePath(UserManager.HomeDirectory);

            if (path.Equals(home, StringComparison.OrdinalIgnoreCase))
                return "~";

            if (path.StartsWith(home + "/", StringComparison.OrdinalIgnoreCase))
                return "~" + path.Substring(home.Length);

            if (path == "/")
                return "/";

            return path;
        }
    }
}