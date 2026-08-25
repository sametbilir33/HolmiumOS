using Cosmos.System;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;

namespace HolmiumOS.GUI
{
    public enum CursorType
    {
        Default
        // Disabled,
        // IBeam,
        // ResizeHorizontal,
        // ResizeVertical,
        // ResizeDiagonal,
        // ResizeDiagonalReverse
    }

    public static class CursorManager
    {
        [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.Cursor.bmp")]
        private static byte[] defaultCursorData;

        // [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.CursorDisabled.bmp")]
        // private static byte[] disabledCursorData;

        // [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.CursorIBeam.bmp")]
        // private static byte[] iBeamCursorData;

        // [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.CursorResizeHorizontal.bmp")]
        // private static byte[] resizeHorizontalCursorData;

        // [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.CursorResizeVertical.bmp")]
        // private static byte[] resizeVerticalCursorData;

        // [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.CursorResizeDiagonal.bmp")]
        // private static byte[] resizeDiagonalCursorData;

        // [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.CursorResizeDiagonalReverse.bmp")]
        // private static byte[] resizeDiagonalReverseCursorData;

        private static Bitmap defaultCursor;

        // private static Bitmap disabledCursor;
        // private static Bitmap iBeamCursor;
        // private static Bitmap resizeHorizontalCursor;
        // private static Bitmap resizeVerticalCursor;
        // private static Bitmap resizeDiagonalCursor;
        // private static Bitmap resizeDiagonalReverseCursor;

        public static CursorType Current { get; private set; } = CursorType.Default;

        public static void Initialize()
        {
            defaultCursor = new Bitmap(defaultCursorData);

            // disabledCursor = new Bitmap(disabledCursorData);
            // iBeamCursor = new Bitmap(iBeamCursorData);
            // resizeHorizontalCursor = new Bitmap(resizeHorizontalCursorData);
            // resizeVerticalCursor = new Bitmap(resizeVerticalCursorData);
            // resizeDiagonalCursor = new Bitmap(resizeDiagonalCursorData);
            // resizeDiagonalReverseCursor = new Bitmap(resizeDiagonalReverseCursorData);

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
            Bitmap cursor = GetCurrentCursor();

            int x = Clamp(
                (int)MouseManager.X,
                0,
                (int)canvas.Mode.Width - (int)cursor.Width
            );

            int y = Clamp(
                (int)MouseManager.Y,
                0,
                (int)canvas.Mode.Height - (int)cursor.Height
            );

            canvas.DrawImageAlpha(cursor, x, y);
        }

        public static int GetWidth()
        {
            return (int)GetCurrentCursor().Width;
        }

        public static int GetHeight()
        {
            return (int)GetCurrentCursor().Height;
        }

        private static Bitmap GetCurrentCursor()
        {
            return defaultCursor;
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