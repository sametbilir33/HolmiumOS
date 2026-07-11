using System.Collections.Generic;

namespace HolmiumOS.GUI
{
    public static class AppManager
    {
        private static List<AppBase> apps =
            new List<AppBase>();


        public static void Run<T>()
            where T : AppBase, new()
        {
            T app = new T();

            apps.Add(app);

            app.Open();
        }


        public static void Close(AppBase app)
        {
            app.Close();

            apps.Remove(app);
        }
    }
}