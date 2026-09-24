namespace HolmiumOS.Commands
{
    public interface ICommand
    {
        string Name { get; }         // Komutun adi (orn: "dir")
        string Description { get; }  // Komutun aciklamasi
        string Usage { get; }        // Kullanim bilgisi
        void Execute(string args);    // Komut calistirildiginda cagrilan metot
    }
}