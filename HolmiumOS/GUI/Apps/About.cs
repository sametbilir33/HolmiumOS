using System;
using HolmiumOS.GUI.Controls;
using Cosmos.Core;
using Cosmos.Core.Multiboot;
using Sys = Cosmos.System;
using System.Drawing;

namespace HolmiumOS.GUI.Apps
{
    public class About : AppBase
    {
        public About() : base("Hakkinda")
        {
        }

        public override void Load()
        {
            if (Window != null)
            {
                Window.Title = "Sistem Hakkinda";
            }

            Label headerLabel = new Label("HolmiumOS GUI Ortami", 20, 15);

            string cpuName = "x86/x64 Islemci";
            string vmType = "Bilinmiyor";
            string ramInfo = "RAM: Bilgi Alinamadi";
            string storageInfo = "Depolama: Bilgi Alinamadi";
            int storagePercent = 0;

            try
            {
                string brand = CPU.GetCPUBrandString();
                if (!string.IsNullOrEmpty(brand)) cpuName = brand;

                vmType = Sys.VMTools.IsVMWare ? "VMWare" :
                         Sys.VMTools.IsVirtualBox ? "VirtualBox" :
                         Sys.VMTools.IsQEMU ? "QEMU" : "Fiziksel";

                ulong totalRamMB = Multiboot2.GetMemUpper() / 1024;
                ramInfo = $"RAM: {totalRamMB} MB Toplam";

                var fs = Kernel.fs;
                string driveId = "0:";
                ulong totalBytes = (ulong)fs.GetTotalSize(driveId);
                ulong freeBytes = (ulong)fs.GetAvailableFreeSpace(driveId);
                ulong usedBytes = totalBytes - freeBytes;

                double totalGB = totalBytes / 1024.0 / 1024.0 / 1024.0;

                if (totalBytes > 0)
                {
                    storagePercent = (int)((usedBytes * 100) / totalBytes);
                }

                if (usedBytes >= 1024L * 1024L * 1024L)
                {
                    double usedGB = usedBytes / 1024.0 / 1024.0 / 1024.0;
                    storageInfo = $"Depolama: {usedGB:F2} GB / {totalGB:F2} GB";
                }
                else
                {
                    double usedMB = usedBytes / 1024.0 / 1024.0;
                    storageInfo = $"Depolama: {usedMB:F2} MB / {totalGB:F2} GB";
                }
            }
            catch
            {
            }

            Label cpuInfoLabel = new Label("CPU: " + cpuName, 20, 35);
            Label vmInfoLabel = new Label("Ortam: " + vmType, 20, 55);
            Label ramTextLabel = new Label(ramInfo, 20, 75);
            Label storageTextLabel = new Label(storageInfo, 20, 95);

            ProgressBar storageBar = new ProgressBar(20, 120, 240, 15);
            storageBar.Value = storagePercent;
            storageBar.BarColor = Color.DodgerBlue;
            storageBar.BackgroundColor = Color.LightGray;

            if (Window != null)
            {
                Window.AddControl(headerLabel);
                Window.AddControl(cpuInfoLabel);
                Window.AddControl(vmInfoLabel);
                Window.AddControl(ramTextLabel);
                Window.AddControl(storageTextLabel);
                Window.AddControl(storageBar);
            }
        }

        public override void Close()
        {
        }
    }
}