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

        private bool modified = false;

        private int cursorX = 0;
        private int cursorY = 0;

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine("Kullanim: nano <dosya>");
                return;
            }

            string file = ResolvePath(args.Trim());

            if (!File.Exists(file))
                File.WriteAllText(file, "");

            // CRLF ve CR'yi normalize et, boş dosyada da tek satır garanti et
            string raw = File.ReadAllText(file).Replace("\r\n", "\n").Replace("\r", "\n");
            string[] lines = raw.Split('\n');

            if (lines.Length == 0)
                lines = new string[] { "" };

            cursorX = 0;
            cursorY = 0;

            while (true)
            {
                DrawUI(file, lines);

                var key = Console.ReadKey(true);

                if ((key.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if (key.Key == ConsoleKey.S)
                    {
                        Save(file, lines);
                        modified = false;
                        continue;
                    }

                    if (key.Key == ConsoleKey.X)
                        return;
                }

                HandleInput(key, ref lines);
            }
        }

        private void HandleInput(ConsoleKeyInfo key, ref string[] lines)
        {
            // Satır değişiminden geldiyse cursorX her zaman sınırlar içinde olsun
            ClampCursor(lines);

            string currentLine = lines[cursorY];

            switch (key.Key)
            {
                case ConsoleKey.Backspace:
                    if (cursorX > 0)
                    {
                        currentLine = currentLine.Remove(cursorX - 1, 1);
                        cursorX--;
                        modified = true;
                    }
                    else if (cursorY > 0)
                    {
                        // Satır başında backspace: önceki satırla birleştir
                        MergeWithPreviousLine(ref lines);
                        return; // lines ve cursor zaten güncellendi, currentLine'ı tekrar atama
                    }
                    break;

                case ConsoleKey.Delete:
                    if (cursorX < currentLine.Length)
                    {
                        currentLine = currentLine.Remove(cursorX, 1);
                        modified = true;
                    }
                    else if (cursorY < lines.Length - 1)
                    {
                        // Satır sonunda delete: sonraki satırı buraya birleştir
                        lines[cursorY] = currentLine;
                        MergeNextLineInto(ref lines, cursorY);
                        return;
                    }
                    break;

                case ConsoleKey.Enter:
                    InsertLine(ref lines, currentLine);
                    modified = true;
                    return; // InsertLine kendi içinde lines'ı güncelliyor

                case ConsoleKey.Home:
                    cursorX = 0;
                    break;

                case ConsoleKey.End:
                    cursorX = currentLine.Length;
                    break;

                case ConsoleKey.LeftArrow:
                    if (cursorX > 0)
                    {
                        cursorX--;
                    }
                    else if (cursorY > 0)
                    {
                        cursorY--;
                        cursorX = lines[cursorY].Length;
                    }
                    break;

                case ConsoleKey.RightArrow:
                    if (cursorX < currentLine.Length)
                    {
                        cursorX++;
                    }
                    else if (cursorY < lines.Length - 1)
                    {
                        cursorY++;
                        cursorX = 0;
                    }
                    break;

                case ConsoleKey.UpArrow:
                    if (cursorY > 0)
                    {
                        cursorY--;
                        cursorX = Mat.Min(cursorX, lines[cursorY].Length);
                    }
                    break;

                case ConsoleKey.DownArrow:
                    if (cursorY < lines.Length - 1)
                    {
                        cursorY++;
                        cursorX = Mat.Min(cursorX, lines[cursorY].Length);
                    }
                    break;

                default:
                    if (!char.IsControl(key.KeyChar))
                    {
                        currentLine = currentLine.Insert(cursorX, key.KeyChar.ToString());
                        cursorX++;
                        modified = true;
                    }
                    break;
            }

            lines[cursorY] = currentLine;
        }

        private void ClampCursor(string[] lines)
        {
            if (cursorY < 0) cursorY = 0;
            if (cursorY > lines.Length - 1) cursorY = lines.Length - 1;

            int len = lines[cursorY].Length;
            if (cursorX < 0) cursorX = 0;
            if (cursorX > len) cursorX = len;
        }

        private void InsertLine(ref string[] lines, string current)
        {
            var list = new Syste.List<string>(lines);

            string rest = current.Substring(cursorX);

            list[cursorY] = current.Substring(0, cursorX);
            list.Insert(cursorY + 1, rest);

            lines = list.ToArray();

            cursorY++;
            cursorX = 0;
        }

        private void MergeWithPreviousLine(ref string[] lines)
        {
            var list = new Syste.List<string>(lines);

            string current = list[cursorY];
            int prevLen = list[cursorY - 1].Length;

            list[cursorY - 1] += current;
            list.RemoveAt(cursorY);

            lines = list.ToArray();

            cursorY--;
            cursorX = prevLen;
        }

        private void MergeNextLineInto(ref string[] lines, int y)
        {
            var list = new Syste.List<string>(lines);

            list[y] += list[y + 1];
            list.RemoveAt(y + 1);

            lines = list.ToArray();
            modified = true;
        }

        private void DrawUI(string file, string[] lines)
        {
            Console.Clear();

            DrawTitleBar(file);
            DrawBody(lines);
            DrawStatusBar(file);

            PositionCursor(lines);
        }

        private void DrawTitleBar(string file)
        {
            string status = modified ? "MODIFIED" : "SAVED";
            Console.WriteLine($"Nano - {file} [{status}]");
            Console.WriteLine("CTRL+S Kaydet | CTRL+X Cikis");
            Console.WriteLine("--------------------------------");
        }

        private void DrawBody(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                Console.WriteLine(lines[i]);
            }
        }

        private void DrawStatusBar(string file)
        {
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Satir: {cursorY + 1}, Sutun: {cursorX + 1}");
        }

        private void PositionCursor(string[] lines)
        {
            // Baslik barinda 3 satir var (title, kisayollar, ayrac)
            const int headerLines = 3;

            int row = headerLines + cursorY;
            int col = cursorX;

            int maxRow = Console.BufferHeight - 1;
            int maxCol = Console.BufferWidth - 1;

            if (row > maxRow) row = maxRow;
            if (col > maxCol) col = maxCol;

            Console.SetCursorPosition(col, row);
        }

        private void Save(string file, string[] lines)
        {
            File.WriteAllText(file, string.Join("\n", lines));
        }

        private string ResolvePath(string file)
        {
            if (file.Contains(":\\"))
                return file;

            string current = FileSystemManager.CurrentDirectory;

            if (!current.EndsWith("\\"))
                current += "\\";

            return current + file;
        }
    }
}