using HolmiumOS.Shell;
using System;

namespace HolmiumOS.Commands.System
{
    public class Ember : ICommand
    {
        public string Name => "ember";
        public string Description => "Yeni, temiz bir Ember kabugu acar ve komut gecmisini siler";
        public string Usage => "ember";

        public void Execute(string args)
        {
            CommandHistory.Clear();
            Console.Clear();
        }
    }
}