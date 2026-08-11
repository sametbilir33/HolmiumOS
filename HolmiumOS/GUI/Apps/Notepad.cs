using System;
using HolmiumOS.GUI.Controls;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI.Apps
{
    public class Notepad : AppBase
    {
        private RichTextBox txtEditor;

        private Button btnNew;
        private Button btnOpen;
        private Button btnSave;
        private Button btnSaveAs;

        private Label lblStatus;

        private string currentFile = null;
        private string initialFilePath = null;

        // Normal açılış için boş constructor
        public Notepad() : base("Not Defteri")
        {
        }

        // Masaüstünden veya dışarıdan dosya yoluyla açılış için constructor
        public Notepad(string filePath) : base("Not Defteri")
        {
            initialFilePath = filePath;
        }

        public override void Load()
        {
            if (Window != null)
            {
                Window.Title = "Not Defteri";
            }

            txtEditor = new RichTextBox(10, 10, 370, 230);

            btnNew = new Button("Yeni", 10, 250, 60, 25);
            btnNew.ClickAction = NewClick;

            btnOpen = new Button("Ac", 80, 250, 60, 25);
            btnOpen.ClickAction = OpenClick;

            btnSave = new Button("Kaydet", 150, 250, 70, 25);
            btnSave.ClickAction = SaveClick;

            btnSaveAs = new Button("Farkli Kaydet", 230, 250, 110, 25);
            btnSaveAs.ClickAction = SaveAsClick;

            lblStatus = new Label("Hazir", 10, 285);

            if (Window != null)
            {
                Window.AddControl(txtEditor);
                Window.AddControl(btnNew);
                Window.AddControl(btnOpen);
                Window.AddControl(btnSave);
                Window.AddControl(btnSaveAs);
                Window.AddControl(lblStatus);
            }

            // Arayüz yüklendiği an eğer dışarıdan dosya yolu verildiyse içeriği yükle
            if (!string.IsNullOrEmpty(initialFilePath))
            {
                OpenFileDirectly(initialFilePath);
            }
        }

        private void NewClick()
        {
            if (txtEditor == null) return;

            txtEditor.Text = "";
            currentFile = null;
            SetStatus("Yeni belge olusturuldu.");
        }

        private void OpenClick()
        {
            string defaultPath = NormalizePath(
                UserManager.IsLoggedIn ? $"{UserManager.HomeDirectory}/" : "0:/home/"
            );

            var msg = new MessageBox(
                "Dosya Ac",
                "Acilacak dosya yolunu girin:",
                true,
                defaultPath,
                OnOpenConfirm,
                OnCancel
            );

            AppManager.Run(msg);
        }

        private void OnOpenConfirm(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            path = NormalizePath(path.Trim());

            try
            {
                string resolvedPath = FileSystemManager.ResolvePath(path);

                if (!FileSystemManager.FileExists(resolvedPath))
                {
                    SetStatus("Dosya bulunamadi!");
                    return;
                }

                txtEditor.Text = FileSystemManager.ReadFile(resolvedPath);
                currentFile = resolvedPath;
                SetStatus("Acildi: " + ExtractName(currentFile));
            }
            catch
            {
                SetStatus("Dosya acilırken hata olustu!");
            }
        }

        private void SaveClick()
        {
            if (string.IsNullOrEmpty(currentFile))
            {
                SaveAsClick();
                return;
            }

            try
            {
                currentFile = NormalizePath(FileSystemManager.ResolvePath(currentFile));

                FileSystemManager.WriteFile(currentFile, txtEditor.Text);

                DesktopManager.RefreshIcons();

                SetStatus("Kaydedildi.");
            }
            catch
            {
                SetStatus("Kaydetme hatasi!");
            }
        }

        private void SaveAsClick()
        {
            string defaultPath = NormalizePath(
                UserManager.IsLoggedIn ? $"{UserManager.HomeDirectory}/not.txt" : "0:/home/not.txt"
            );

            var msg = new MessageBox(
                "Farkli Kaydet",
                "Dosya yolunu girin:",
                true,
                defaultPath,
                OnSaveAsConfirm,
                OnCancel
            );

            AppManager.Run(msg);
        }

        private void OnSaveAsConfirm(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            path = NormalizePath(path.Trim());

            try
            {
                currentFile = NormalizePath(FileSystemManager.ResolvePath(path));

                FileSystemManager.WriteFile(currentFile, txtEditor.Text);

                DesktopManager.RefreshIcons();

                SetStatus("Kaydedildi: " + ExtractName(currentFile));
            }
            catch
            {
                SetStatus("Kaydetme hatasi!");
            }
        }

        private void OnCancel()
        {
            SetStatus("Islem iptal edildi.");
        }

        private void SetStatus(string msg)
        {
            if (lblStatus != null)
            {
                lblStatus.Text = msg;
            }
        }

        public void OpenFileDirectly(string path)
        {
            try
            {
                string resolvedPath = NormalizePath(FileSystemManager.ResolvePath(path));

                if (FileSystemManager.FileExists(resolvedPath))
                {
                    string content = FileSystemManager.ReadFile(resolvedPath);
                    if (txtEditor != null)
                    {
                        txtEditor.Text = content ?? "";
                    }
                    currentFile = resolvedPath;
                    SetStatus("Acildi: " + ExtractName(currentFile));
                }
                else
                {
                    SetStatus("Dosya bulunamadi: " + path);
                }
            }
            catch
            {
                SetStatus("Dosya acilirken hata olustu!");
            }
        }

        private string ExtractName(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return "";
            string p = NormalizePath(fullPath).TrimEnd('/');
            int idx = p.LastIndexOf('/');

            return (idx >= 0 && idx < p.Length - 1) ? p.Substring(idx + 1) : p;
        }

        private string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            return path.Replace('\\', '/');
        }
    }
}