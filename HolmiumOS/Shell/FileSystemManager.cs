using System;
using System.IO;
using System.Linq;
using Cosmos.System.FileSystem.VFS;

namespace HolmiumOS.Shell
{
    public static class FileSystemManager
    {
        public static string CurrentDirectory { get; set; } = @"0:\";

        private static readonly Random deviceRandom = new Random();

        private const string DevNull = @"0:\dev\null";
        private const string DevZero = @"0:\dev\zero";
        private const string DevRandom = @"0:\dev\random";

        public static string ResolvePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return CurrentDirectory;

            path = path.Replace('/', '\\');

            if (path == "~")
                return UserManager.HomeDirectory;

            if (path.StartsWith(@"~\"))
                return Path.Combine(UserManager.HomeDirectory, path.Substring(2));

            if (path.StartsWith(@"\"))
                return @"0:\" + path.Substring(1);

            if (path == ".")
                return CurrentDirectory;

            if (path == "..")
            {
                string dir = CurrentDirectory.TrimEnd('\\');

                int index = dir.LastIndexOf('\\');

                if (index <= 2)
                    return @"0:\";

                return dir.Substring(0, index + 1);
            }

            if (Path.IsPathRooted(path))
                return path;

            return Path.Combine(CurrentDirectory, path);
        }

        private static bool IsDeviceFile(string path)
        {
            return path.Equals(DevNull, StringComparison.OrdinalIgnoreCase)
                || path.Equals(DevZero, StringComparison.OrdinalIgnoreCase)
                || path.Equals(DevRandom, StringComparison.OrdinalIgnoreCase);
        }

        public static bool DirectoryExists(string path)
        {
            return VFSManager.DirectoryExists(ResolvePath(path));
        }

        public static bool FileExists(string path)
        {
            string resolved = ResolvePath(path);

            if (IsDeviceFile(resolved))
                return true;

            return VFSManager.FileExists(resolved);
        }

        public static bool ChangeDirectory(string path)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanEnter(path))
                throw new UnauthorizedAccessException("Permission denied.");

            if (!VFSManager.DirectoryExists(path))
                return false;

            if (!path.EndsWith("\\"))
                path += "\\";

            CurrentDirectory = path;

            return true;
        }

        public static void CreateDirectory(string path)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanCreate(path))
                throw new UnauthorizedAccessException("Permission denied.");

            if (Directory.Exists(path) || File.Exists(path))
                throw new IOException("Ayni adda dosya veya klasor zaten var.");

            Directory.CreateDirectory(path);
        }

        public static void DeleteDirectory(string path, bool recursive = true)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanDelete(path))
                throw new UnauthorizedAccessException("Permission denied.");

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();

            Directory.Delete(path, recursive);
        }

        public static void CreateFile(string path)
        {
            path = ResolvePath(path);

            if (IsDeviceFile(path))
                throw new IOException("Bu bir aygit dosyasidir, olusturulamaz.");

            if (!PermissionManager.CanCreate(path))
                throw new UnauthorizedAccessException("Permission denied.");

            if (File.Exists(path))
                throw new IOException("Dosya zaten var.");

            File.Create(path).Dispose();
        }

        public static void DeleteFile(string path)
        {
            path = ResolvePath(path);

            if (IsDeviceFile(path))
                throw new IOException("Bu bir aygit dosyasidir, silinemez.");

            if (!PermissionManager.CanDelete(path))
                throw new UnauthorizedAccessException("Permission denied.");

            if (!File.Exists(path))
                throw new FileNotFoundException();

            File.Delete(path);
        }

        public static string ReadFile(string path)
        {
            path = ResolvePath(path);

            if (path.Equals(DevNull, StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            if (path.Equals(DevZero, StringComparison.OrdinalIgnoreCase))
                return new string('0', 64);

            if (path.Equals(DevRandom, StringComparison.OrdinalIgnoreCase))
            {
                char[] chars = new char[32];

                for (int i = 0; i < chars.Length; i++)
                    chars[i] = (char)('0' + deviceRandom.Next(0, 10));

                return new string(chars);
            }

            if (!PermissionManager.CanRead(path))
                throw new UnauthorizedAccessException("Permission denied.");

            if (!File.Exists(path))
                throw new FileNotFoundException();

            return File.ReadAllText(path);
        }

        public static byte[] ReadBytes(string path)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanRead(path))
                throw new UnauthorizedAccessException("Permission denied.");

            if (!File.Exists(path))
                throw new FileNotFoundException();

            return File.ReadAllBytes(path);
        }
        public static void WriteFile(string path, string content)
        {
            path = ResolvePath(path);

            if (IsDeviceFile(path))
                return; // dev/null, dev/zero, dev/random -> yazma yok sayilir

            if (!PermissionManager.CanWrite(path))
                throw new UnauthorizedAccessException("Permission denied.");

            File.WriteAllText(path, content);
        }

        public static void AppendFile(string path, string content)
        {
            path = ResolvePath(path);

            if (IsDeviceFile(path))
                return;

            if (!PermissionManager.CanWrite(path))
                throw new UnauthorizedAccessException("Permission denied.");

            File.AppendAllText(path, content);
        }

        public static void CopyFile(string source, string destination, bool overwrite = true)
        {
            source = ResolvePath(source);
            destination = ResolvePath(destination);

            if (!PermissionManager.CanRead(source))
                throw new UnauthorizedAccessException("Permission denied.");

            if (!PermissionManager.CanCreate(destination))
                throw new UnauthorizedAccessException("Permission denied.");

            File.Copy(source, destination, overwrite);
        }

        public static void MoveFile(string source, string destination)
        {
            source = ResolvePath(source);
            destination = ResolvePath(destination);

            if (!PermissionManager.CanDelete(source))
                throw new UnauthorizedAccessException("Permission denied.");

            if (!PermissionManager.CanCreate(destination))
                throw new UnauthorizedAccessException("Permission denied.");

            File.Copy(source, destination, true);
            File.Delete(source);
        }

        public static string[] GetFiles(string path = null)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanRead(path))
                throw new UnauthorizedAccessException("Permission denied.");

            return Directory.GetFiles(path);
        }

        public static string[] GetDirectories(string path = null)
        {
            path = ResolvePath(path);

            if (!PermissionManager.CanRead(path))
                throw new UnauthorizedAccessException("Permission denied.");

            return Directory.GetDirectories(path);
        }

        public static string GetDisplayPath()
        {
            string path = CurrentDirectory.TrimEnd('\\');
            string home = UserManager.HomeDirectory.TrimEnd('\\');

            if (path.Equals(home, StringComparison.OrdinalIgnoreCase))
                return "~";

            if (path.StartsWith(home + "\\", StringComparison.OrdinalIgnoreCase))
                return "~" + path.Substring(home.Length).Replace('\\', '/');

            if (path.Equals(@"0:", StringComparison.OrdinalIgnoreCase))
                return "/";

            if (path.Equals(@"0:\", StringComparison.OrdinalIgnoreCase))
                return "/";

            if (path.StartsWith(@"0:\", StringComparison.OrdinalIgnoreCase))
                return "/" + path.Substring(3).Replace('\\', '/');

            return path.Replace('\\', '/');
        }
    }
}