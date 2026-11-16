
using System;
using System.Collections.Generic;
using HolmiumOS.GUI.Controls;
using SysMath = System.Math;

namespace HolmiumOS.GUI.Apps
{
    public class Calculator : AppBase
    {
        private TextBox? displayTextBox;
        private string currentExpression = "";

        public Calculator() : base("Hesap Makinesi")
        {
        }

        public override void Load()
        {
            if (Window == null)
                return;

            Window.Title = "Hesap Makinesi";

            displayTextBox = new TextBox(15, 15, 300, 42);
            displayTextBox.Text = "0";
            displayTextBox.MaxLength = 25;
            Window.AddControl(displayTextBox);

            string[,] keys =
            {
                { "7", "8", "9", "/" },
                { "4", "5", "6", "*" },
                { "1", "2", "3", "-" },
                { "C", "0", "=", "+" }
            };

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    string key = keys[row, col];
                    int x = 15 + col * 75;
                    int y = 75 + row * 65;
                    int width = col == 3 ? 75 : 65;

                    AddButton(key, x, y, width, 55);
                }
            }
        }

        private void AddButton(string text, int x, int y, int width, int height)
        {
            Button button = new Button(text, x, y, width, height);
            button.ClickAction = () => ProcessInput(text);
            Window?.AddControl(button);
        }

        private void ProcessInput(string val)
        {
            if (displayTextBox == null)
                return;

            if (val == "C")
            {
                currentExpression = "";
                displayTextBox.Text = "0";
                return;
            }

            if (val == "=")
            {
                if (currentExpression.Length == 0)
                    return;

                try
                {
                    double result = Evaluate(currentExpression);
                    string resultStr = result.ToString();

                    displayTextBox.Text = resultStr;
                    currentExpression = resultStr;
                }
                catch
                {
                    displayTextBox.Text = "Hata";
                    currentExpression = "";
                }

                return;
            }

            if (currentExpression == "0" && val != "+" && val != "-" && val != "*" && val != "/")
                currentExpression = val;
            else
                currentExpression += val;

            displayTextBox.Text = currentExpression;
        }

        private double Evaluate(string expr)
        {
            expr = expr.Replace(" ", "");

            Stack<double> values = new Stack<double>();
            Stack<char> ops = new Stack<char>();

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

                if ((c == '-' || c == '+') && (i == 0 || "+-*/^(".Contains(expr[i - 1].ToString())))
                {
                    bool negative = c == '-';
                    i++;

                    if (i >= expr.Length || !(char.IsDigit(expr[i]) || expr[i] == '.'))
                        throw new Exception("Gecersiz ifade.");

                    int start = i;

                    while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
                        i++;

                    double num = double.Parse(expr.Substring(start, i - start));
                    values.Push(negative ? -num : num);
                    i--;
                }
                else if (char.IsDigit(c) || c == '.')
                {
                    int start = i;

                    while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
                        i++;

                    values.Push(double.Parse(expr.Substring(start, i - start)));
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

                    ops.Pop();
                }
                else if ("+-*/^".Contains(c.ToString()))
                {
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

        public override void Close()
        {
        }
    }
}
