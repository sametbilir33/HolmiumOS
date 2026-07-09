using System;
using System.Collections.Generic;
using HolmiumOS.Commands;
using SysMath = System.Math;

namespace HolmiumOS.Commands.Math
{
    public class Calc : ICommand
    {
        public string Name => "calc";
        public string Description => "Matematiksel ifadeyi hesaplar";
        public string Usage => "calc <ifade>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Kullanim: calc <ifade>");
                Console.ResetColor();
                return;
            }

            try
            {
                double Evaluate(string expr)
                {
                    expr = expr.Replace(" ", "");

                    Stack<double> values = new();
                    Stack<char> ops = new();

                    int Precedence(char op) => op switch
                    {
                        '+' or '-' => 1,
                        '*' or '/' => 2,
                        '^' => 3,
                        _ => 0
                    };

                    double Apply(double b, double a, char op) => op switch
                    {
                        '+' => a + b,
                        '-' => a - b,
                        '*' => a * b,
                        '/' => b == 0 ? throw new Exception("Sifira bolme") : a / b,
                        '^' => SysMath.Pow(a, b),
                        _ => 0
                    };

                    void ApplyTop()
                    {
                        if (values.Count < 2 || ops.Count == 0)
                            throw new Exception("Gecersiz ifade.");

                        values.Push(Apply(values.Pop(), values.Pop(), ops.Pop()));
                    }

                    for (int i = 0; i < expr.Length; i++)
                    {
                        char c = expr[i];

                        // Unary + / - (basta, operatorden sonra veya acilan parantezden sonra)
                        if ((c == '-' || c == '+') && (i == 0 || "+-*/^(".Contains(expr[i - 1])))
                        {
                            bool negative = c == '-';
                            i++;

                            if (i >= expr.Length || !(char.IsDigit(expr[i]) || expr[i] == '.'))
                                throw new Exception("Gecersiz ifade.");

                            int start = i;

                            while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
                                i++;

                            double num = double.Parse(expr[start..i]);
                            values.Push(negative ? -num : num);
                            i--;
                        }
                        else if (char.IsDigit(c) || c == '.')
                        {
                            int start = i;

                            while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
                                i++;

                            values.Push(double.Parse(expr[start..i]));
                            i--;
                        }
                        else if (c == '(')
                        {
                            ops.Push(c);
                        }
                        else if (c == ')')
                        {
                            while (ops.Count > 0 && ops.Peek() != '(')
                                ApplyTop();

                            if (ops.Count == 0)
                                throw new Exception("Parantezler eslesmiyor.");

                            ops.Pop(); // '(' at
                        }
                        else if ("+-*/^".Contains(c))
                        {
                            // '^' sagdan sola, digerleri soldan saga
                            while (ops.Count > 0 && ops.Peek() != '(' &&
                                  (Precedence(ops.Peek()) > Precedence(c) ||
                                  (Precedence(ops.Peek()) == Precedence(c) && c != '^')))
                            {
                                ApplyTop();
                            }

                            ops.Push(c);
                        }
                        else
                        {
                            throw new Exception($"Gecersiz karakter: '{c}'");
                        }
                    }

                    while (ops.Count > 0)
                    {
                        if (ops.Peek() == '(')
                            throw new Exception("Parantezler eslesmiyor.");

                        ApplyTop();
                    }

                    if (values.Count != 1)
                        throw new Exception("Gecersiz ifade.");

                    return values.Pop();
                }

                double result = Evaluate(args);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Sonuc: {result}");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Hata: {e.Message}");
            }

            Console.ResetColor();
        }
    }
}