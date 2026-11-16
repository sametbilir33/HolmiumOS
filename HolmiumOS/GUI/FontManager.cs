
using System.IO;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI
{
    public static class FontManager
    {
        private const string UiFontPath = "/cdrom/Resources/Fonts/IBMPlexSans-Regular.ttf";
        private const string MonoFontPath = "/cdrom/Resources/Fonts/IBMPlexMono-Regular.ttf";

        private static Font _uiFont = PCScreenFont.DefaultFont;
        private static Font _monoFont = PCScreenFont.DefaultFont;
        private static bool _initialized;

        public static Font UiFont => _uiFont;
        public static Font MonoFont => _monoFont;

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
                    _uiFont = PCScreenFont.DefaultFont;
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
                    _monoFont = PCScreenFont.DefaultFont;
                }
            }
        }
    }
}
