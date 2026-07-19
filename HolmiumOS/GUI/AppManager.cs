using System.Collections.Generic;

namespace HolmiumOS.GUI
{
    public static class AppManager
    {
        private static List<AppBase> apps = new List<AppBase>();

        public static void Run<T>() where T : AppBase, new()
        {
            T app = new T();

            // Tip kontrolünü garantiye almak için isim üzerinden veya döngü koruması yapıyoruz
            for (int i = 0; i < apps.Count; i++)
            {
                if (apps[i] != null && apps[i].Name == app.Name)
                {
                    // Uygulama zaten açık! Yenisini açma, çökme yaratma.
                    return;
                }
            }

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