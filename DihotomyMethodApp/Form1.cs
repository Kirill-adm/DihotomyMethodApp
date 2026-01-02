using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ZedGraph;

namespace DihotomyMethodApp
{
    public partial class Form1 : Form
    {
        private TextBox txtA, txtB, txtEpsilon, txtFunction;
        private System.Windows.Forms.Label lblResult;  // Явно указываем пространство имен
        private ZedGraphControl zedGraphControl;

        public Form1()
        {
            InitializeComponent();
            InitializeDefaultValues();
            SetupZedGraph();
        }

        private void InitializeComponent()
        {
            // Основные настройки формы
            this.Text = "Поиск минимума функции методом дихотомии";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Меню сверху
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Dock = DockStyle.Top;

            // Кнопки меню
            ToolStripMenuItem calculateItem = new ToolStripMenuItem("Рассчитать");
            calculateItem.Click += Calculate_Click;

            ToolStripMenuItem clearItem = new ToolStripMenuItem("Очистить");
            clearItem.Click += Clear_Click;

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Выход");
            exitItem.Click += (s, e) => Application.Exit();

            menuStrip.Items.AddRange(new ToolStripItem[] { calculateItem, clearItem, exitItem });

            // Панель для ввода параметров (слева)
            Panel inputPanel = new Panel();
            inputPanel.Dock = DockStyle.Left;
            inputPanel.Width = 350;
            inputPanel.BackColor = Color.AliceBlue;
            inputPanel.Padding = new Padding(10);

            // Заголовок
            System.Windows.Forms.Label labelTitle = new System.Windows.Forms.Label()  // Явно указываем
            {
                Text = "ПАРАМЕТРЫ РАСЧЕТА",
                Location = new Point(20, 10),
                Font = new Font("Arial", 12, FontStyle.Bold),
                Width = 300
            };

            // Поля для ввода - ВСЕ Label явно указываем
            System.Windows.Forms.Label labelA = new System.Windows.Forms.Label()
            {
                Text = "Начало интервала (a):",
                Location = new Point(20, 50),
                Width = 150
            };

            System.Windows.Forms.Label labelB = new System.Windows.Forms.Label()
            {
                Text = "Конец интервала (b):",
                Location = new Point(20, 90),
                Width = 150
            };

            System.Windows.Forms.Label labelEpsilon = new System.Windows.Forms.Label()
            {
                Text = "Точность (ε):",
                Location = new Point(20, 130),
                Width = 150
            };

            System.Windows.Forms.Label labelFunction = new System.Windows.Forms.Label()
            {
                Text = "Функция f(x):",
                Location = new Point(20, 170),
                Width = 150
            };

            txtA = new TextBox()
            {
                Location = new Point(180, 47),
                Width = 100,
                Text = "0"
            };

            txtB = new TextBox()
            {
                Location = new Point(180, 87),
                Width = 100,
                Text = "2"
            };

            txtEpsilon = new TextBox()
            {
                Location = new Point(180, 127),
                Width = 100,
                Text = "0.001"
            };

            txtFunction = new TextBox()
            {
                Location = new Point(180, 167),
                Width = 140,
                Text = "x*x"
            };

            // Подсказка по функциям
            System.Windows.Forms.Label labelExamples = new System.Windows.Forms.Label()
            {
                Text = "Допустимые функции:\n" +
                       "• x*x (квадрат)\n" +
                       "• sin(x), cos(x)\n" +
                       "• exp(x), sqrt(x)\n" +
                       "• (x-1)*(x-1)\n" +
                       "• x*x*x - 3*x",
                Location = new Point(20, 220),
                Width = 300,
                Height = 120,
                ForeColor = Color.DarkSlateBlue
            };

            // Результат
            lblResult = new System.Windows.Forms.Label()  // Это поле класса, уже объявлено
            {
                Text = "Результат: введите параметры и нажмите 'Рассчитать'",
                Location = new Point(20, 350),
                Font = new Font("Arial", 10, FontStyle.Bold),
                Width = 300,
                Height = 60
            };

            // Кнопки (дополнительно к меню)
            Button btnCalculate = new Button()
            {
                Text = "Рассчитать",
                Location = new Point(20, 420),
                Size = new Size(100, 30),
                BackColor = Color.LightGreen
            };
            btnCalculate.Click += Calculate_Click;

            Button btnClear = new Button()
            {
                Text = "Очистить",
                Location = new Point(140, 420),
                Size = new Size(100, 30),
                BackColor = Color.LightCoral
            };
            btnClear.Click += Clear_Click;

            // Добавляем элементы на панель ввода
            inputPanel.Controls.AddRange(new Control[]
            {
                labelTitle,
                labelA, txtA,
                labelB, txtB,
                labelEpsilon, txtEpsilon,
                labelFunction, txtFunction,
                labelExamples,
                lblResult,
                btnCalculate, btnClear
            });

            // Создаем ZedGraphControl для графика
            zedGraphControl = new ZedGraphControl();
            zedGraphControl.Dock = DockStyle.Fill;
            zedGraphControl.Name = "zedGraphControl";

            // Добавляем элементы на форму
            this.Controls.AddRange(new Control[]
            {
                menuStrip,
                inputPanel,
                zedGraphControl
            });

            this.MainMenuStrip = menuStrip;
        }

