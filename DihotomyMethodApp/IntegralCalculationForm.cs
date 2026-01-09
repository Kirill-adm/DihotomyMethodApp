using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace DihotomyMethodApp
{
    public partial class IntegralCalculationForm : Form
    {
        private TextBox txtFunction, txtA, txtB, txtEpsilon;
        private CheckBox chkRectangles, chkTrapezoids, chkSimpson;
        private Panel graphPanel;
        private DataGridView dgvResults;
        private ListBox lstHistory;
        private MenuStrip menuStrip;
        private List<PointF> functionPoints = new List<PointF>();
        private List<List<PointF>> partitionHistory = new List<List<PointF>>();
        private float graphScale = 50;
        private PointF graphOrigin;

        public IntegralCalculationForm()
        {
            InitializeComponent();
            InitializeGraph();
        }

        private void InitializeComponent()
        {
            this.Text = "Вычисление определенного интеграла";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            this.menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Файл");
            var calculateItem = new ToolStripMenuItem("Рассчитать");
            var clearItem = new ToolStripMenuItem("Очистить");
            var exitItem = new ToolStripMenuItem("Выход");
            var helpMenu = new ToolStripMenuItem("Справка");
            var instructionItem = new ToolStripMenuItem("Инструкция");
            instructionItem.Click += ShowIntegralInstruction;

            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { calculateItem, clearItem, new ToolStripSeparator(), exitItem });
            helpMenu.DropDownItems.Add(instructionItem);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, helpMenu });

            this.graphPanel = new Panel();
            this.graphPanel.Location = new Point(400, 50);
            this.graphPanel.Size = new Size(750, 600);
            this.graphPanel.BackColor = Color.White;
            this.graphPanel.BorderStyle = BorderStyle.FixedSingle;
            this.graphPanel.Paint += GraphPanel_Paint;

            var leftPanel = new Panel();
            leftPanel.Location = new Point(20, 50);
            leftPanel.Size = new Size(350, 600);
            leftPanel.BorderStyle = BorderStyle.FixedSingle;

            var lblFunction = new Label { Text = "Функция f(x):", Location = new Point(10, 20), Width = 150 };
            this.txtFunction = new TextBox { Location = new Point(160, 18), Width = 170, Text = "Math.Sin(x) + Math.Cos(x)" };

            var lblA = new Label { Text = "Нижний предел (a):", Location = new Point(10, 60), Width = 150 };
            this.txtA = new TextBox { Location = new Point(160, 58), Width = 80, Text = "0" };

            var lblB = new Label { Text = "Верхний предел (b):", Location = new Point(10, 100), Width = 150 };
            this.txtB = new TextBox { Location = new Point(160, 98), Width = 80, Text = "Math.PI" };

            var lblEpsilon = new Label { Text = "Точность (ε):", Location = new Point(10, 140), Width = 150 };
            this.txtEpsilon = new TextBox { Location = new Point(160, 138), Width = 80, Text = "0.001" };

            var lblMethods = new Label { Text = "Методы интегрирования:", Location = new Point(10, 180), Width = 200, Font = new Font("Arial", 10, FontStyle.Bold) };
            this.chkRectangles = new CheckBox { Text = "Метод прямоугольников", Location = new Point(20, 210), Width = 200, Checked = true };
            this.chkTrapezoids = new CheckBox { Text = "Метод трапеций", Location = new Point(20, 240), Width = 200, Checked = true };
            this.chkSimpson = new CheckBox { Text = "Метод парабол (Симпсона)", Location = new Point(20, 270), Width = 200, Checked = true };

            var lblResults = new Label { Text = "Результаты:", Location = new Point(10, 310), Width = 150, Font = new Font("Arial", 10, FontStyle.Bold) };
            this.dgvResults = new DataGridView();
            this.dgvResults.Location = new Point(10, 340);
            this.dgvResults.Size = new Size(320, 150);
            this.dgvResults.ReadOnly = true;
            this.dgvResults.Columns.Add("Method", "Метод");
            this.dgvResults.Columns.Add("Result", "Значение интеграла");
            this.dgvResults.Columns.Add("Partitions", "Разбиения");
            this.dgvResults.Columns.Add("Error", "Погрешность");

            var lblHistory = new Label { Text = "История разбиений:", Location = new Point(10, 500), Width = 150, Font = new Font("Arial", 10, FontStyle.Bold) };
            this.lstHistory = new ListBox();
            this.lstHistory.Location = new Point(10, 530);
            this.lstHistory.Size = new Size(320, 60);
            this.lstHistory.SelectedIndexChanged += LstHistory_SelectedIndexChanged;

            leftPanel.Controls.AddRange(new Control[] {
                lblFunction, txtFunction, lblA, txtA, lblB, txtB, lblEpsilon, txtEpsilon,
                lblMethods, chkRectangles, chkTrapezoids, chkSimpson,
                lblResults, dgvResults, lblHistory, lstHistory
            });

            var btnBack = new Button();
            btnBack.Text = "← Назад в меню";
            btnBack.Location = new Point(20, 10);
            btnBack.Size = new Size(120, 30);
            btnBack.BackColor = Color.LightGray;
            btnBack.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { leftPanel, graphPanel, btnBack, menuStrip });
            this.MainMenuStrip = menuStrip;

            calculateItem.Click += CalculateItem_Click;
            clearItem.Click += ClearItem_Click;
            exitItem.Click += (s, e) => this.Close();
        }

        private void InitializeGraph() => graphOrigin = new PointF(graphPanel.Width / 2, graphPanel.Height / 2);

        private void GraphPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            DrawCoordinateAxes(g);
            DrawFunction(g);
            DrawPartitions(g);
        }

        private void DrawCoordinateAxes(Graphics g)
        {
            Pen axisPen = new Pen(Color.Black, 2);
            Font axisFont = new Font("Arial", 10);
            g.DrawLine(axisPen, 0, graphOrigin.Y, graphPanel.Width, graphOrigin.Y);
            g.DrawLine(axisPen, graphOrigin.X, 0, graphOrigin.X, graphPanel.Height);
            g.DrawLine(axisPen, graphPanel.Width - 10, graphOrigin.Y - 5, graphPanel.Width, graphOrigin.Y);
            g.DrawLine(axisPen, graphPanel.Width - 10, graphOrigin.Y + 5, graphPanel.Width, graphOrigin.Y);
            g.DrawLine(axisPen, graphOrigin.X - 5, 10, graphOrigin.X, 0);
            g.DrawLine(axisPen, graphOrigin.X + 5, 10, graphOrigin.X, 0);
            g.DrawString("X", axisFont, Brushes.Black, graphPanel.Width - 20, graphOrigin.Y + 10);
            g.DrawString("Y", axisFont, Brushes.Black, graphOrigin.X + 10, 0);

            for (int i = -10; i <= 10; i++)
            {
                if (i == 0) continue;
                float x = graphOrigin.X + i * graphScale;
                g.DrawLine(Pens.Black, x, graphOrigin.Y - 5, x, graphOrigin.Y + 5);
                g.DrawString(i.ToString(), axisFont, Brushes.Black, x - 10, graphOrigin.Y + 10);
                float y = graphOrigin.Y - i * graphScale;
                g.DrawLine(Pens.Black, graphOrigin.X - 5, y, graphOrigin.X + 5, y);
                g.DrawString(i.ToString(), axisFont, Brushes.Black, graphOrigin.X + 10, y - 10);
            }
        }

        private void DrawFunction(Graphics g)
        {
            if (functionPoints.Count < 2) return;
            Pen functionPen = new Pen(Color.Blue, 2);
            PointF[] screenPoints = functionPoints.Select(p => new PointF(graphOrigin.X + p.X * graphScale, graphOrigin.Y - p.Y * graphScale)).ToArray();
            for (int i = 0; i < screenPoints.Length - 1; i++)
                g.DrawLine(functionPen, screenPoints[i], screenPoints[i + 1]);
            Font font = new Font("Arial", 12, FontStyle.Bold);
            g.DrawString("f(x) = " + txtFunction.Text, font, Brushes.Blue, 10, 10);
        }

        private void DrawPartitions(Graphics g)
        {
            if (partitionHistory.Count == 0 || lstHistory.SelectedIndex < 0) return;
            int selectedIndex = lstHistory.SelectedIndex;
            if (selectedIndex >= partitionHistory.Count) return;

            List<PointF> partitions = partitionHistory[selectedIndex];
            Pen partitionPen = new Pen(Color.Red, 1);
            Brush fillBrush = new SolidBrush(Color.FromArgb(50, 255, 0, 0));
            string method = lstHistory.Items[selectedIndex].ToString();

            for (int i = 0; i < partitions.Count - 1; i++)
            {
                float x1 = graphOrigin.X + partitions[i].X * graphScale;
                float x2 = graphOrigin.X + partitions[i + 1].X * graphScale;
                float y1 = graphOrigin.Y - partitions[i].Y * graphScale;
                float y2 = graphOrigin.Y - partitions[i + 1].Y * graphScale;

                if (method.Contains("прямоугольник"))
                {
                    float midX = (x1 + x2) / 2;
                    float midY = graphOrigin.Y - functionPoints.Where(p => Math.Abs(p.X - ((partitions[i].X + partitions[i + 1].X) / 2)) < 0.01).Select(p => p.Y).FirstOrDefault() * graphScale;
                    g.FillRectangle(fillBrush, x1, midY, x2 - x1, graphOrigin.Y - midY);
                    g.DrawRectangle(partitionPen, x1, midY, x2 - x1, graphOrigin.Y - midY);
                }
                else if (method.Contains("трапец") || method.Contains("парабол"))
                {
                    PointF[] points = new PointF[4];
                    points[0] = new PointF(x1, graphOrigin.Y); points[1] = new PointF(x1, y1);
                    points[2] = new PointF(x2, y2); points[3] = new PointF(x2, graphOrigin.Y);
                    g.FillPolygon(fillBrush, points);
                    g.DrawPolygon(partitionPen, points);
                }
            }
        }

        private void CalculateItem_Click(object sender, EventArgs e)
        {
            try
            {
                dgvResults.Rows.Clear(); lstHistory.Items.Clear(); functionPoints.Clear(); partitionHistory.Clear();
                string functionStr = txtFunction.Text;
                double a = ParseMathExpression(txtA.Text);
                double b = ParseMathExpression(txtB.Text);
                double epsilon = double.Parse(txtEpsilon.Text);
                if (a >= b) { MessageBox.Show("a должно быть меньше b", "Ошибка"); return; }
                if (epsilon <= 0) { MessageBox.Show("Точность должна быть положительным числом", "Ошибка"); return; }

                GenerateFunctionPoints(functionStr, a, b);
                if (chkRectangles.Checked) CalculateRectangleMethod(functionStr, a, b, epsilon);
                if (chkTrapezoids.Checked) CalculateTrapezoidMethod(functionStr, a, b, epsilon);
                if (chkSimpson.Checked) CalculateSimpsonMethod(functionStr, a, b, epsilon);
                graphPanel.Invalidate();
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка расчета"); }
        }

        private double ParseMathExpression(string expression)
        {
            expression = expression.Replace("Math.PI", Math.PI.ToString()).Replace("Math.E", Math.E.ToString()).Replace("pi", Math.PI.ToString()).Replace("e", Math.E.ToString());
            if (double.TryParse(expression, out double result)) return result;
            try
            {
                var dataTable = new System.Data.DataTable();
                var value = dataTable.Compute(expression, "");
                return Convert.ToDouble(value);
            }
            catch { throw new ArgumentException($"Некорректное выражение: {expression}"); }
        }

        private double EvaluateFunction(string functionStr, double x)
        {
            try
            {
                string expression = functionStr.Replace("x", x.ToString()).Replace("Math.PI", Math.PI.ToString()).Replace("Math.E", Math.E.ToString());
                var dataTable = new System.Data.DataTable();
                var value = dataTable.Compute(expression, "");
                return Convert.ToDouble(value);
            }
            catch (Exception ex) { throw new ArgumentException($"Ошибка вычисления функции в точке x={x}: {ex.Message}"); }
        }

        private void GenerateFunctionPoints(string functionStr, double a, double b)
        {
            functionPoints.Clear();
            int pointsCount = 1000; double step = (b - a) / pointsCount;
            for (int i = 0; i <= pointsCount; i++)
            {
                double x = a + i * step;
                double y = EvaluateFunction(functionStr, x);
                functionPoints.Add(new PointF((float)x, (float)y));
            }
        }

        private void CalculateRectangleMethod(string functionStr, double a, double b, double epsilon)
        {
            int n = 4; double prevResult = 0, result = 0; int iteration = 0;
            do
            {
                iteration++; prevResult = result; result = 0; double h = (b - a) / n;
                List<PointF> partitions = new List<PointF>();
                for (int i = 0; i < n; i++)
                {
                    double x_i = a + i * h; double x_next = a + (i + 1) * h; double x_mid = (x_i + x_next) / 2;
                    double f_mid = EvaluateFunction(functionStr, x_mid); result += f_mid * h;
                    partitions.Add(new PointF((float)x_i, (float)EvaluateFunction(functionStr, x_i)));
                    if (i == n - 1) partitions.Add(new PointF((float)x_next, (float)EvaluateFunction(functionStr, x_next)));
                }
                partitionHistory.Add(partitions); lstHistory.Items.Add($"Прямоугольники: n={n}, S={result:F6}"); n *= 2;
            } while (Math.Abs(result - prevResult) > epsilon && iteration < 20);
            dgvResults.Rows.Add("Метод прямоугольников", Math.Round(result, 6), n / 2, Math.Round(Math.Abs(result - prevResult), 8));
        }

        private void CalculateTrapezoidMethod(string functionStr, double a, double b, double epsilon)
        {
            int n = 4; double prevResult = 0, result = 0; int iteration = 0;
            do
            {
                iteration++; prevResult = result; result = 0; double h = (b - a) / n;
                List<PointF> partitions = new List<PointF>();
                for (int i = 0; i < n; i++)
                {
                    double x_i = a + i * h; double x_next = a + (i + 1) * h;
                    double f_i = EvaluateFunction(functionStr, x_i); double f_next = EvaluateFunction(functionStr, x_next);
                    result += (f_i + f_next) * h / 2;
                    partitions.Add(new PointF((float)x_i, (float)f_i));
                    if (i == n - 1) partitions.Add(new PointF((float)x_next, (float)f_next));
                }
                partitionHistory.Add(partitions); lstHistory.Items.Add($"Трапеции: n={n}, S={result:F6}"); n *= 2;
            } while (Math.Abs(result - prevResult) > epsilon && iteration < 20);
            dgvResults.Rows.Add("Метод трапеций", Math.Round(result, 6), n / 2, Math.Round(Math.Abs(result - prevResult), 8));
        }

        private void CalculateSimpsonMethod(string functionStr, double a, double b, double epsilon)
        {
            int n = 4; double prevResult = 0, result = 0; int iteration = 0;
            do
            {
                iteration++; prevResult = result; result = 0; double h = (b - a) / n;
                List<PointF> partitions = new List<PointF>();
                result += EvaluateFunction(functionStr, a); result += EvaluateFunction(functionStr, b);
                for (int i = 1; i < n; i++)
                {
                    double x_i = a + i * h; double f_i = EvaluateFunction(functionStr, x_i);
                    if (i % 2 == 0) result += 2 * f_i; else result += 4 * f_i;
                    partitions.Add(new PointF((float)x_i, (float)f_i));
                }
                result *= h / 3; partitionHistory.Add(partitions); lstHistory.Items.Add($"Симпсон: n={n}, S={result:F6}"); n += 2;
            } while (Math.Abs(result - prevResult) > epsilon && iteration < 20);
            dgvResults.Rows.Add("Метод Симпсона", Math.Round(result, 6), n - 2, Math.Round(Math.Abs(result - prevResult), 8));
        }

        private void ClearItem_Click(object sender, EventArgs e)
        {
            txtFunction.Text = "Math.Sin(x) + Math.Cos(x)"; txtA.Text = "0"; txtB.Text = "Math.PI"; txtEpsilon.Text = "0.001";
            chkRectangles.Checked = chkTrapezoids.Checked = chkSimpson.Checked = true;
            dgvResults.Rows.Clear(); lstHistory.Items.Clear(); functionPoints.Clear(); partitionHistory.Clear(); graphPanel.Invalidate();
        }

        private void LstHistory_SelectedIndexChanged(object sender, EventArgs e) => graphPanel.Invalidate();

        private void ShowIntegralInstruction(object sender, EventArgs e)
        {
            string instruction = "∫ ВЫЧИСЛЕНИЕ ОПРЕДЕЛЕННОГО ИНТЕГРАЛА\n\nЦЕЛЬ: Вычисление ∫ₐᵇ f(x) dx\n\n" +
                               "МЕТОДЫ:\n1. ▭ МЕТОД ПРЯМОУГОЛЬНИКОВ - аппроксимация прямоугольниками\n2. ▱ МЕТОД ТРАПЕЦИЙ - аппроксимация трапециями\n" +
                               "3. ⌒ МЕТОД СИМПСОНА - аппроксимация параболами (требует четного n)\n\n" +
                               "📋 ИНСТРУКЦИЯ:\n1. Введите функцию f(x) (используйте Math.Sin, Math.Cos, Math.Exp, Math.Log, x*x и т.д.)\n" +
                               "2. Укажите пределы интегрирования a и b\n3. Задайте точность ε\n4. Выберите методы галочками\n5. Нажмите 'Рассчитать'\n\n" +
                               "📊 РЕЗУЛЬТАТЫ:\n• Таблица со значениями интеграла\n• История разбиений (кликните для визуализации)\n• График функции\n\n" +
                               "🔧 ПРИМЕРЫ ФУНКЦИЙ:\n• Math.Sin(x) + Math.Cos(x)\n• x*x + 2*x + 1\n• Math.Exp(-x*x)\n• 1/(1 + x*x)";

            MessageBox.Show(instruction, "Инструкция - Вычисление интеграла", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}