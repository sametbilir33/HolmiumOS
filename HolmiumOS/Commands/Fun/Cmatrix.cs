using System;
using System.Threading;

namespace HolmiumOS.Commands.Fun
{
    public class Cmatrix : ICommand
    {
        public string Name => "cmatrix";
        public string Description => "Matrix yagmur efekti";
        public string Usage => "cmatrix";

        private Random random = new Random();

        private const string chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#$%&*+-<>[]{}";

        public void Execute(string args)
        {
            Console.Clear();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            int[] drops = new int[width];
            int[] lengths = new int[width];

            for (int i = 0; i < width; i++)
            {
                drops[i] = random.Next(-height, 0);
                lengths[i] = random.Next(3, 15);
            }

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Escape)
                        break;
                }


                for (int x = 0; x < width; x++)
                {
                    int y = drops[x];


                    if (y >= 0 && y < height)
                    {
                        Draw(
                            x,
                            y,
                            ConsoleColor.White
                        );
                    }


                    for (int i = 1; i < lengths[x]; i++)
                    {
                        int trail = y - i;

                        if (trail >= 0 && trail < height)
                        {
                            Draw(
                                x,
                                trail,
                                ConsoleColor.DarkGreen
                            );
                        }
                    }


                    int clear = y - lengths[x];

                    if (clear >= 0 && clear < height)
                    {
                        Console.SetCursorPosition(
                            x,
                            clear
                        );

                        Console.Write(' ');
                    }


                    drops[x]++;


                    if (drops[x] - lengths[x] > height)
                    {
                        drops[x] = random.Next(-height, 0);
                        lengths[x] = random.Next(3, 15);
                    }
                }


                Thread.Sleep(60);
            }

            Console.ResetColor();
            Console.Clear();
        }


        private void Draw(
            int x,
            int y,
            ConsoleColor color
        )
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;

            Console.Write(
                chars[random.Next(chars.Length)]
            );
        }
    }
}