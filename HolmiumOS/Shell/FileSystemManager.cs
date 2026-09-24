using System;
using System.IO;

namespace HolmiumOS.Shell
{
    public static class FileSystemManager
    {
        private static string currentDirectory = "/";

        public static ShellContext ActiveContext { get; set; }

        /*
         * Kullanıcının gördüğü sanal filesystem:
         *
         * /
         * /home
         * /home/samet
         * /dev
         * /tmp
         *
         * Cosmos'un gerçek filesystem'i:
         *
         * /mnt
         * /mnt/home
         * /mnt/home/samet
         * /mnt/dev
         * /mnt/tmp
         */

        private const string MountPoint = "/mnt";

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
                    : NormalizePath(value);
            }
        }

        private static readonly Random deviceRandom = new Random();

        private const string DevNull = "/dev/null";
        private const string DevZero = "/dev/zero";
        private const string DevRandom = "/dev/random";

        /*
         * Kullanıcı tarafından verilen yolu
         * sanal filesystem yoluna çevirir.
         *
         * Örnek:
         *
         * "test.txt"       -> "/home/samet/test.txt"
         * "~"              -> "/home/samet"
         * "~/test.txt"     -> "/home/samet/test.txt"
         * "/tmp/test.txt"  -> "/tmp/test.txt"
         */
        public static string ResolvePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return CurrentDirectory;

            path = path.Trim();

            if (path == "~")
                return NormalizePath(UserManager.HomeDirectory);

            if (path.StartsWith("~/", StringComparison.Ordinal))
            {
                return CombinePath(
                    UserManager.HomeDirectory,
                    path.Substring(2));
            }

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

        /*
         * Sanal filesystem yolunu Cosmos'un gerçek
         * /mnt filesystem yoluna çevirir.
         *
         * "/"               -> "/mnt"
         * "/home"           -> "/mnt/home"
         * "/home/samet"     -> "/mnt/home/samet"
         * "/dev/null"       -> "/mnt/dev/null"
         */
        private static string ToRealPath(string virtualPath)
{
    virtualPath = NormalizePath(virtualPath);

    if (virtualPath == "/")
        return "/mnt";

    if (virtualPath.StartsWith("/mnt/", StringComparison.Ordinal))
        return virtualPath;

    return "/mnt" + virtualPath;
}

        private static string CombinePath(
            string basePath,
            string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return NormalizePath(basePath);

            if (relativePath.StartsWith("/", StringComparison.Ordinal))
                return NormalizePath(relativePath);

            if (string.IsNullOrEmpty(basePath) || basePath == "/")
                return NormalizePath("/" + relativePath);

            return NormalizePath(
                basePath.TrimEnd('/') + "/" + relativePath);
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
            string virtualPath = ResolvePath(path);
            string realPath = ToRealPath(virtualPath);

            return Directory.Exists(realPath);
        }

        public static bool FileExists(string path)
        {
            string virtualPath = ResolvePath(path);

            if (IsDeviceFile(virtualPath))
                return true;

            string realPath = ToRealPath(virtualPath);

            return File.Exists(realPath);
        }

        public static bool ChangeDirectory(string path)
        {
            string virtualPath = ResolvePath(path);
            string realPath = ToRealPath(virtualPath);

            if (!PermissionManager.CanEnter(virtualPath))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            if (!Directory.Exists(realPath))
                return false;

            CurrentDirectory = virtualPath;

            return true;
        }

        public static void CreateDirectory(string path)
        {
            string virtualPath = ResolvePath(path);
            string realPath = ToRealPath(virtualPath);

            if (!PermissionManager.CanCreate(virtualPath))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            if (Directory.Exists(realPath) ||
                File.Exists(realPath))
            {
                throw new IOException(
                    "Ayni adda dosya veya klasor zaten var.");
            }

            Directory.CreateDirectory(realPath);
        }

        public static void DeleteDirectory(
            string path,
            bool recursive = true)
        {
            string virtualPath = ResolvePath(path);
            string realPath = ToRealPath(virtualPath);

            if (!PermissionManager.CanDelete(virtualPath))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            if (!Directory.Exists(realPath))
                throw new DirectoryNotFoundException();

            Directory.Delete(realPath, recursive);
        }

        public static void CreateFile(string path)
        {
            string virtualPath = ResolvePath(path);

            if (IsDeviceFile(virtualPath))
            {
                throw new IOException(
                    "Bu bir aygit dosyasidir, olusturulamaz.");
            }

            string realPath = ToRealPath(virtualPath);

            if (!PermissionManager.CanCreate(virtualPath))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            if (File.Exists(realPath))
                throw new IOException("Dosya zaten var.");

            File.Create(realPath).Dispose();
        }

        public static void DeleteFile(string path)
        {
            string virtualPath = ResolvePath(path);

            if (IsDeviceFile(virtualPath))
            {
                throw new IOException(
                    "Bu bir aygit dosyasidir, silinemez.");
            }

            string realPath = ToRealPath(virtualPath);

            if (!PermissionManager.CanDelete(virtualPath))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            if (!File.Exists(realPath))
                throw new FileNotFoundException();

            File.Delete(realPath);
        }

        public static string ReadFile(string path)
        {
            string virtualPath = ResolvePath(path);

            if (virtualPath.Equals(
                    DevNull,
                    StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if (virtualPath.Equals(
                    DevZero,
                    StringComparison.OrdinalIgnoreCase))
            {
                return new string('0', 64);
            }

            if (virtualPath.Equals(
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

            if (!PermissionManager.CanRead(virtualPath))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            string realPath = ToRealPath(virtualPath);

            if (!File.Exists(realPath))
                throw new FileNotFoundException();

            return File.ReadAllText(realPath);
        }

        public static byte[] ReadBytes(string path)
        {
            string virtualPath = ResolvePath(path);

            if (!PermissionManager.CanRead(virtualPath))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            string realPath = ToRealPath(virtualPath);

            if (!File.Exists(realPath))
                throw new FileNotFoundException();

            return File.ReadAllBytes(realPath);
        }

        public static void WriteFile(
            string path,
            string content)
        {
            string virtualPath = ResolvePath(path);

            if (IsDeviceFile(virtualPath))
                return;

            if (!PermissionManager.CanWrite(virtualPath))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            string realPath = ToRealPath(virtualPath);

            File.WriteAllText(realPath, content);
        }

public static void WriteBytes(string path, byte[] data)
{
    string virtualPath = ResolvePath(path);

    if (!PermissionManager.CanWrite(virtualPath))
        throw new UnauthorizedAccessException(
            "Erisim reddedildi.");

    string realPath = ToRealPath(virtualPath);

    File.WriteAllBytes(realPath, data);
}

        public static void AppendFile(
            string path,
            string content)
        {
            string virtualPath = ResolvePath(path);

            if (IsDeviceFile(virtualPath))
                return;

            if (!PermissionManager.CanWrite(virtualPath))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            string realPath = ToRealPath(virtualPath);

            File.AppendAllText(realPath, content);
        }

        public static void CopyFile(
            string source,
            string destination,
            bool overwrite = true)
        {
            string sourceVirtual = ResolvePath(source);
            string destinationVirtual = ResolvePath(destination);

            if (!PermissionManager.CanRead(sourceVirtual))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            if (!PermissionManager.CanCreate(destinationVirtual))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            string sourceReal = ToRealPath(sourceVirtual);
            string destinationReal = ToRealPath(destinationVirtual);

            File.Copy(
                sourceReal,
                destinationReal,
                overwrite);
        }

        public static void MoveFile(
            string source,
            string destination)
        {
            string sourceVirtual = ResolvePath(source);
            string destinationVirtual = ResolvePath(destination);

            if (!PermissionManager.CanDelete(sourceVirtual))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            if (!PermissionManager.CanCreate(destinationVirtual))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            string sourceReal = ToRealPath(sourceVirtual);
            string destinationReal = ToRealPath(destinationVirtual);

            File.Copy(
                sourceReal,
                destinationReal,
                true);

            File.Delete(sourceReal);
        }

        public static string[] GetFiles(string path = null)
        {
            string virtualPath = ResolvePath(path);

            if (!PermissionManager.CanRead(virtualPath))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            string realPath = ToRealPath(virtualPath);

            return Directory.GetFiles(realPath);
        }

        public static string[] GetDirectories(string path = null)
        {
            string virtualPath = ResolvePath(path);

            if (!PermissionManager.CanRead(virtualPath))
                throw new UnauthorizedAccessException(
                    "Erisim reddedildi.");

            string realPath = ToRealPath(virtualPath);

            return Directory.GetDirectories(realPath);
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