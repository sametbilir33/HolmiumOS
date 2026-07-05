using System;
using System.Collections.Generic;
using System.IO;
using HolmiumOS.Shell;

namespace HolmiumOS.HE
{
    public class HeInterpreter
    {
        private enum ValueType { Int, Str, Bool }
        private bool _inFunc;

        private struct Value
        {
            public ValueType Type;
            public int IntValue;
            public string StrValue;
            public bool BoolValue;

            public static Value FromInt(int v) => new() { Type = ValueType.Int, IntValue = v };
            public static Value FromStr(string v) => new() { Type = ValueType.Str, StrValue = v };
            public static Value FromBool(bool v) => new() { Type = ValueType.Bool, BoolValue = v };
        }

        private readonly Dictionary<string, Value> _vars = new();
        private readonly Dictionary<string, int> _funcs = new();

        private string[] _lines = Array.Empty<string>();
        private int _ip;

        public void Run(string path)
        {
            if (!File.Exists(path))
                return;

            _lines = File.ReadAllLines(path);
            _funcs.Clear();

            for (int i = 0; i < _lines.Length; i++)
            {
                var l = _lines[i].Trim();

                if (l.StartsWith("func "))
                    _funcs[l.Substring(5).Trim()] = i;
            }

            _ip = 0;

            while (_ip < _lines.Length)
            {
                var line = _lines[_ip].Trim();

                if (line.Length == 0 || line[0] == '#')
                {
                    _ip++;
                    continue;
                }

                Execute(line);
                _ip++;
            }
        }

        private void Execute(string line)
        {
            if (line.StartsWith("os.run("))
            {
                int s = line.IndexOf('(');
                int e = line.LastIndexOf(')');

                if (s == -1 || e == -1 || e <= s)
                    return;

                string cmd = ExtractString(line.Substring(s + 1, e - s - 1));

                if (!string.IsNullOrWhiteSpace(cmd))
                    CommandManager.ExecuteCommand(cmd);

                return;
            }

            if (line.StartsWith("print("))
            {
                var inner = GetInside(line);
                var v = Eval(inner);
                Console.WriteLine(ToString(v));
                return;
            }

            if (line.StartsWith("let "))
            {
                var parts = line.Substring(4).Split('=', 2);
                if (parts.Length != 2) return;

                _vars[parts[0].Trim()] = Eval(parts[1].Trim());
                return;
            }

            if (line.StartsWith("if("))
            {
                if (!ToBool(Eval(GetInside(line))))
                    SkipBlock();
                return;
            }

            if (line.StartsWith("call "))
            {
                RunFunc(line.Substring(5).Trim());
                return;
            }

            // FS

            if (line.StartsWith("fs.write "))
            {
                var args = SplitArgs(line.Substring(9));
                if (args.Length != 2) return;

                File.WriteAllText(ResolveFsPath(args[0]), args[1]);
                return;
            }

            if (line.StartsWith("fs.read "))
            {
                var args = SplitArgs(line.Substring(8));
                if (args.Length != 1) return;

                var path = ResolveFsPath(args[0]);
                if (File.Exists(path))
                    Console.WriteLine(File.ReadAllText(path));

                return;
            }

            if (line.StartsWith("fs.exists "))
            {
                var args = SplitArgs(line.Substring(10));
                if (args.Length != 1) return;

                Console.WriteLine(File.Exists(ResolveFsPath(args[0])).ToString().ToLower());
                return;
            }

            if (line.StartsWith("fs.ls"))
            {
                var raw = line.Substring(5).Trim();
                var path = FileSystemManager.CurrentDirectory;

                if (!string.IsNullOrWhiteSpace(raw))
                {
                    var a = SplitArgs(raw);
                    if (a.Length > 0)
                        path = ResolveFsPath(a[0]);
                }

                if (Directory.Exists(path))
                {
                    foreach (var d in Directory.GetDirectories(path))
                        Console.WriteLine("[DIR] " + Path.GetFileName(d));

                    foreach (var f in Directory.GetFiles(path))
                        Console.WriteLine("[FILE] " + Path.GetFileName(f));
                }

                return;
            }
        }

        // =========================
        // EXPRESSION ENGINE
        // =========================

        private Value Eval(string expr)
        {
            expr = expr.Trim();

            if (_vars.TryGetValue(expr, out var v))
                return v;

            if (expr.Length > 1 && expr[0] == '"' && expr[^1] == '"')
                return Value.FromStr(expr.Substring(1, expr.Length - 2));

            return ParseExpression(expr);
        }

