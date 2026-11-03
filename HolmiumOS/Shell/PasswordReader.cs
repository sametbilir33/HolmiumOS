using System;
using Cosmos.Kernel.System.Input;

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

                Key key = keyEvent.Key;

                switch (key)
                {
                    case Key.Enter:
                        Console.WriteLine();
                        return password;

                    case Key.Backspace:
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

                    case Key.Delete:
                    case Key.LeftArrow:
                    case Key.RightArrow:
                    case Key.UpArrow:
                    case Key.DownArrow:
                    case Key.Home:
                    case Key.End:
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