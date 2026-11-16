
using System;
using System.IO;
using HolmiumOS.GUI.Controls;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI.Apps
{
    public class Terminal : AppBase
    {
        private const int LineCount = 10;
        private const int LineHeight = 29;

        private Label[] outputLabels = Array.Empty<Label>();
        private Label? pathLabel;
        private TextBox? inputTextBox;
        private Button? submitButton;

        private TextWriter? previousOutput;
        private TerminalWriter? terminalWriter;
        private ShellContext? shellContext;

        public Terminal() : base("HolmiumOS Terminal")
        {
        }

        public override void Load()
        {
            if (Window == null)
                return;

            Window.Title = "Terminal";

            shellContext = new ShellContext(UserManager.HomeDirectory);

            Label headerLabel = new Label("HOLMIUMOS TERMINAL", 15, 12);
            Label separatorLabel = new Label("────────────────────────────────────────", 15, 43);

            Window.AddControl(headerLabel);
            Window.AddControl(separatorLabel);

            outputLabels = new Label[LineCount];

            for (int i = 0; i < LineCount; i++)
            {
                outputLabels[i] = new Label("", 15, 65 + (i * LineHeight));
                Window.AddControl(outputLabels[i]);
            }

            pathLabel = new Label("", 15, 365);
            Window.AddControl(pathLabel);

            inputTextBox = new TextBox(15, 400, 445, 42);
            inputTextBox.MaxLength = 200;

            submitButton = new Button("Calistir", 470, 400, 110, 48);
            submitButton.ClickAction = ExecuteInput;

            Window.AddControl(inputTextBox);
            Window.AddControl(submitButton);

            previousOutput = Console.Out;
            terminalWriter = new TerminalWriter(AddOutputLine, ClearOutput);

            AddOutputLine("HolmiumOS Terminal");
            AddOutputLine("Komutlari calistirmak icin asagidaki alani kullanin.");

            UpdatePathLabel();
        }

        private string GetShortPath()
        {
            if (shellContext == null)
                return "~";

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
                    string relativePath = currentPath.Substring(homePath.Length).TrimStart('\\').Replace('\\', '/');
                    return "~/" + relativePath;
                }
            }

            return currentPath.Replace('\\', '/');
        }

        private void UpdatePathLabel()
        {
            if (pathLabel != null)
                pathLabel.Text = GetShortPath();
        }

        private void AddOutputLine(string text)
        {
            if (outputLabels.Length == 0)
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
            for (int i = 0; i < outputLabels.Length; i++)
                outputLabels[i].Text = "";
        }

        private void ExecuteInput()
        {
            if (inputTextBox == null || terminalWriter == null || shellContext == null)
                return;

            string input = inputTextBox.Text;

            if (string.IsNullOrWhiteSpace(input))
                return;

            input = input.Trim();
            inputTextBox.Text = "";

            AddOutputLine("$ " + input);

            TextWriter oldOutput = Console.Out;

            try
            {
                FileSystemManager.ActiveContext = shellContext;
                Console.SetOut(terminalWriter);
                terminalWriter.Activate();

                CommandManager.ExecuteCommand(input);
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
