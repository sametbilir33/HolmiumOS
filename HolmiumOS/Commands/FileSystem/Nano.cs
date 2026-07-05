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

        // Cosmos'ta Console.SetCursorPosition / BufferHeight / BufferWidth
        // guvenilir degil (Invalid Opcode CPU exception'ina yol aciyor).
        // Bu yuzden gercek terminal imleci hic kullanilmiyor; ekran her
        // dongude bastan ciziliyor ve imlec, metnin icine yerlestirilen
        // gorsel bir isaretleyici (marker) karakterle gosteriliyor.
        private const char CursorMarker = '_';

        private const int ScreenCols = 80;
        private const int ScreenRows = 25;

        // Basliktaki satir sayisi (title, kisayollar, ayrac) ve
        // alttaki satir sayisi (ayrac, durum satiri)
        private const int HeaderLines = 3;
        private const int FooterLines = 2;
        private const int VisibleBodyLines = ScreenRows - HeaderLines - FooterLines;

        // Dikey kaydirma (scroll) icin ekranin ustunden gorunen ilk satir
        private int topLine = 0;

        // Dosya diskte gercekten var mi (kaydetmeden diske yazmamak icin)
        private bool fileExistsOnDisk = false;

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine("Kullanim: nano <dosya>");
                return;
            }

            string file = ResolvePath(args.Trim());

            string[] lines;

            if (File.Exists(file))
            {
                fileExistsOnDisk = true;

                // CRLF ve CR'yi normalize et
                string raw = File.ReadAllText(file).Replace("\r\n", "\n").Replace("\r", "\n");
                lines = raw.Split('\n');

                if (lines.Length == 0)
                    lines = new string[] { "" };
            }
            else
            {
                // Dosya henuz diskte yok; kullanici CTRL+S yapmadan
                // hicbir sey yazilmayacak (gercek nano gibi).
                fileExistsOnDisk = false;
                lines = new string[] { "" };
            }

            cursorX = 0;
            cursorY = 0;
            topLine = 0;

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
                        fileExistsOnDisk = true;
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
            // Satir degisiminden geldiyse cursorX her zaman sinirlar icinde olsun
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
                        // Satir basinda backspace: onceki satirla birlestir
                        MergeWithPreviousLine(ref lines);
                        return;
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
                        lines[cursorY] = currentLine;
                        MergeNextLineInto(ref lines, cursorY);
                        return;
                    }
                    break;

                case ConsoleKey.Enter:
                    InsertLine(ref lines, currentLine);
                    modified = true;
                    return;

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

                case ConsoleKey.PageUp:
                    cursorY = Mat.Max(0, cursorY - VisibleBodyLines);
                    cursorX = Mat.Min(cursorX, lines[cursorY].Length);
                    break;

                case ConsoleKey.PageDown:
                    cursorY = Mat.Min(lines.Length - 1, cursorY + VisibleBodyLines);
                    cursorX = Mat.Min(cursorX, lines[cursorY].Length);
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

        // Cursor gorunen pencerenin (viewport) disina cikinca kaydirma
        // ofsetini (topLine) ayarlar. Uzun dosyalarda ekranin tasmamasi
        // icin bu sart.
        private void AdjustScroll(string[] lines)
        {
            if (cursorY < topLine)
                topLine = cursorY;

            if (cursorY > topLine + VisibleBodyLines - 1)
                topLine = cursorY - VisibleBodyLines + 1;

            int maxTop = Mat.Max(0, lines.Length - VisibleBodyLines);
            if (topLine > maxTop) topLine = maxTop;
            if (topLine < 0) topLine = 0;
        }

        private void DrawUI(string file, string[] lines)
        {
            AdjustScroll(lines);

            Console.Clear();

            DrawTitleBar(file, lines);
            DrawBody(lines);
            DrawStatusBar(file, lines);

            // Not: Burada bilerek Console.SetCursorPosition CAGRILMIYOR.
            // Cosmos'ta bu API guvenilir degil ve Invalid Opcode CPU
            // exception'ina yol aciyordu. Imlecin nerede oldugu, DrawBody
            // icinde metnin icine gomulen CursorMarker karakteriyle
            // gosteriliyor.
        }

        private void DrawTitleBar(string file, string[] lines)
        {
            string status = modified ? "MODIFIED" : "SAVED";
            Console.WriteLine($"Nano - {file} [{status}]");
            Console.WriteLine("CTRL+S Kaydet | CTRL+X Cikis | PgUp/PgDn Kaydir");
            Console.WriteLine("--------------------------------");
        }

        private void DrawBody(string[] lines)
        {
            int shown = 0;

            for (int i = topLine; i < lines.Length && shown < VisibleBodyLines; i++, shown++)
            {
                string line = lines[i];

                if (i == cursorY)
                {
                    // Imlecin bulundugu satirda, SetCursorPosition yerine
                    // gorsel bir isaretleyici karakteri metnin icine
                    // yerlestiriyoruz (sadece ekranda gosterim icin,
                    // gercek veriyi degistirmiyor).
                    int markerPos = Mat.Min(cursorX, line.Length);
                    line = line.Insert(markerPos, CursorMarker.ToString());
                }

                // Ekran genisligini asan satirlari kirp; asmayan Cosmos
                // konsolunda beklenmedik satir kaymasina yol acabiliyor.
                if (line.Length > ScreenCols)
                    line = line.Substring(0, ScreenCols);

                Console.WriteLine(line);
            }

            // Dosyanin sonuna gelindiyse kalan bos satirlari doldur ki
            // durum cubugu her zaman ayni satirda kalsin.
            for (; shown < VisibleBodyLines; shown++)
            {
                Console.WriteLine();
            }
        }

        private void DrawStatusBar(string file, string[] lines)
        {
            Console.WriteLine("--------------------------------");

            string scrollInfo = lines.Length > VisibleBodyLines
                ? $" | Gorunen: {topLine + 1}-{Mat.Min(topLine + VisibleBodyLines, lines.Length)}/{lines.Length}"
                : "";

            Console.WriteLine($"Satir: {cursorY + 1}, Sutun: {cursorX + 1}{scrollInfo}");
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