using System;
using System.IO;
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
        private static Bitmap? defaultCursor;

        public static CursorType Current { get; private set; } =
            CursorType.Default;

        public static void Initialize()
        {
            defaultCursor = LoadBitmap(
                "/cdrom/Resources/Cursor.bmp"
            );

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
            if (defaultCursor is null ||
                canvas == null)
            {
                return;
            }

            Bitmap cursor =
                GetCurrentCursor();

            int x = Clamp(
    (int)MouseManager.X,
    0,
    canvas.Width - 1);

int y = Clamp(
    (int)MouseManager.Y,
    0,
    canvas.Height - 1);

            canvas.DrawImage(
                cursor,
                x,
                y);
        }

        public static int GetWidth()
        {
            Bitmap? cursor =
                defaultCursor;

            return cursor is null
                ? 0
                : (int)cursor.Width;
        }

        public static int GetHeight()
        {
            Bitmap? cursor =
                defaultCursor;

            return cursor is null
                ? 0
                : (int)cursor.Height;
        }

        private static Bitmap GetCurrentCursor()
        {
            return defaultCursor
                ?? throw new InvalidOperationException(
                    "Cursor bitmap yüklenemedi.");
        }

        private static Bitmap? LoadBitmap(
            params string[] paths)
        {
            foreach (string path in paths)
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        continue;
                    }

                    return new Bitmap(path);
                }
                catch
                {
                    // Bir sonraki olası yolu dene.
                }
            }

            return null;
        }

        private static int Clamp(
            int value,
            int min,
            int max)
        {
            if (min > max)
            {
                return min;
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }
    }
}