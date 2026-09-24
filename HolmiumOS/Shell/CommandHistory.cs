using System.Collections.Generic;

namespace HolmiumOS.Shell
{
    public static class CommandHistory
    {
        private static List<string> history = new List<string>();
        private static int cursor = 0;

        public static void Add(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return;

            if (history.Count > 0 && history[history.Count - 1] == command)
            {
                cursor = history.Count;
                return;
            }

            history.Add(command);
            cursor = history.Count;
        }

        public static string Previous()
        {
            if (history.Count == 0)
                return null;

            if (cursor > 0)
                cursor--;

            return history[cursor];
        }

        public static string Next()
        {
            if (history.Count == 0)
                return null;

            if (cursor < history.Count - 1)
            {
                cursor++;
                return history[cursor];
            }

            cursor = history.Count;
            return string.Empty;
        }

        public static void Clear()
        {
            history.Clear();
            cursor = 0;
        }
    }
}