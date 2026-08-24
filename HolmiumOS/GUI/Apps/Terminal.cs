using System;
using System.IO;
using HolmiumOS.GUI.Controls;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI.Apps
{
    public class Terminal : AppBase
    {
        private const int LineCount = 20;

        private Label[] outputLabels;
        private Label pathLabel;

        private TextBox inputTextBox;
        private Button submitButton;

        private TextWriter previousOutput;
        private TerminalWriter terminalWriter;
        private ShellContext shellContext;

        public Terminal() : base("HolmiumOS Terminal")
        {
        }

        public override void Load()
        {
            if (this.Window == null)
                return;

            this.Window.Title = "HolmiumOS Terminal";

            shellContext = new ShellContext(UserManager.HomeDirectory);

            outputLabels = new Label[LineCount];

            for (int i = 0; i < LineCount; i++)
            {
                outputLabels[i] = new Label("", 10, 10 + (i * 18));
                this.Window.AddControl(outputLabels[i]);
            }

            pathLabel = new Label("", 10, 375);
            this.Window.AddControl(pathLabel);

            inputTextBox = new TextBox(100, 372, 370, 25);
            inputTextBox.MaxLength = 200;

            submitButton = new Button("Gonder", 480, 372, 90, 25);
            submitButton.ClickAction = ExecuteInput;

            this.Window.AddControl(inputTextBox);
            this.Window.AddControl(submitButton);

            previousOutput = Console.Out;

            terminalWriter = new TerminalWriter(
                AddOutputLine,
                ClearOutput
            );

            UpdatePathLabel();
        }

        private string GetShortPath()
        {
            string currentPath = shellContext.CurrentDirectory;
            string homePath = UserManager.HomeDirectory;

            if (string.IsNullOrEmpty(currentPath))
                return "~";

            currentPath = currentPath.Replace('/', '\\');

            if (!string.IsNullOrEmpty(homePath))
            {
                homePath = homePath.Replace('/', '\\');

                if (currentPath.Equals(homePath, StringComparison.OrdinalIgnoreCase))
                    return "~";

                if (currentPath.StartsWith(homePath + "\\", StringComparison.OrdinalIgnoreCase))
                {
                    string relativePath = currentPath
                        .Substring(homePath.Length)
                        .TrimStart('\\')
                        .Replace('\\', '/');

                    return "~/" + relativePath;
                }
            }

            return currentPath.Replace('\\', '/');
        }

        private void UpdatePathLabel()
        {
            if (pathLabel == null)
                return;

            pathLabel.Text = GetShortPath();
        }

        private void AddOutputLine(string text)
        {
            if (outputLabels == null)
                return;

            int emptyIndex = -1;

            for (int i = 0; i < LineCount; i++)
            {
                if (string.IsNullOrEmpty(outputLabels[i].Text))
                {
                    emptyIndex = i;
                    break;
                }
            }

            if (emptyIndex >= 0)
            {
                outputLabels[emptyIndex].Text = text;
                return;
            }

            for (int i = 0; i < LineCount - 1; i++)
                outputLabels[i].Text = outputLabels[i + 1].Text;

            outputLabels[LineCount - 1].Text = text;
        }

        private void ClearOutput()
        {
            if (outputLabels == null)
                return;

            for (int i = 0; i < LineCount; i++)
                outputLabels[i].Text = "";
        }

        private void ExecuteInput()
        {
            if (inputTextBox == null)
                return;

            string input = inputTextBox.Text;

            if (string.IsNullOrWhiteSpace(input))
                return;

            inputTextBox.Text = "";

            ClearOutput();

            TextWriter oldOutput = Console.Out;

            try
            {
                FileSystemManager.ActiveContext = shellContext;

                Console.SetOut(terminalWriter);
                terminalWriter.Activate();

                CommandManager.ExecuteCommand(input.Trim());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
            }
            finally
            {
                terminalWriter.FlushPending();
                terminalWriter.Deactivate();

                FileSystemManager.ActiveContext = null;
                Console.SetOut(oldOutput);
            }

            UpdatePathLabel();
        }

        public override void Close()
        {
            if (terminalWriter != null)
            {
                terminalWriter.FlushPending();
                terminalWriter.Deactivate();
            }

            if (FileSystemManager.ActiveContext == shellContext)
                FileSystemManager.ActiveContext = null;

            if (previousOutput != null)
                Console.SetOut(previousOutput);

            previousOutput = null;
            terminalWriter = null;
            shellContext = null;
        }
    }
}