        private void InitializeDefaultValues()
        {
            // Установка значений по умолчанию
            txtA.Text = "0";
            txtB.Text = "2";
            txtEpsilon.Text = "0.001";
            txtFunction.Text = "x*x";
        }

        private void SetupZedGraph()
        {
            // Настройка графика
            GraphPane pane = zedGraphControl.GraphPane;

            // Очищаем предыдущие кривые
            pane.CurveList.Clear();

            // Настройка заголовков
            pane.Title.Text = "График функции и точка минимума";
            pane.XAxis.Title.Text = "x";
            pane.YAxis.Title.Text = "f(x)";

            // Настройка сетки
            pane.XAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.IsVisible = true;
            pane.XAxis.MajorGrid.Color = Color.LightGray;
            pane.YAxis.MajorGrid.Color = Color.LightGray;

            // Обновляем график
            zedGraphControl.AxisChange();
            zedGraphControl.Invalidate();
        }

        private void Calculate_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем входные данные
                if (!ValidateInputs(out double a, out double b, out double epsilon))
                    return;

                string function = txtFunction.Text;

                // Ищем минимум методом дихотомии
                double minX, minY;
                FindMinimum(a, b, epsilon, function, out minX, out minY);

                // Показываем результат
                lblResult.Text = $"Найден минимум:\n" +
                               $"x = {minX:F6}\n" +
                               $"f(x) = {minY:F6}\n" +
                               $"ε = {epsilon}, интервал [{a}, {b}]";

                // Строим график
                PlotFunction(a, b, function, minX, minY);

