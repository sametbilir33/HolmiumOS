using System;
using HolmiumOS.GUI.Controls;
using Cosmos.Core;
using Sys = Cosmos.System;
using Cosmos.System.FileSystem;

namespace HolmiumOS.GUI.Apps
{
    public class About : AppBase
    {
        private Label osInfoLabel;
        private Label cpuInfoLabel;
        private Label vmInfoLabel;
        private Label ramTextLabel;

        private Label diskTextLabel;
        private ProgressBar diskProgressBar;

        private Button refreshButton;

        public About() : base("HolmiumOS - Hakkinda")
        {
        }

        public override void Load()
        {
            if (this.Window != null)
            {
                this.Window.Title = "Sistem Hakkinda";
            }

            Label headerLabel = new Label("HolmiumOS GUI Ortami", 20, 15);

            string osVer = "Surum: " + Kernel.OSVERSION;
            osInfoLabel = new Label(osVer, 20, 35);

            string cpuName = "x86/x64 Islemci";
            try
            {
                cpuName = CPU.GetCPUBrandString();
                if (string.IsNullOrEmpty(cpuName)) cpuName = "x86/x64 Islemci";
            }
            catch { }
            cpuInfoLabel = new Label("CPU: " + cpuName, 20, 60);

            string vmType = "Bilinmiyor";
            try
            {
                vmType = Sys.VMTools.IsVMWare ? "VMWare" :
                         Sys.VMTools.IsVirtualBox ? "VirtualBox" :
                         Sys.VMTools.IsQEMU ? "QEMU" : "Fiziksel";
            }
            catch { }
            vmInfoLabel = new Label("Ortam: " + vmType, 20, 80);

            string ramInfo = "RAM: Bilgi Alinamadi";
            try
            {
                uint totalRamMB = CPU.GetAmountOfRAM();
                ramInfo = "Toplam RAM: " + totalRamMB.ToString() + " MB";
            }
            catch { }
            ramTextLabel = new Label(ramInfo, 20, 110);

            diskTextLabel = new Label("Disk: Bilgiler Yukleniyor...                    ", 20, 140);
            diskProgressBar = new ProgressBar(20, 160, 260, 20);

            refreshButton = new Button("Bilgileri Guncelle", 20, 195, 260, 30);
            refreshButton.ClickAction = OnRefreshButtonClick;

            if (this.Window != null)
            {
                this.Window.AddControl(headerLabel);
                this.Window.AddControl(osInfoLabel);
                this.Window.AddControl(cpuInfoLabel);
                this.Window.AddControl(vmInfoLabel);
                this.Window.AddControl(ramTextLabel);

                this.Window.AddControl(diskTextLabel);
                this.Window.AddControl(diskProgressBar);

                this.Window.AddControl(refreshButton);
            }

            UpdateDiskStats();
        }

        private void OnRefreshButtonClick()
        {
            UpdateDiskStats();
        }

        private void UpdateDiskStats()
        {
            try
            {
                if (Kernel.fs != null)
                {
                    string driveId = @"0:";

                    ulong totalBytes = (ulong)Kernel.fs.GetTotalSize(driveId);
                    ulong freeBytes = (ulong)Kernel.fs.GetAvailableFreeSpace(driveId);
                    ulong usedBytes = totalBytes > freeBytes ? totalBytes - freeBytes : 0;

                    double totalMB = totalBytes / 1024.0 / 1024.0;
                    double usedMB = usedBytes / 1024.0 / 1024.0;

                    int diskPercent = totalBytes > 0 ? (int)((usedBytes * 100) / totalBytes) : 0;
                    if (diskPercent > 100) diskPercent = 100;
                    if (diskPercent < 0) diskPercent = 0;

                    string fsType = "FAT";
                    try
                    {
                        fsType = Kernel.fs.GetFileSystemType(driveId);
                    }
                    catch { }

                    if (totalMB >= 1024)
                    {
                        double totalGB = totalMB / 1024.0;
                        double usedGB = usedMB / 1024.0;
                        diskTextLabel.Text = $"Disk ({fsType}): {usedGB:F1} GB / {totalGB:F1} GB (%{diskPercent})";
                    }
                    else
                    {
                        diskTextLabel.Text = $"Disk ({fsType}): {(int)usedMB} MB / {(int)totalMB} MB (%{diskPercent})";
                    }

                    diskProgressBar.Value = diskPercent;
                }
                else
                {
                    diskTextLabel.Text = "Disk: Dosya Sistemi Bulunamadi";
                    diskProgressBar.Value = 0;
                }
            }
            catch
            {
                diskTextLabel.Text = "Disk: Okuma Hatasi";
                diskProgressBar.Value = 0;
            }
        }

        public override void Close()
        {
        }
    }
}