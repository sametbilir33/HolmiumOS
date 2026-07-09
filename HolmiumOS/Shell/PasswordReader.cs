using System;

namespace HolmiumOS.Shell
{
    public static class PasswordReader
    {
        public static string ReadPassword()
        {
            string password = "";

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        return password;

                    case ConsoleKey.Backspace:
                        if (password.Length > 0)
                        {
                            password = password.Substring(0, password.Length - 1);

                            if (Console.CursorLeft > 0)
                            {
                                Console.CursorLeft--;
                                Console.Write(' ');
                                Console.CursorLeft--;
                            }
                        }
                        continue;

                    case ConsoleKey.Delete:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.Home:
                    case ConsoleKey.End:
                        continue;
                }

                if (key.KeyChar >= ' ')
                {
                    password += key.KeyChar;
                    Console.Write('*');
                }
            }
        }
    }
}