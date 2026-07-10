using System;
using hal = Cosmos.HAL;

namespace HolmiumOS.Commands.System
{
    public class PCI : ICommand
    {
        public string Name => "pci";
        public string Description => "PCI aygiti bilgilerini goruntuler.";
        public string Usage => "pci [info <index>|storage|network|display|raw]";

        public void Execute(string args)
        {
            if (hal.PCI.Devices == null || hal.PCI.Devices.Count == 0)
            {
                Console.WriteLine("No PCI devices found.");
                return;
            }

            args = (args ?? "").Trim();

            if (args.Length == 0)
            {
                Console.WriteLine($"PCI Devices ({hal.PCI.Count})");
                Console.WriteLine();

                for (int i = 0; i < hal.PCI.Devices.Count; i++)
                {
                    var d = hal.PCI.Devices[i];

                    Console.WriteLine($"[{i}] {hal.PCIDevice.DeviceClass.GetDeviceString(d)}");
                    Console.WriteLine($"    {hal.PCIDevice.DeviceClass.GetTypeString(d)}");
                    Console.WriteLine($"    VID:0x{d.VendorID:X4} DID:0x{d.DeviceID:X4}");
                    Console.WriteLine();
                }

                return;
            }

            var split = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            switch (split[0].ToLower())
            {
                case "info":
                    if (split.Length < 2 || !int.TryParse(split[1], out int index))
                    {
                        Console.WriteLine("Usage: pci info <index>");
                        return;
                    }

                    if (index < 0 || index >= hal.PCI.Devices.Count)
                    {
                        Console.WriteLine("Invalid device index.");
                        return;
                    }

                    {
                        var d = hal.PCI.Devices[index];

                        Console.WriteLine($"Index      : {index}");
                        Console.WriteLine($"Name       : {hal.PCIDevice.DeviceClass.GetDeviceString(d)}");
                        Console.WriteLine($"Type       : {hal.PCIDevice.DeviceClass.GetTypeString(d)}");
                        Console.WriteLine($"Vendor ID  : 0x{d.VendorID:X4}");
                        Console.WriteLine($"Device ID  : 0x{d.DeviceID:X4}");
                        Console.WriteLine($"Bus        : {d.bus}");
                        Console.WriteLine($"Slot       : {d.slot}");
                        Console.WriteLine($"Function   : {d.function}");
                        Console.WriteLine($"Class      : 0x{d.ClassCode:X2}");
                        Console.WriteLine($"Subclass   : 0x{d.Subclass:X2}");
                        Console.WriteLine($"ProgIF     : 0x{d.ProgIF:X2}");
                        Console.WriteLine($"Revision   : 0x{d.RevisionID:X2}");
                        //Console.WriteLine($"HeaderType : {d.HeaderType}");  ////////HATA ÇIKARTIYOR////////////
                        Console.WriteLine($"IRQ        : {d.InterruptLine}");
                        Console.WriteLine($"BAR0       : 0x{d.BAR0:X8}");
                    }
                    break;

                case "storage":
                case "network":
                case "display":
                    byte classCode = split[0] switch
                    {
                        "storage" => (byte)0x01,
                        "network" => (byte)0x02,
                        _ => (byte)0x03
                    };

                    bool found = false;

                    for (int i = 0; i < hal.PCI.Devices.Count; i++)
                    {
                        var d = hal.PCI.Devices[i];

                        if (d.ClassCode != classCode)
                            continue;

                        found = true;

                        Console.WriteLine($"[{i}] {hal.PCIDevice.DeviceClass.GetDeviceString(d)}");
                        Console.WriteLine($"    {hal.PCIDevice.DeviceClass.GetTypeString(d)}");
                        Console.WriteLine($"    VID:0x{d.VendorID:X4} DID:0x{d.DeviceID:X4}");
                        Console.WriteLine();
                    }

                    if (!found)
                        Console.WriteLine("No matching devices.");

                    break;

                case "raw":
                    for (int i = 0; i < hal.PCI.Devices.Count; i++)
                    {
                        var d = hal.PCI.Devices[i];

                        Console.WriteLine($"[{i}]");

                        Console.WriteLine($"VendorID   : 0x{d.VendorID:X4}");
                        Console.WriteLine($"DeviceID   : 0x{d.DeviceID:X4}");
                        Console.WriteLine($"Class      : 0x{d.ClassCode:X2}");
                        Console.WriteLine($"Subclass   : 0x{d.Subclass:X2}");
                        Console.WriteLine($"ProgIF     : 0x{d.ProgIF:X2}");
                        Console.WriteLine($"Revision   : 0x{d.RevisionID:X2}");
                        Console.WriteLine($"Bus        : {d.bus}");
                        Console.WriteLine($"Slot       : {d.slot}");
                        Console.WriteLine($"Function   : {d.function}");
                        //Console.WriteLine($"HeaderType : {d.HeaderType}");  ////////HATA ÇIKARTIYOR////////////
                        Console.WriteLine($"IRQ        : {d.InterruptLine}");
                        Console.WriteLine($"BAR0       : 0x{d.BAR0:X8}");

                        if (d.BaseAddressBar != null)
                        {
                            for (int j = 0; j < d.BaseAddressBar.Length; j++)
                            {
                                var bar = d.BaseAddressBar[j];

                                if (bar == null)
                                    continue;

                                Console.WriteLine($"BAR{j}       : 0x{bar.BaseAddress:X8} {(bar.IsIO ? "(IO)" : "(MEM)")}");
                            }
                        }

                        Console.WriteLine();
                    }

                    break;

                default:
                    Console.WriteLine("Usage:");
                    Console.WriteLine("  pci");
                    Console.WriteLine("  pci info <index>");
                    Console.WriteLine("  pci storage");
                    Console.WriteLine("  pci network");
                    Console.WriteLine("  pci display");
                    Console.WriteLine("  pci raw");
                    break;
            }
        }
    }
}