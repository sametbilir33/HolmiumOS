using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Input;

namespace HolmiumOS.GUI
{
    public enum CursorType
    {
        Default
    }

    public static class CursorManager
    {
        private static Png? defaultCursor;

        public static CursorType Current { get; private set; } =
            CursorType.Default;

        public static void Initialize()
        {
            defaultCursor = LoadPng("/cdrom/Resources/Cursor.png");
            Current = CursorType.Default;
        }

        public static void Set(CursorType type)
        {
            Current = type;
        }

        public static void Reset()
        {
            Current = CursorType.Default;
        }

        public static void Draw(Canvas canvas)
        {
            if (defaultCursor is null || canvas is null)
                return;

            int x = Clamp(
                (int)MouseManager.X,
                0,
                canvas.Width - 1);

            int y = Clamp(
                (int)MouseManager.Y,
                0,
                canvas.Height - 1);

            canvas.DrawImage(defaultCursor, x, y);
        }

        public static int GetWidth()
        {
            return defaultCursor is null
                ? 0
                : defaultCursor.Width;
        }

        public static int GetHeight()
        {
            return defaultCursor is null
                ? 0
                : defaultCursor.Height;
        }

        private static Png? LoadPng(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return null;

                return new Png(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Cursor hatasi: " + ex.Message);

                return null;
            }
        }

        private static int Clamp(int value, int min, int max)
        {
            if (min > max)
                return min;

            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }
    }
}