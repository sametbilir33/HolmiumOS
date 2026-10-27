using System;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Storage;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class About : AppBase
    {
        private const int LeftX = 20;
        private const int RightX = 500;
        private const int LineHeight = 32;

        private int leftY;
        private int rightY;

        public About() : base("Hakkinda")
        {
        }

        public override void Load()
        {
            if (Window != null)
                Window.Title = "HolmiumOS Hakkinda";

            leftY = 20;
            rightY = 20;

            AddSectionTitle("[ RUNTIME ]", LeftX, ref leftY);
            AddRuntimeInfo();

            AddSectionTitle("[ GRAPHICS ]", LeftX, ref leftY);
            AddGraphicsInfo();

            AddSectionTitle("[ STORAGE ]", RightX, ref rightY);
            AddStorageInfo();
        }

        private void AddRuntimeInfo()
        {
            try
            {
                AddLine(LeftX, ref leftY, ".NET Runtime: " + Environment.Version);
                AddLine(LeftX, ref leftY, "Process: " + (Environment.Is64BitProcess ? "64-bit" : "32-bit"));
            }
            catch
            {
                // Bilgi alınamıyorsa gösterme.
            }
        }

        private void AddGraphicsInfo()
        {
            try
            {
                Canvas canvas = Init.canvas;
                if (canvas == null)
                    return;

                AddLine(LeftX, ref leftY, "Resolution: " + (int)canvas.Mode.Width + " x " + (int)canvas.Mode.Height);
                AddLine(LeftX, ref leftY, "Color depth: " + (int)canvas.Mode.ColorDepth + " bit");
                AddLine(LeftX, ref leftY, "Refresh rate: " + canvas.RefreshRate + " Hz");

                if (!string.IsNullOrWhiteSpace(canvas.Name))
                    AddLine(LeftX, ref leftY, "Canvas: " + canvas.Name);

                if (canvas.AvailableModes != null)
                    AddLine(LeftX, ref leftY, "Available modes: " + canvas.AvailableModes.Count);
            }
            catch
            {
                // Bilgi alınamıyorsa gösterme.
            }
        }

        private void AddStorageInfo()
        {
            try
            {
                AddLine(RightX, ref rightY, "Devices: " + StorageManager.DeviceCount);
                AddLine(RightX, ref rightY, "Partitions: " + StorageManager.Partitions.Count);

                for (int i = 0; i < StorageManager.Devices.Count; i++)
                {
                    var device = StorageManager.Devices[i];
                    if (device == null)
                        continue;

                    string name = string.IsNullOrWhiteSpace(device.Name) ? "Device " + i : device.Name;
                    ulong totalBytes = (ulong)device.BlockCount * device.BlockSize;

                    AddLine(RightX, ref rightY, name + ": " + FormatBytes(totalBytes));
                    AddLine(RightX, ref rightY, "Blocks: " + device.BlockCount + " x " + device.BlockSize + " B");
                }
            }
            catch
            {
                // Bilgi alınamıyorsa gösterme.
            }
        }

        private void AddSectionTitle(string title, int x, ref int y)
        {
            Add(new Label(title, x, y));
            y += LineHeight;
        }

        private void AddLine(int x, ref int y, string text)
        {
            Add(new Label(text, x, y));
            y += LineHeight;
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