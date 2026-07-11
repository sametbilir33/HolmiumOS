using HolmiumOS.GUI;

namespace HolmiumOS.GUI
{
    public abstract class AppBase
    {
        public string Name;

        public Window Window;


        public AppBase(string name)
        {
            Name = name;
        }


        public void Open()
        {
            Window = new Window(
                this,
                Name,
                100,
                100,
                300,
                200
            );


            Load();


            WindowManager.Add(Window);
        }


        public abstract void Load();


        public virtual void Close()
        {
            WindowManager.Remove(Window);
        }
    }
}