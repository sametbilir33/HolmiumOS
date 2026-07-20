using System;
using System.Collections.Generic;
using HolmiumOS.GUI.Controls;
using SysMath = System.Math;

namespace HolmiumOS.GUI.Apps
{
    public class Calculator : AppBase
    {
        private TextBox displayTextBox;
        private string currentExpression = "";

        public Calculator() : base("Calculator")
        {
        }

        public override void Load()
        {
            if (this.Window == null) return;

            this.Window.Title = "Calculator";

            displayTextBox = new TextBox(15, 15, 210, 30);
            displayTextBox.Text = "0";
            displayTextBox.MaxLength = 25;
            this.Window.AddControl(displayTextBox);

            Button btn7 = new Button("7", 15, 65, 45, 45); btn7.ClickAction = OnClick7; this.Window.AddControl(btn7);
            Button btn8 = new Button("8", 70, 65, 45, 45); btn8.ClickAction = OnClick8; this.Window.AddControl(btn8);
            Button btn9 = new Button("9", 125, 65, 45, 45); btn9.ClickAction = OnClick9; this.Window.AddControl(btn9);
            Button btnDiv = new Button("/", 180, 65, 45, 45); btnDiv.ClickAction = OnClickDiv; this.Window.AddControl(btnDiv);

            Button btn4 = new Button("4", 15, 120, 45, 45); btn4.ClickAction = OnClick4; this.Window.AddControl(btn4);
            Button btn5 = new Button("5", 70, 120, 45, 45); btn5.ClickAction = OnClick5; this.Window.AddControl(btn5);
            Button btn6 = new Button("6", 125, 120, 45, 45); btn6.ClickAction = OnClick6; this.Window.AddControl(btn6);
            Button btnMul = new Button("*", 180, 120, 45, 45); btnMul.ClickAction = OnClickMul; this.Window.AddControl(btnMul);

            Button btn1 = new Button("1", 15, 175, 45, 45); btn1.ClickAction = OnClick1; this.Window.AddControl(btn1);
            Button btn2 = new Button("2", 70, 175, 45, 45); btn2.ClickAction = OnClick2; this.Window.AddControl(btn2);
            Button btn3 = new Button("3", 125, 175, 45, 45); btn3.ClickAction = OnClick3; this.Window.AddControl(btn3);
            Button btnSub = new Button("-", 180, 175, 45, 45); btnSub.ClickAction = OnClickSub; this.Window.AddControl(btnSub);

            Button btnC = new Button("C", 15, 230, 45, 45); btnC.ClickAction = OnClickC; this.Window.AddControl(btnC);
            Button btn0 = new Button("0", 70, 230, 45, 45); btn0.ClickAction = OnClick0; this.Window.AddControl(btn0);
            Button btnEqual = new Button("=", 125, 230, 45, 45); btnEqual.ClickAction = OnClickEqual; this.Window.AddControl(btnEqual);
            Button btnAdd = new Button("+", 180, 230, 45, 45); btnAdd.ClickAction = OnClickAdd; this.Window.AddControl(btnAdd);
        }

        private void OnClick7() { ProcessInput("7"); }
        private void OnClick8() { ProcessInput("8"); }
        private void OnClick9() { ProcessInput("9"); }
        private void OnClickDiv() { ProcessInput("/"); }
        private void OnClick4() { ProcessInput("4"); }
        private void OnClick5() { ProcessInput("5"); }
        private void OnClick6() { ProcessInput("6"); }
        private void OnClickMul() { ProcessInput("*"); }
        private void OnClick1() { ProcessInput("1"); }
        private void OnClick2() { ProcessInput("2"); }
        private void OnClick3() { ProcessInput("3"); }
        private void OnClickSub() { ProcessInput("-"); }
        private void OnClickC() { ProcessInput("C"); }
        private void OnClick0() { ProcessInput("0"); }
        private void OnClickEqual() { ProcessInput("="); }
        private void OnClickAdd() { ProcessInput("+"); }

        private void ProcessInput(string val)
        {
            if (displayTextBox == null) return;

            if (val == "C")
            {
                currentExpression = "";
                displayTextBox.Text = "0";
                return;
            }

            if (val == "=")
            {
                if (currentExpression.Length == 0) return;

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
            {
                currentExpression = val;
            }
            else
            {
                currentExpression += val;
            }

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
    }
}