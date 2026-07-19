using System.Collections.Generic;

namespace HolmiumOS.GUI
{
    public static class AppManager
    {
        private static List<AppBase> apps = new List<AppBase>();

        public static void Run<T>() where T : AppBase, new()
        {
            for (int i = 0; i < apps.Count; i++)
            {
                if (apps[i] is T)
                {
                    return;
                }
            }

            T app = new T();
            apps.Add(app);
            app.Open();
        }

        public static void Close(AppBase app)
        {
            if (app == null) return;

            if (apps.Contains(app))
            {
                apps.Remove(app);
            }

            app.Close();
        }
    }
}