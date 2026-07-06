using HolmiumOS.Shell;

namespace HolmiumOS.Commands.FileSystem
{
    public class Miv : ICommand
    {
        public string Name => "miv";
        public string Description => "Metin Editoru";
        public string Usage => "miv";

        public void Execute(string args)
        {
            MIV.StartMIV();
        }
    }
}