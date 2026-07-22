using System.Collections.Generic;

namespace HolmiumOS.GUI
{
    public static class AppManager
    {
        public static List<AppBase> apps = new List<AppBase>();

        public static void Run<T>(int x = 100, int y = 100) where T : AppBase, new()
        {
            int instanceCount = 0;
            for (int i = 0; i < apps.Count; i++)
            {
                if (apps[i] != null && apps[i] is T)
                {
                    instanceCount++;
                }
            }

            int spawnX = x + (instanceCount * 30);
            int spawnY = y + (instanceCount * 30);

            T app = new T();
            apps.Add(app);

            app.Open(spawnX, spawnY);
        }

        public static void Run(AppBase app, int x = 150, int y = 150)
        {
            if (app == null) return;

            int spawnX = x + (apps.Count * 20);
            int spawnY = y + (apps.Count * 20);

            apps.Add(app);
            app.Open(spawnX, spawnY);
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