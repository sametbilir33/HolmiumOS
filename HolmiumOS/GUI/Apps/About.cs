using System;
using HolmiumOS.GUI.Controls;
using Cosmos.Core;
using Sys = Cosmos.System;

namespace HolmiumOS.GUI.Apps
{
    public class About : AppBase
    {
        private Label osInfoLabel;
        private Label cpuInfoLabel;
        private Label vmInfoLabel;
        private Label ramTextLabel;

        public About() : base("Hakkinda")
        {
        }

        public override void Load()
        {
            try
            {
                if (this.Window != null)
                {
                    this.Window.Title = "Sistem Hakkinda";
                }

                Label headerLabel = new Label("HolmiumOS GUI Ortami", 20, 15);

                osInfoLabel = new Label("Surum: " + Kernel.OSVERSION, 20, 35);

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

                if (this.Window != null)
                {
                    this.Window.AddControl(headerLabel);
                    this.Window.AddControl(osInfoLabel);
                    this.Window.AddControl(cpuInfoLabel);
                    this.Window.AddControl(vmInfoLabel);
                    this.Window.AddControl(ramTextLabel);
                }
            }
            catch
            {
            }
        }

        public override void Close()
        {
        }
    }
}