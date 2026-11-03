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

    if (IsShadow(path) || IsSystem(path) || IsOtherHome(path))
        return false;

    return true;
}

public static bool CanWrite(string path)
{
    path = Normalize(path);

    if (IsRoot)
        return true;

    if (IsSystem(path) || IsEtc(path) || IsOtherHome(path))
        return false;

    return IsOwnHome(path);
}

public static bool CanDelete(string path) => CanWrite(path);

public static bool CanCreate(string path) => CanWrite(path);

public static bool CanEnter(string path)
{
    path = Normalize(path);

    if (IsRoot)
        return true;

    if (IsSystem(path) || IsOtherHome(path))
        return false;

    return true;
}

        private static bool IsOwnHome(string path)
{
    string home = Normalize($"/mnt/home/{UserManager.CurrentUser}");

    return path.Equals(home, StringComparison.OrdinalIgnoreCase)
        || path.StartsWith(home + "/", StringComparison.OrdinalIgnoreCase);
}

private static bool IsOtherHome(string path)
{
    path = Normalize(path);

    const string homeRoot = "/mnt/home";

    if (path.Equals(homeRoot, StringComparison.OrdinalIgnoreCase))
        return false;

    if (!path.StartsWith(homeRoot + "/", StringComparison.OrdinalIgnoreCase))
        return false;

    return !IsOwnHome(path);
}

        private static bool IsSystem(string path)
{
    return path.Equals("/mnt/system", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/mnt/system/", StringComparison.OrdinalIgnoreCase);
}

private static bool IsEtc(string path)
{
    return path.Equals("/mnt/etc", StringComparison.OrdinalIgnoreCase)
        || path.StartsWith("/mnt/etc/", StringComparison.OrdinalIgnoreCase);
}

private static bool IsShadow(string path)
{
    return path.Equals("/mnt/etc/shadow", StringComparison.OrdinalIgnoreCase);
}

        private static string Normalize(string path)
{
    if (string.IsNullOrWhiteSpace(path))
        return "/";

    path = path.Replace('\\', '/').Trim();

    while (path.Contains("//"))
        path = path.Replace("//", "/");

    if (path.Length > 1)
        path = path.TrimEnd('/');

    return path;
}
    }
}