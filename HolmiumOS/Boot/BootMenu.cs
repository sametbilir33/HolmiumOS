using System;
using Cosmos.System;

namespace HolmiumOS.Boot
{
    public enum BootMode
    {
        CLI,
        GUI
    }

    public static class BootMenu
    {
        public static BootMode Show()
        {
            int selected = 0;

            string[] items =
            {
                "CLI",
                "GUI",
                "Shutdown",
                "Reboot"
            };

            while (true)
            {
                System.Console.Clear();

                System.Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.WriteLine("==== HolmiumOS Boot Menu ====");
                System.Console.ResetColor();

                for (int i = 0; i < items.Length; i++)
                {
                    if (i == selected)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Black;
                        System.Console.BackgroundColor = ConsoleColor.White;
                    }

                    System.Console.WriteLine($"> {items[i]}");

                    System.Console.ResetColor();
                }


                var key = System.Console.ReadKey(true);

                if (key.Key == ConsoleKey.UpArrow)
                {
                    selected--;

                    if (selected < 0)
                        selected = items.Length - 1;
                }


                if (key.Key == ConsoleKey.DownArrow)
                {
                    selected++;

                    if (selected >= items.Length)
                        selected = 0;
                }


                if (key.Key == ConsoleKey.Enter)
                {
                    switch (selected)
                    {
                        case 0:
                            return BootMode.CLI;

                        case 1:
                            return BootMode.GUI;

                        case 2:
                            Power.Shutdown();

                            break;

                        case 3:
                            Power.Reboot();

                            break;
                    }
                }
            }
        }
    }
}