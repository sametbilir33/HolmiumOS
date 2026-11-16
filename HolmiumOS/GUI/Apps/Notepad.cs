
using System;
using HolmiumOS.GUI.Controls;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI.Apps
{
    public class Notepad : AppBase
    {
        private RichTextBox? txtEditor;

        private Button? btnNew;
        private Button? btnOpen;
        private Button? btnSave;
        private Button? btnSaveAs;

        private Label? lblStatus;

        private string? currentFile;
        private string? initialFilePath;

        public Notepad() : base("Not Defteri")
        {
        }

        public Notepad(string? filePath = null) : base("Not Defteri")
        {
            initialFilePath = filePath;
        }

        public override void Load()
        {
            if (Window == null)
                return;

            Window.Title = "Not Defteri";

            btnNew = new Button("Yeni", 10, 10, 75, 45);
            btnNew.ClickAction = NewClick;

            btnOpen = new Button("Ac", 95, 10, 75, 45);
            btnOpen.ClickAction = OpenClick;

            btnSave = new Button("Kaydet", 180, 10, 95, 45);
            btnSave.ClickAction = SaveClick;

            btnSaveAs = new Button("Farkli Kaydet", 285, 10, 145, 45);
            btnSaveAs.ClickAction = SaveAsClick;

            txtEditor = new RichTextBox(10, 65, 560, 335);

            lblStatus = new Label("Hazir", 10, 415);

            Window.AddControl(btnNew);
            Window.AddControl(btnOpen);
            Window.AddControl(btnSave);
            Window.AddControl(btnSaveAs);
            Window.AddControl(txtEditor);
            Window.AddControl(lblStatus);

            if (!string.IsNullOrEmpty(initialFilePath))
                OpenFileDirectly(initialFilePath);
        }

        private void NewClick()
        {
            if (txtEditor == null)
                return;

            txtEditor.Text = "";
            currentFile = null;
            SetStatus("Yeni belge olusturuldu.");
        }

        private void OpenClick()
        {
            string defaultPath = NormalizePath(UserManager.IsLoggedIn ? $"{UserManager.HomeDirectory}/" : "/home/");

            MessageBox msg = new MessageBox("Dosya Ac", "Acilacak dosya yolunu girin:", true, defaultPath, OnOpenConfirm, OnCancel);
            AppManager.Run(msg);
        }

        private void OnOpenConfirm(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || txtEditor == null)
                return;

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
                SetStatus("Dosya acilirken hata olustu!");
            }
        }

        private void SaveClick()
        {
            if (txtEditor == null)
                return;

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
            string defaultPath = NormalizePath(UserManager.IsLoggedIn ? $"{UserManager.HomeDirectory}/not.txt" : "/home/not.txt");

            MessageBox msg = new MessageBox("Farkli Kaydet", "Dosya yolunu girin:", true, defaultPath, OnSaveAsConfirm, OnCancel);
            AppManager.Run(msg);
        }

        private void OnSaveAsConfirm(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || txtEditor == null)
                return;

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
                currentFile = null;
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
                lblStatus.Text = msg;
        }

        public void OpenFileDirectly(string path)
        {
            try
            {
                string resolvedPath = NormalizePath(FileSystemManager.ResolvePath(path));

                if (!FileSystemManager.FileExists(resolvedPath))
                {
                    SetStatus("Dosya bulunamadi: " + path);
                    return;
                }

                string content = FileSystemManager.ReadFile(resolvedPath);

                if (txtEditor != null)
                    txtEditor.Text = content ?? "";

                currentFile = resolvedPath;
                SetStatus("Acildi: " + ExtractName(currentFile));
            }
            catch
            {
                SetStatus("Dosya acilirken hata olustu!");
            }
        }

        private string ExtractName(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return "";

            string path = NormalizePath(fullPath).TrimEnd('/');
            int index = path.LastIndexOf('/');

            return index >= 0 && index < path.Length - 1 ? path.Substring(index + 1) : path;
        }

        private string NormalizePath(string? path)
        {
            return string.IsNullOrEmpty(path) ? "" : path.Replace('\\', '/');
        }

        public override void Close()
        {
        }
    }
}