using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DihotomyMethodApp
{
    public partial class DihotomyMethodForm : Form
    {
        private TextBox txtA, txtB, txtEpsilon;
        private Label lblResult;
        private DataGridView dataGridView;

        public DihotomyMethodForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Метод дихотомии";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Справка");
            ToolStripMenuItem instructionItem = new ToolStripMenuItem("Инструкция");
            instructionItem.Click += ShowInstruction;
            helpMenu.DropDownItems.Add(instructionItem);
            menuStrip.Items.Add(helpMenu);

            var lblA = new Label { Text = "Начало интервала (a):", Location = new Point(20, 20), Width = 150 };
            txtA = new TextBox { Location = new Point(180, 18), Width = 100, Text = "0" };

            var lblB = new Label { Text = "Конец интервала (b):", Location = new Point(20, 50), Width = 150 };
            txtB = new TextBox { Location = new Point(180, 48), Width = 100, Text = "2" };

            var lblEpsilon = new Label { Text = "Точность (ε):", Location = new Point(20, 80), Width = 150 };
            txtEpsilon = new TextBox { Location = new Point(180, 78), Width = 100, Text = "0.001" };

            var btnCalculate = new Button { Text = "Вычислить", Location = new Point(300, 50), Width = 100 };
            btnCalculate.Click += BtnCalculate_Click;

            var btnClear = new Button { Text = "Очистить", Location = new Point(410, 50), Width = 100 };
            btnClear.Click += (s, e) =>
            {
                txtA.Text = "0";
                txtB.Text = "2";
                txtEpsilon.Text = "0.001";
                lblResult.Text = "";
                dataGridView.Rows.Clear();
            };

            var btnHelp = new Button { Text = "❓ Помощь", Location = new Point(520, 50), Width = 100 };
            btnHelp.Click += ShowInstruction;

            dataGridView = new DataGridView();
            dataGridView.Location = new Point(20, 120);
            dataGridView.Size = new Size(740, 350);
            dataGridView.ScrollBars = ScrollBars.Vertical;

            dataGridView.Columns.Add("Iteration", "Итерация");
            dataGridView.Columns.Add("A", "a");
            dataGridView.Columns.Add("B", "b");
            dataGridView.Columns.Add("X1", "x₁");
            dataGridView.Columns.Add("X2", "x₂");
            dataGridView.Columns.Add("F1", "f(x₁)");
            dataGridView.Columns.Add("F2", "f(x₂)");
            dataGridView.Columns.Add("NewInterval", "Новый интервал");

            lblResult = new Label();
            lblResult.Location = new Point(20, 480);
            lblResult.Size = new Size(740, 60);
            lblResult.Font = new Font("Arial", 10, FontStyle.Bold);
            lblResult.ForeColor = Color.Blue;

            var btnBack = new Button { Text = "← Назад в меню", Location = new Point(20, 550), Width = 150 };
            btnBack.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblA, txtA, lblB, txtB, lblEpsilon, txtEpsilon,
                btnCalculate, btnClear, btnHelp, dataGridView, lblResult, btnBack
            });
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
        }

        private void ShowInstruction(object sender, EventArgs e)
        {
            string instruction =
                "📖 МЕТОД ДИХОТОМИИ (ПОЛОВИННОГО ДЕЛЕНИЯ)\n\n" +
                "НАЗНАЧЕНИЕ:\n• Поиск корня уравнения f(x) = 0 на интервале [a, b]\n\n" +
                "УСЛОВИЯ ПРИМЕНИМОСТИ:\n1. Функция f(x) непрерывна на [a, b]\n2. На концах интервала разные знаки: f(a)·f(b) < 0\n\n" +
                "📋 ИНСТРУКЦИЯ:\n1. Введите интервал [a, b] (пример: a=0, b=2)\n2. Укажите требуемую точность ε (пример: 0.001)\n3. Нажмите 'Вычислить'\n\n" +
                "📊 РЕЗУЛЬТАТЫ:\n• Таблица итераций покажет процесс сужения интервала\n• Будет найден корень x*, такой что |b-a| < ε\n• f(x*) будет близко к 0\n\n" +
                "🔧 ПРИМЕР ФУНКЦИИ:\nf(x) = x² - 2 = 0\nКорень: x = √2 ≈ 1.41421356";

            MessageBox.Show(instruction, "Инструкция - Метод дихотомии",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView.Rows.Clear();

                double a = double.Parse(txtA.Text);
                double b = double.Parse(txtB.Text);
                double epsilon = double.Parse(txtEpsilon.Text);

                if (a >= b)
                {
                    MessageBox.Show("a должно быть меньше b", "Ошибка");
                    return;
                }

                if (Function(a) * Function(b) > 0)
                {
                    string warning = "⚠️ Нарушено условие метода дихотомии!\nf(a) * f(b) должно быть < 0\n\n" +
                                   $"f({a}) = {Function(a):F4}\n" +
                                   $"f({b}) = {Function(b):F4}\n\n" +
                                   "Выберите другой интервал, где функция меняет знак.";

                    MessageBox.Show(warning, "Предупреждение",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int iteration = 0;
                List<double> iterations = new List<double>();

                while (Math.Abs(b - a) > epsilon)
                {
                    iteration++;
                    double x1 = (a + b) / 2 - epsilon / 4;
                    double x2 = (a + b) / 2 + epsilon / 4;
                    double f1 = Function(x1);
                    double f2 = Function(x2);

                    dataGridView.Rows.Add(
                        iteration,
                        Math.Round(a, 6),
                        Math.Round(b, 6),
                        Math.Round(x1, 6),
                        Math.Round(x2, 6),
                        Math.Round(f1, 6),
                        Math.Round(f2, 6),
                        f1 < f2 ? "[a, x₂]" : "[x₁, b]"
                    );

                    if (f1 < f2)
                        b = x2;
                    else
                        a = x1;

                    iterations.Add((a + b) / 2);
                }

                double result = (a + b) / 2;
                lblResult.Text = $"✅ РЕЗУЛЬТАТ:\nКорень: x = {Math.Round(result, 6)}\n" +
                               $"f(x) = {Math.Round(Function(result), 6)}\n" +
                               $"Итераций: {iteration}\n" +
                               $"Точность: {Math.Abs(b - a):E2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка расчета");
            }
        }

        private double Function(double x)
        {
            return x * x - 2;
        }
    }
}