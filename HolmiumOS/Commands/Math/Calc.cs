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

                    for (int i = 0; i < expr.Length; i++)
                    {
                        char c = expr[i];

                        // Negatif sayilar
                        if (c == '-' && (i == 0 || "+-*/^(".Contains(expr[i - 1])))
                        {
                            i++;
                            int start = i;
                            while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.')) i++;
                            values.Push(-double.Parse(expr[start..i]));
                            i--;
                        }
                        else if (char.IsDigit(c) || c == '.')
                        {
                            int start = i;
                            while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.')) i++;
                            values.Push(double.Parse(expr[start..i]));
                            i--;
                        }
                        else if (c == '(')
                        {
                            ops.Push(c);
                        }
                        else if (c == ')')
                        {
                            while (ops.Peek() != '(')
                                values.Push(Apply(values.Pop(), values.Pop(), ops.Pop()));
                            ops.Pop();
                        }
                        else
                        {
                            while (ops.Count > 0 && Precedence(ops.Peek()) >= Precedence(c))
                                values.Push(Apply(values.Pop(), values.Pop(), ops.Pop()));
                            ops.Push(c);
                        }
                    }

                    while (ops.Count > 0)
                        values.Push(Apply(values.Pop(), values.Pop(), ops.Pop()));

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