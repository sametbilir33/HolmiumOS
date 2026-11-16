
using System;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Storage;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class About : AppBase
    {
        private const int LeftX = 20;
        private const int LineHeight = 30;
        private const int SectionSpacing = 12;

        private int currentY;

        public About() : base("Hakkında")
        {
        }

        public override void Load()
        {
            if (Window != null)
                Window.Title = "HolmiumOS Hakkında";

            currentY = 20;

            AddSectionTitle("HolmiumOS", "Sistem bilgileri");
            AddLine("Runtime", Environment.Version.ToString());
            AddLine("Architecture", Environment.Is64BitProcess ? "64-bit" : "32-bit");

            currentY += SectionSpacing;

            AddSectionTitle("Graphics", "Görüntü sistemi");
            AddGraphicsInfo();

            currentY += SectionSpacing;

            AddSectionTitle("Storage", "Depolama aygıtları");
            AddStorageInfo();
        }

        private void AddSectionTitle(string title, string subtitle)
        {
            Add(new Label(title, LeftX, currentY));
            currentY += LineHeight;

            Add(new Label(subtitle, LeftX, currentY));
            currentY += LineHeight + 4;
        }

        private void AddLine(string name, string value)
        {
            Add(new Label(name + ": " + value, LeftX, currentY));
            currentY += LineHeight;
        }

        private void AddGraphicsInfo()
        {
            try
            {
                Canvas canvas = Init.canvas;

                if (canvas == null)
                    return;

                AddLine("Resolution", (int)canvas.Mode.Width + " x " + (int)canvas.Mode.Height);
                AddLine("Color depth", (int)canvas.Mode.ColorDepth + " bit");
                AddLine("Refresh rate", canvas.RefreshRate + " Hz");

                if (!string.IsNullOrWhiteSpace(canvas.Name))
                    AddLine("Canvas", canvas.Name);

                if (canvas.AvailableModes != null)
                    AddLine("Available modes", canvas.AvailableModes.Count.ToString());
            }
            catch
            {
                // Grafik bilgileri alınamazsa gösterme.
            }
        }

        private void AddStorageInfo()
        {
            try
            {
                AddLine("Devices", StorageManager.DeviceCount.ToString());
                AddLine("Partitions", StorageManager.Partitions.Count.ToString());

                for (int i = 0; i < StorageManager.Devices.Count; i++)
                {
                    var device = StorageManager.Devices[i];

                    if (device == null)
                        continue;

                    string name = string.IsNullOrWhiteSpace(device.Name) ? "Device " + i : device.Name;
                    ulong totalBytes = (ulong)device.BlockCount * device.BlockSize;

                    AddLine(name, FormatBytes(totalBytes));
                    AddLine("Block count", device.BlockCount + " x " + device.BlockSize + " B");
                }
            }
            catch
            {
                // Depolama bilgileri alınamazsa gösterme.
            }
        }

        private static string FormatBytes(ulong bytes)
        {
            const ulong KB = 1024UL;
            const ulong MB = KB * 1024UL;
            const ulong GB = MB * 1024UL;
            const ulong TB = GB * 1024UL;

            if (bytes >= TB)
                return (bytes / TB) + " TB";

            if (bytes >= GB)
                return (bytes / GB) + " GB";

            if (bytes >= MB)
                return (bytes / MB) + " MB";

            if (bytes >= KB)
                return (bytes / KB) + " KB";

            return bytes + " B";
        }

        private void Add(Control control)
        {
            if (Window != null)
                Window.AddControl(control);
        }

        public override void Close()
        {
        }
    }
}
