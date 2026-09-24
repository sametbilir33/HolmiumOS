using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Storage;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class About : AppBase
    {
        private const string CosmosVersion = "3.0.86";

        public About() : base("Hakkinda")
        {
        }

        public override void Load()
        {
            if (Window != null)
            {
                Window.Title = "HolmiumOS Gen3 System Intelligence";
            }

            AddHeader();

            AddRuntimeSection();
            AddHardwareSection();
            AddGraphicsSection();
            AddStorageSection();
            AddBootSection();
        }

        private void AddHeader()
        {
            Label title = new Label(
                "HOLMIUMOS",
                20,
                15);

            Label subtitle = new Label(
                "GEN3 SYSTEM INTELLIGENCE",
                20,
                35);

            Label separator = new Label(
                "================================",
                20,
                52);

            Add(title);
            Add(subtitle);
            Add(separator);
        }

        private void AddRuntimeSection()
        {
            Label section = new Label(
                "[ RUNTIME ]",
                20,
                75);

            Add(section);

            string architecture =
                Environment.Is64BitProcess
                    ? "x86_64 / 64-bit"
                    : "32-bit";

            string runtimeVersion =
                Environment.Version.ToString();

            string executionMode =
                RuntimeFeature.IsDynamicCodeSupported
                    ? "JIT / Dynamic Code"
                    : "NativeAOT";

            Label os = new Label(
                "OS       : HolmiumOS Gen3",
                20,
                95);

            Label cosmos = new Label(
                "Cosmos    : " + CosmosVersion,
                20,
                112);

            Label dotnet = new Label(
                ".NET      : " + runtimeVersion,
                20,
                129);

            Label arch = new Label(
                "Arch      : " + architecture,
                20,
                146);

            Label execution = new Label(
                "Execution : " + executionMode,
                20,
                163);

            Add(os);
            Add(cosmos);
            Add(dotnet);
            Add(arch);
            Add(execution);
        }

        private void AddHardwareSection()
        {
            Label section = new Label(
                "[ HARDWARE ]",
                320,
                75);

            Add(section);

            string cpuInfo;

            if (Environment.Is64BitProcess)
            {
                cpuInfo = "x86_64 processor";
            }
            else
            {
                cpuInfo = "32-bit processor";
            }

            Label cpu = new Label(
                "CPU       : " + cpuInfo,
                320,
                95);

            Label cpuApi = new Label(
                "CPU data  : Gen3 public API",
                320,
                112);

            Label ram = new Label(
                "RAM       : Kernel API unavailable",
                320,
                129);

            Label memory = new Label(
                "Memory    : NativeAOT runtime",
                320,
                146);

            Label hardware = new Label(
                "HAL       : Gen3 x64 HAL",
                320,
                163);

            Add(cpu);
            Add(cpuApi);
            Add(ram);
            Add(memory);
            Add(hardware);
        }

        private void AddGraphicsSection()
        {
            Label section = new Label(
                "[ GRAPHICS ]",
                20,
                195);

            Add(section);

            try
            {
                Canvas canvas = Init.canvas;

                if (canvas == null)
                {
                    Add(new Label(
                        "Graphics  : Canvas unavailable",
                        20,
                        215));

                    return;
                }

                int width = (int)canvas.Mode.Width;
                int height = (int)canvas.Mode.Height;
                int depth = (int)canvas.Mode.ColorDepth;
                int refresh = canvas.RefreshRate;

                string canvasName = canvas.Name;

                Label resolution = new Label(
                    "Resolution: " +
                    width +
                    " x " +
                    height,
                    20,
                    215);

                Label colorDepth = new Label(
                    "Color     : " +
                    depth +
                    " bit",
                    20,
                    232);

                Label refreshRate = new Label(
                    "Refresh   : " +
                    refresh +
                    " Hz",
                    20,
                    249);

                Label backend = new Label(
                    "Canvas    : " +
                    canvasName,
                    20,
                    266);

                int modeCount = 0;

                if (canvas.AvailableModes != null)
                {
                    modeCount = canvas.AvailableModes.Count;
                }

                Label modes = new Label(
                    "Modes     : " +
                    modeCount +
                    " available",
                    20,
                    283);

                Add(resolution);
                Add(colorDepth);
                Add(refreshRate);
                Add(backend);
                Add(modes);
            }
            catch
            {
                Add(new Label(
                    "Graphics  : Information unavailable",
                    20,
                    215));
            }
        }

        private void AddStorageSection()
        {
            Label section = new Label(
                "[ STORAGE ]",
                320,
                195);

            Add(section);

            try
            {
                int deviceCount = StorageManager.DeviceCount;
                int partitionCount =
                    StorageManager.Partitions.Count;

                Label devices = new Label(
                    "Devices   : " +
                    deviceCount,
                    320,
                    215);

                Label partitions = new Label(
                    "Partitions: " +
                    partitionCount,
                    320,
                    232);

                Add(devices);
                Add(partitions);

                int y = 249;

                for (int i = 0;
                     i < StorageManager.Devices.Count &&
                     i < 4;
                     i++)
                {
                    var device =
                        StorageManager.Devices[i];

                    ulong totalBytes =
                        device.BlockCount *
                        device.BlockSize;

                    string size =
                        FormatBytes(totalBytes);

                    Label deviceLabel = new Label(
                        device.Name +
                        "  " +
                        size,
                        320,
                        y);

                    Add(deviceLabel);

                    Label geometry = new Label(
                        "  " +
                        device.BlockCount +
                        " blocks x " +
                        device.BlockSize +
                        " B",
                        320,
                        y + 17);

                    Add(geometry);

                    y += 34;
                }

                if (deviceCount == 0)
                {
                    Add(new Label(
                        "No block devices detected",
                        320,
                        249));
                }
            }
            catch
            {
                Add(new Label(
                    "Storage   : Information unavailable",
                    320,
                    215));
            }
        }

        private void AddBootSection()
        {
            Label section = new Label(
                "[ BOOT / KERNEL ]",
                20,
                315);

            Add(section);

            Label bootloader = new Label(
                "Bootloader: Limine",
                20,
                335);

            Label protocol = new Label(
                "Protocol  : Limine Boot Protocol",
                20,
                352);

            Label kernel = new Label(
                "Kernel    : Cosmos Gen3",
                20,
                369);

            Label target = new Label(
                "Target    : x64",
                20,
                386);

            Label storage = new Label(
                "Storage   : " +
                (StorageManager.IsEnabled
                    ? "Enabled"
                    : "Disabled"),
                20,
                403);

            Label initialized = new Label(
                "StorageInit: " +
                (StorageManager.IsInitialized
                    ? "Ready"
                    : "Not initialized"),
                20,
                420);

            Add(bootloader);
            Add(protocol);
            Add(kernel);
            Add(target);
            Add(storage);
            Add(initialized);

            AddFooter();
        }

        private void AddFooter()
        {
            Label separator = new Label(
                "================================",
                20,
                447);

            Label footer = new Label(
                "HolmiumOS Gen3  |  Cosmos SDK " +
                CosmosVersion,
                20,
                465);

            Add(separator);
            Add(footer);
        }

        private static string FormatBytes(ulong bytes)
        {
            if (bytes >= 1024UL * 1024UL * 1024UL)
            {
                ulong gb =
                    bytes /
                    (1024UL * 1024UL * 1024UL);

                return gb + " GB";
            }

            if (bytes >= 1024UL * 1024UL)
            {
                ulong mb =
                    bytes /
                    (1024UL * 1024UL);

                return mb + " MB";
            }

            if (bytes >= 1024UL)
            {
                ulong kb =
                    bytes /
                    1024UL;

                return kb + " KB";
            }

            return bytes + " B";
        }

        private void Add(Control control)
        {
            if (Window != null)
            {
                Window.AddControl(control);
            }
        }

        public override void Close()
        {
        }
    }
}