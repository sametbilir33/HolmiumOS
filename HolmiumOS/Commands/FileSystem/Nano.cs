using System;
using System.IO;
using System.Text;
using HolmiumOS.Shell;

using Syste = System.Collections.Generic;
using Mat = System.Math;

namespace HolmiumOS.Commands.System
{
    public class Nano : ICommand
    {
        public string Name => "nano";
        public string Description => "Basit terminal metin editörü";
        public string Usage => "nano <dosya>";

        public void Execute(string args)
        {
            
        }

    }
}