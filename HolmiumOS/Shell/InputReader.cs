using System;
using System.Text;

namespace HolmiumOS.Shell
{
    public static class InputReader
    {
        public static string ReadLineWithHistory(Action redrawPrompt)
        {
            StringBuilder buffer = new StringBuilder();
            int lastVisibleLength = 0;

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return buffer.ToString();
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Remove(buffer.Length - 1, 1);
                        lastVisibleLength = RedrawLine(buffer.ToString(), redrawPrompt, lastVisibleLength);
                    }
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    string prev = CommandHistory.Previous();
                    if (prev != null)
                    {
                        buffer.Clear();
                        buffer.Append(prev);
                        lastVisibleLength = RedrawLine(buffer.ToString(), redrawPrompt, lastVisibleLength);
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    string next = CommandHistory.Next();
                    if (next != null)
                    {
                        buffer.Clear();
                        buffer.Append(next);
                        lastVisibleLength = RedrawLine(buffer.ToString(), redrawPrompt, lastVisibleLength);
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    buffer.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                    lastVisibleLength = buffer.Length;
                }
            }
        }

        private static int RedrawLine(string text, Action redrawPrompt, int previousLength)
        {
            Console.Write("\r");
            redrawPrompt();
            Console.Write(text);

            int diff = previousLength - text.Length;

            if (diff > 0)
            {
                Console.Write(new string(' ', diff));
                Console.Write("\r");
                redrawPrompt();
                Console.Write(text);
            }

            return text.Length;
        }
    }
}