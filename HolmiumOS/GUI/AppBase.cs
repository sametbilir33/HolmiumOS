namespace HolmiumOS.GUI
{
    public abstract class AppBase
    {
        public string Name { get; set; }
        public Window Window { get; set; }

        public AppBase(string name)
        {
            this.Name = name;
        }

        public void Open(int defaultX = 100, int defaultY = 100)
        {
            this.Window = new Window(this, this.Name, defaultX, defaultY);

            WindowManager.Add(this.Window);
            Load();
        }

        public abstract void Load();

        public virtual void Close()
        { }
    }
}