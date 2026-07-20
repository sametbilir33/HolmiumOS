using System;

namespace HolmiumOS.Commands.System
{
    public class Gui : ICommand
    {
        public string Name => "gui";
        public string Description => "HolmiumOS Grafik Arayuzunu (GUI) baslatir.";
        public string Usage => "gui";

        public void Execute(string args)
        {
            Console.WriteLine("Grafik motoru yukleniyor, lutfen bekleyin...");

            try
            {
                GUI.Init.Start();

                Console.ResetColor();
                Console.Clear();
                Console.WriteLine("GUI modundan cikildi. CLI aktif.");
            }
            catch (Exception ex)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[HATA] GUI baslatilamadi!");
                Console.WriteLine($"Hata Tipi: {ex.GetType().Name}");
                Console.WriteLine($"Mesaj: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
