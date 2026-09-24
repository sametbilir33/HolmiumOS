namespace HolmiumOS.Shell
{
    public class ShellContext
    {
        public string CurrentDirectory { get; set; }

        public ShellContext(string currentDirectory)
        {
            CurrentDirectory = currentDirectory;
        }
    }
}