                // Сообщение об успехе
                MessageBox.Show($"Минимум найден!\nx = {minX:F6}\nf(x) = {minY:F6}",
                    "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}\n\nИспользуйте простые функции:\nx*x, sin(x), cos(x), exp(x)",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs(out double a, out double b, out double epsilon)
        {
            a = 0; b = 0; epsilon = 0;

            // Проверка a
            if (!double.TryParse(txtA.Text.Replace(',', '.'),
                NumberStyles.Float, CultureInfo.InvariantCulture, out a))
            {
                MessageBox.Show("Ошибка в параметре a. Введите число.", "Ошибка ввода");
                txtA.Focus();
                return false;
            }

            // Проверка b
            if (!double.TryParse(txtB.Text.Replace(',', '.'),
                NumberStyles.Float, CultureInfo.InvariantCulture, out b))
            {
                MessageBox.Show("Ошибка в параметре b. Введите число.", "Ошибка ввода");
                txtB.Focus();
                return false;
            }

            if (a >= b)
            {
                MessageBox.Show("a должно быть меньше b", "Ошибка ввода");
                txtA.Focus();
                return false;
            }

            // Проверка epsilon
            if (!double.TryParse(txtEpsilon.Text.Replace(',', '.'),
                NumberStyles.Float, CultureInfo.InvariantCulture, out epsilon))
            {
                MessageBox.Show("Ошибка в точности ε. Введите число.", "Ошибка ввода");
                txtEpsilon.Focus();
                return false;
            }

            if (epsilon <= 0)
            {
                MessageBox.Show("Точность ε должна быть больше 0", "Ошибка ввода");
                txtEpsilon.Focus();
                return false;
            }

            // Проверка функции на границах
            try
            {
                EvaluateFunction(a, txtFunction.Text);
                EvaluateFunction(b, txtFunction.Text);
            }
            catch
            {
                MessageBox.Show("Функция содержит ошибки или не может быть вычислена на заданном интервале.",
                    "Ошибка функции");
                txtFunction.Focus();
                return false;
            }

            return true;
        }

        private void FindMinimum(double a, double b, double epsilon, string function,
                                out double minX, out double minY)
        {
            double left = a;
            double right = b;
            double delta = epsilon / 10;

            // Метод дихотомии
            while (Math.Abs(right - left) > epsilon)
            {
                double x1 = (left + right - delta) / 2;
                double x2 = (left + right + delta) / 2;

                double f1 = EvaluateFunction(x1, function);
                double f2 = EvaluateFunction(x2, function);

                if (f1 < f2)
                    right = x2;
                else
                    left = x1;
            }

            minX = (left + right) / 2;
            minY = EvaluateFunction(minX, function);
        }

        private double EvaluateFunction(double x, string function)
        {
            // Упрощаем функцию для вычисления
            string func = function.ToLower().Replace(" ", "");

            // Простые случаи
            if (func == "x*x" || func == "x^2")
                return x * x;
            else if (func == "x*x*x" || func == "x^3")
                return x * x * x;
            else if (func == "sin(x)")
                return Math.Sin(x);
            else if (func == "cos(x)")
                return Math.Cos(x);
            else if (func == "exp(x)")
                return Math.Exp(x);
            else if (func == "sqrt(x)")
                return Math.Sqrt(x);
            else if (func == "abs(x)")
                return Math.Abs(x);
            else if (func == "(x-1)*(x-1)" || func == "x*x-2*x+1")
                return (x - 1) * (x - 1);
            else if (func == "x*x*x-3*x")
                return x * x * x - 3 * x;

            // Пытаемся вычислить сложное выражение
            try
            {
                // Заменяем x на значение
                string expression = func.Replace("x", x.ToString(CultureInfo.InvariantCulture));

                // Заменяем операторы
                expression = expression.Replace("^", "**");

                // Вычисляем
                DataTable table = new DataTable();
                object result = table.Compute(expression, "");
                return Convert.ToDouble(result);
            }
            catch
            {
                throw new Exception($"Не могу вычислить функцию: {function}");
            }
        }

        private void PlotFunction(double a, double b, string function, double minX, double minY)
        {
            // Получаем панель графика
            GraphPane pane = zedGraphControl.GraphPane;

            // Очищаем предыдущие кривые
            pane.CurveList.Clear();

            // Создаем списки точек для графика функции
            PointPairList functionPoints = new PointPairList();
            int pointsCount = 100;
            double step = (b - a) / pointsCount;

            for (int i = 0; i <= pointsCount; i++)
            {
                double x = a + i * step;
                try
                {
                    double y = EvaluateFunction(x, function);
                    functionPoints.Add(x, y);
                }
                catch
                {
                    // Пропускаем точки, где функция не определена
                }
            }

            // Добавляем кривую функции
            LineItem functionCurve = pane.AddCurve($"f(x) = {function}",
                functionPoints, Color.Blue, SymbolType.None);
            functionCurve.Line.Width = 2.0f;

            // Добавляем точку минимума
            PointPairList minPoint = new PointPairList();
            minPoint.Add(minX, minY);

            LineItem minPointCurve = pane.AddCurve("Точка минимума",
                minPoint, Color.Red, SymbolType.Circle);
            minPointCurve.Line.IsVisible = false;
            minPointCurve.Symbol.Fill = new Fill(Color.Red);
            minPointCurve.Symbol.Size = 10;

            // Добавляем вертикальную линию в точку минимума
            PointPairList verticalLine = new PointPairList();
            double minYValue = EvaluateFunction(a, function);
            double maxYValue = EvaluateFunction(b, function);
            for (double x = a; x <= b; x += step)
            {
                double y = EvaluateFunction(x, function);
                if (y < minYValue) minYValue = y;
                if (y > maxYValue) maxYValue = y;
            }

            // Линия для точки минимума
            PointPairList minLine = new PointPairList();
            minLine.Add(minX, minYValue - Math.Abs(maxYValue - minYValue) * 0.1);
            minLine.Add(minX, minY);

            LineItem minLineCurve = pane.AddCurve("", minLine, Color.Red, SymbolType.None);
            minLineCurve.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            minLineCurve.Line.Width = 1.0f;

            // Настраиваем оси
            pane.XAxis.Scale.Min = a;
            pane.XAxis.Scale.Max = b;

            // Добавляем легенду
            pane.Legend.IsVisible = true;
            pane.Legend.Position = LegendPos.TopCenter;

            // Обновляем график
            zedGraphControl.AxisChange();
            zedGraphControl.Invalidate();
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            // Сброс полей ввода
            txtA.Text = "0";
            txtB.Text = "2";
            txtEpsilon.Text = "0.001";
            txtFunction.Text = "x*x";

            // Сброс результата
            lblResult.Text = "Результат: введите параметры и нажмите 'Рассчитать'";

            // Очистка графика
            GraphPane pane = zedGraphControl.GraphPane;
            pane.CurveList.Clear();
            pane.Title.Text = "График функции и точка минимума";
            zedGraphControl.AxisChange();
            zedGraphControl.Invalidate();

            // Фокус на первое поле
            txtA.Focus();
        }
    }
}