        private Value ParseExpression(string expr)
        {
            expr = expr.Trim();

            if (expr.StartsWith("(") && expr.EndsWith(")"))
                return ParseExpression(expr.Substring(1, expr.Length - 2));

            int depth = 0;
            for (int i = expr.Length - 1; i >= 0; i--)
            {
                char c = expr[i];

                if (c == ')') depth++;
                else if (c == '(') depth--;
                else if (depth == 0 && (c == '+' || c == '-'))
                {
                    var left = expr.Substring(0, i);
                    var right = expr.Substring(i + 1);

                    if (c == '+')
                        return Value.FromInt(ToInt(Eval(left)) + ToInt(Eval(right)));

                    return Value.FromInt(ToInt(Eval(left)) - ToInt(Eval(right)));
                }
            }

            depth = 0;
            for (int i = expr.Length - 1; i >= 0; i--)
            {
                char c = expr[i];

                if (c == ')') depth++;
                else if (c == '(') depth--;
                else if (depth == 0 && (c == '*' || c == '/'))
                {
                    var left = expr.Substring(0, i);
                    var right = expr.Substring(i + 1);

                    if (c == '*')
                        return Value.FromInt(ToInt(Eval(left)) * ToInt(Eval(right)));

                    int r = ToInt(Eval(right));
                    return Value.FromInt(r == 0 ? 0 : ToInt(Eval(left)) / r);
                }
            }

            if (int.TryParse(expr, out int n))
                return Value.FromInt(n);

            if (expr == "true") return Value.FromBool(true);
            if (expr == "false") return Value.FromBool(false);

            return Value.FromStr(expr);
        }

        // =========================
        // HELPERS
        // =========================

        private string GetInside(string line)
        {
            int s = line.IndexOf('(');
            int e = line.LastIndexOf(')');

            if (s == -1 || e == -1 || e <= s)
                return "";

            return line.Substring(s + 1, e - s - 1);
        }

        private string ExtractString(string input)
        {
            input = input.Trim();

            if (input.Length >= 2 && input[0] == '"' && input[^1] == '"')
                return input.Substring(1, input.Length - 2);

            return input;
        }

        private int ToInt(Value v)
        {
            return v.Type switch
            {
                ValueType.Int => v.IntValue,
                ValueType.Bool => v.BoolValue ? 1 : 0,
                _ => int.TryParse(v.StrValue, out int n) ? n : 0
            };
        }

        private bool ToBool(Value v)
        {
            return v.Type switch
            {
                ValueType.Bool => v.BoolValue,
                ValueType.Int => v.IntValue != 0,
                _ => v.StrValue == "true"
            };
        }

        private string ToString(Value v)
        {
            return v.Type switch
            {
                ValueType.Int => v.IntValue.ToString(),
                ValueType.Bool => v.BoolValue ? "true" : "false",
                _ => v.StrValue ?? ""
            };
        }

        private string[] SplitArgs(string input)
        {
            var list = new List<string>();
            bool q = false;
            string cur = "";

            foreach (var c in input)
            {
                if (c == '"')
                {
                    q = !q;
                    continue;
                }

                if (c == ' ' && !q)
                {
                    if (cur.Length > 0)
                    {
                        list.Add(cur);
                        cur = "";
                    }
                    continue;
                }

                cur += c;
            }

            if (cur.Length > 0)
                list.Add(cur);

            return list.ToArray();
        }

        private string ResolveFsPath(string path)
        {
            if (Path.IsPathRooted(path))
                return path;

            string baseDir = FileSystemManager.CurrentDirectory;

            if (!baseDir.EndsWith("\\"))
                baseDir += "\\";

            return Path.GetFullPath(Path.Combine(baseDir, path));
        }

        private void RunFunc(string name)
        {
            if (!_funcs.TryGetValue(name, out int start))
                return;

            _inFunc = true;

            for (int i = start + 1; i < _lines.Length; i++)
            {
                var l = _lines[i].Trim();

                if (l == "endfunc")
                    break;

                Execute(l);
            }

            _inFunc = false;
        }

        private void SkipBlock()
        {
            int depth = 0;

            for (int i = _ip + 1; i < _lines.Length; i++)
            {
                var l = _lines[i].Trim();

                if (l.StartsWith("if("))
                    depth++;

                if (l == "endif")
                {
                    if (depth == 0)
                    {
                        _ip = i;
                        return;
                    }
                    depth--;
                }
            }

            _ip = _lines.Length;
        }   
    }
}