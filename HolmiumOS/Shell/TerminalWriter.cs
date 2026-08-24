using System;
using System.IO;
using System.Text;

namespace HolmiumOS.Shell
{
    public class TerminalWriter : TextWriter
    {
        private readonly Action<string> onLine;
        private readonly Action clearAction;
        private readonly StringBuilder buffer = new StringBuilder();

        public static TerminalWriter Current { get; private set; }

        public TerminalWriter(Action<string> onLine, Action clearAction = null)
        {
            this.onLine = onLine;
            this.clearAction = clearAction;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public void Activate()
        {
            Current = this;
        }

        public void Deactivate()
        {
            if (Current == this)
                Current = null;
        }

        public override void Write(char value)
        {
            if (value == '\n')
            {
                FlushLine();
                return;
            }

            if (value != '\r')
                buffer.Append(value);
        }

        public override void Write(string value)
        {
            if (value == null)
                return;

            for (int i = 0; i < value.Length; i++)
                Write(value[i]);
        }

        public override void WriteLine(string value)
        {
            if (value != null)
                buffer.Append(value);

            FlushLine();
        }

        public override void WriteLine()
        {
            FlushLine();
        }

        public void FlushPending()
        {
            if (buffer.Length == 0)
                return;

            FlushLine();
        }

        private void FlushLine()
        {
            string line = buffer.ToString();
            buffer.Clear();

            onLine?.Invoke(line);
        }

        public void Clear()
        {
            buffer.Clear();
            clearAction?.Invoke();
        }

        public static void ClearCurrent()
        {
            Current?.Clear();
        }
    }
}