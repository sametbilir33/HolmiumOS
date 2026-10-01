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

        private const int LeftX = 20;
        private const int RightX = 500;

        private const int LineHeight = 32;
        private const int SectionGap = 18;

        public About() : base("Hakkinda")
        {
        }

        public override void Load()
        {
            if (Window != null)
            {
                Window.Title = "HolmiumOS Hakkinda";
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
                LeftX,
                15);

            Label subtitle = new Label(
                "HAKKINDA",
                LeftX,
                47);

            Label separator = new Label(
                "================================",
                LeftX,
                79);

            Add(title);
            Add(subtitle);
            Add(separator);
        }

        private void AddRuntimeSection()
        {
            Label section = new Label(
                "[ RUNTIME ]",
                LeftX,
                120);

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
                "OS        : HolmiumOS Gen3",
                LeftX,
                152);

            Label cosmos = new Label(
                "Cosmos    : " + CosmosVersion,
                LeftX,
                184);

            Label dotnet = new Label(
                ".NET      : " + runtimeVersion,
                LeftX,
                216);

            Label arch = new Label(
                "Arch      : " + architecture,
                LeftX,
                248);

            Label execution = new Label(
                "Execution : " + executionMode,
                LeftX,
                280);

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
                RightX,
                120);

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
                RightX,
                152);

            Label cpuApi = new Label(
                "CPU data  : Gen3 public API",
                RightX,
                184);

            Label ram = new Label(
                "RAM       : Kernel API unavailable",
                RightX,
                216);

            Label memory = new Label(
                "Memory    : NativeAOT runtime",
                RightX,
                248);

            Label hardware = new Label(
                "HAL       : Gen3 x64 HAL",
                RightX,
                280);

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
                LeftX,
                340);

            Add(section);

            try
            {
                Canvas canvas = Init.canvas;

                if (canvas == null)
                {
                    Add(new Label(
                        "Graphics  : Canvas unavailable",
                        LeftX,
                        372));

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
                    LeftX,
                    372);

                Label colorDepth = new Label(
                    "Color     : " +
                    depth +
                    " bit",
                    LeftX,
                    404);

                Label refreshRate = new Label(
                    "Refresh   : " +
                    refresh +
                    " Hz",
                    LeftX,
                    436);

                Label backend = new Label(
                    "Canvas    : " +
                    canvasName,
                    LeftX,
                    468);

                int modeCount = 0;

                if (canvas.AvailableModes != null)
                {
                    modeCount = canvas.AvailableModes.Count;
                }

                Label modes = new Label(
                    "Modes     : " +
                    modeCount +
                    " available",
                    LeftX,
                    500);

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
                    LeftX,
                    372));
            }
        }

        private void AddStorageSection()
        {
            Label section = new Label(
                "[ STORAGE ]",
                RightX,
                340);

            Add(section);

            try
            {
                int deviceCount = StorageManager.DeviceCount;

                int partitionCount =
                    StorageManager.Partitions.Count;

                Label devices = new Label(
                    "Devices   : " +
                    deviceCount,
                    RightX,
                    372);

                Label partitions = new Label(
                    "Partitions: " +
                    partitionCount,
                    RightX,
                    404);

                Add(devices);
                Add(partitions);

                int y = 436;

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
                        RightX,
                        y);

                    Add(deviceLabel);

                    Label geometry = new Label(
                        "  " +
                        device.BlockCount +
                        " blocks x " +
                        device.BlockSize +
                        " B",
                        RightX,
                        y + 32);

                    Add(geometry);

                    y += 64;
                }

                if (deviceCount == 0)
                {
                    Add(new Label(
                        "No block devices detected",
                        RightX,
                        436));
                }
            }
            catch
            {
                Add(new Label(
                    "Storage   : Information unavailable",
                    RightX,
                    372));
            }
        }

        private void AddBootSection()
        {
            Label section = new Label(
                "[ BOOT / KERNEL ]",
                LeftX,
                560);

            Add(section);

            Label bootloader = new Label(
                "Bootloader: Limine",
                LeftX,
                592);

            Label protocol = new Label(
                "Protocol  : Limine Boot Protocol",
                LeftX,
                624);

            Label kernel = new Label(
                "Kernel    : Cosmos Gen3",
                LeftX,
                656);

            Label target = new Label(
                "Target    : x64",
                LeftX,
                688);

            Label storage = new Label(
                "Storage   : " +
                (StorageManager.IsEnabled
                    ? "Enabled"
                    : "Disabled"),
                LeftX,
                720);

            Label initialized = new Label(
                "StorageInit: " +
                (StorageManager.IsInitialized
                    ? "Ready"
                    : "Not initialized"),
                LeftX,
                752);

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
                LeftX,
                800);

            Label footer = new Label(
                "HolmiumOS Gen3  |  Cosmos SDK " +
                CosmosVersion,
                LeftX,
                832);

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