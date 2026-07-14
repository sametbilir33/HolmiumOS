using System;
using System.Threading;
using HolmiumOS.Commands;

namespace HolmiumOS.Commands.Fun
{
    public class Starfield : ICommand
    {
        public string Name => "starfield";
        public string Description => "Starfield animasyonu";
        public string Usage => "starfield";

        private Random random = new Random();

        private class Star
        {
            public int X;
            public int Y;
            public int Z;
        }

        public void Execute(string args)
        {
            Console.Clear();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            Star[] stars = new Star[80];

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = CreateStar(width, height);
            }


            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Escape)
                        break;
                }


                Console.Clear();

                foreach (var star in stars)
                {
                    star.Z--;

                    if (star.Z <= 0)
                    {
                        ResetStar(
                            star,
                            width,
                            height
                        );
                    }


                    int x = width / 2 + (star.X * 20) / star.Z;
                    int y = height / 2 + (star.Y * 10) / star.Z;


                    if (x >= 0 &&
                        x < width &&
                        y >= 0 &&
                        y < height)
                    {
                        Console.SetCursorPosition(x, y);

                        if (star.Z < 5)
                            Console.ForegroundColor = ConsoleColor.White;
                        else
                            Console.ForegroundColor = ConsoleColor.Gray;

                        Console.Write('*');
                    }
                }


                Thread.Sleep(80);
            }


            Console.ResetColor();
            Console.Clear();
        }


        private Star CreateStar(int width, int height)
        {
            return new Star
            {
                X = random.Next(-width, width),
                Y = random.Next(-height, height),
                Z = random.Next(1, 20)
            };
        }


        private void ResetStar(
            Star star,
            int width,
            int height
        )
        {
            star.X = random.Next(-width, width);
            star.Y = random.Next(-height, height);
            star.Z = 20;
        }
    }
}