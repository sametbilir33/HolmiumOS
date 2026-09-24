using System;
using Cosmos.Kernel.System.Keyboard;

namespace HolmiumOS.Shell
{
    public static class PasswordReader
    {
        public static string ReadPassword()
        {
            string password = "";

            while (true)
            {
                if (!KeyboardManager.TryReadKey(out KeyEvent keyEvent))
                {
                    continue;
                }

                ConsoleKeyEx key = keyEvent.Key;

                switch (key)
                {
                    case ConsoleKeyEx.Enter:
                        Console.WriteLine();
                        return password;

                    case ConsoleKeyEx.Backspace:
                        if (password.Length > 0)
                        {
                            password = password.Substring(
                                0,
                                password.Length - 1);

                            if (Console.CursorLeft > 0)
                            {
                                Console.CursorLeft--;

                                Console.Write(' ');

                                Console.CursorLeft--;
                            }
                        }

                        continue;

                    case ConsoleKeyEx.Delete:
                    case ConsoleKeyEx.LeftArrow:
                    case ConsoleKeyEx.RightArrow:
                    case ConsoleKeyEx.UpArrow:
                    case ConsoleKeyEx.DownArrow:
                    case ConsoleKeyEx.Home:
                    case ConsoleKeyEx.End:
                        continue;
                }

                char character = keyEvent.KeyChar;

                if (character >= ' ')
                {
                    password += character;
                    Console.Write('*');
                }
            }
        }
    }
}