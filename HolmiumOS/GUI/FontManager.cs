
using System.IO;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI
{
    public static class FontManager
    {
        private const string UiFontPath = "/cdrom/Resources/Fonts/IBMPlexSans-Regular.ttf";
        private const string MonoFontPath = "/cdrom/Resources/Fonts/IBMPlexMono-Regular.ttf";

        private static Font _uiFont = FontManager.UiFont;
        private static Font _monoFont = FontManager.UiFont;
        private static bool _initialized;

        public static Font UiFont => _uiFont;
        public static Font MonoFont => _monoFont;

        public static int UiFontSize { get; set; } = 20;
        public static int MonoFontSize { get; set; } = 18;

        public static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;

            if (File.Exists(UiFontPath))
            {
                try
                {
                    _uiFont = new TrueTypeFont(UiFontPath);
                }
                catch
                {
                    _uiFont = FontManager.UiFont;
                }
            }

            if (File.Exists(MonoFontPath))
            {
                try
                {
                    _monoFont = new TrueTypeFont(MonoFontPath);
                }
                catch
                {
                    _monoFont = FontManager.UiFont;
                }
            }
        }
    }
}
