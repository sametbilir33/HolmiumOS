using System;
using System.Threading;
using HolmiumOS.Commands;
using Mat = System.Math;

namespace HolmiumOS.Commands.Fun
{
    public class Sl : ICommand
    {
        public string Name => "sl";
        public string Description => "Buharli lokomotif animasyonu";
        public string Usage => "sl";

        private Random random = new Random();

        public void Execute(string args)
        {
            Console.Clear();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            // Trenin genişliği yaklaşık 20 karakter, tamamen dışarıdan başlaması için -20
            int x = -20;
            int y = height - 10;
            int frame = 0;

            // Sadece lokomotif kalacak şekilde sadeleştirilmiş tren tasarımı
            string[] train =
            {
                "      ====      ",
                "  ====  ||      ",
                " ______||_____ ",
                "| _  _        |",
                "|_| |_|_______|",
                "   []     []   "
            };

            while (x < width)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                        break;
                }

                // CosmosOS için her karede tüm ekranı temizlemek yerine sadece gerekli yerleri çiziyoruz
                Console.Clear();

                DrawRails(width, frame);
                DrawSmoke(x + 8, y - 1);
                DrawTrain(x, y, train, frame);

                x++;
                frame++;
                Thread.Sleep(100);
            }

            Console.ResetColor();
            Console.Clear();
        }

        private void DrawTrain(int x, int y, string[] train, int frame)
        {
            // Tren tamamen beyaz olacak
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < train.Length; i++)
            {
                Draw(x, y + i, train[i]);
            }

            // Tekerlek animasyonu (Beyaz renk)
            string wheel = (frame % 2 == 0) ? "O       O" : "o       o";
            Draw(x + 3, y + 5, wheel);
        }

        private void DrawSmoke(int x, int y)
        {
            // Dumanlar beyaz ve lokomotif bacasına göre hizalı
            Console.ForegroundColor = ConsoleColor.White;

            int offset = random.Next(0, 2);

            Draw(x, y - offset, "o");
            Draw(x + 1, y - offset - 1, "O");
            Draw(x - 1, y - offset - 2, "o");
        }

        private void DrawRails(int width, int frame)
        {
            // Yol kendi orijinal renginde (DarkGray) kalıyor
            Console.ForegroundColor = ConsoleColor.DarkGray;

            string rail = "";
            for (int i = 0; i < width / 2; i++)
            {
                if ((i + frame) % 2 == 0)
                    rail += "==";
                else
                    rail += "--";
            }

            if (Console.WindowHeight - 2 >= 0)
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 2);
                Console.Write(rail);
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Console.Write(rail);
            }
        }

        private void Draw(int x, int y, string text)
        {
            // Y ekseni sınır kontrolü
            if (y < 0 || y >= Console.WindowHeight)
                return;

            // Tren soldan girerken x negatif olacaktır. 
            // Eğer metnin tamamı ekranın solunun dışındaysa hiç çizme.
            if (x + text.Length <= 0 || x >= Console.WindowWidth)
                return;

            // Eğer metnin bir kısmı ekranın solunda kalıyorsa, sadece ekrana sığan kısmını kesip çiziyoruz.
            if (x < 0)
            {
                int substringIndex = Mat.Abs(x);
                if (substringIndex < text.Length)
                {
                    text = text.Substring(substringIndex);
                    x = 0;
                }
                else
                {
                    return;
                }
            }

            // Ekranın sağına taşan kısmı kesme kontrolü
            if (x + text.Length > Console.WindowWidth)
            {
                text = text.Substring(0, Console.WindowWidth - x);
            }

            Console.SetCursorPosition(x, y);
            Console.Write(text);
        }
    }
}