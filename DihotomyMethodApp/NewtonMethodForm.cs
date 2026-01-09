using System;
using System.Drawing;
using System.Windows.Forms;

namespace DihotomyMethodApp
{
    public partial class NewtonMethodForm : Form
    {
        private TextBox txtX0, txtEpsilon;
        private Label lblResult;
        private DataGridView dataGridView;

        public NewtonMethodForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Метод Ньютона";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // Поля ввода
            var lblX0 = new Label { Text = "Начальное приближение (x₀):", Location = new Point(20, 20), Width = 200 };
            txtX0 = new TextBox { Location = new Point(230, 18), Width = 100, Text = "1" };

            var lblEpsilon = new Label { Text = "Точность (ε):", Location = new Point(20, 50), Width = 200 };
            txtEpsilon = new TextBox { Location = new Point(230, 48), Width = 100, Text = "0.001" };

            // Кнопки
            var btnCalculate = new Button { Text = "Вычислить", Location = new Point(350, 35), Width = 100 };
            btnCalculate.Click += BtnCalculate_Click;

            var btnClear = new Button { Text = "Очистить", Location = new Point(460, 35), Width = 100 };
            btnClear.Click += (s, e) =>
            {
                txtX0.Text = "1";
                txtEpsilon.Text = "0.001";
                lblResult.Text = "";
                dataGridView.Rows.Clear();
            };

            // DataGridView для отображения итераций
            dataGridView = new DataGridView();
            dataGridView.Location = new Point(20, 90);
            dataGridView.Size = new Size(740, 350);
            dataGridView.ScrollBars = ScrollBars.Vertical;

            // Настройка колонок
            dataGridView.Columns.Add("Iteration", "Итерация");
            dataGridView.Columns.Add("X", "x");
            dataGridView.Columns.Add("F", "f(x)");
            dataGridView.Columns.Add("FDerivative", "f'(x)");
            dataGridView.Columns.Add("NextX", "xₙ₊₁");
            dataGridView.Columns.Add("Difference", "|xₙ₊₁ - xₙ|");

            // Метка для результата
            lblResult = new Label();
            lblResult.Location = new Point(20, 450);
            lblResult.Size = new Size(740, 60);
            lblResult.Font = new Font("Arial", 10, FontStyle.Bold);
            lblResult.ForeColor = Color.Blue;

            // Кнопка возврата
            var btnBack = new Button { Text = "← Назад в меню", Location = new Point(20, 520), Width = 150 };
            btnBack.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblX0, txtX0, lblEpsilon, txtEpsilon,
                btnCalculate, btnClear, dataGridView, lblResult, btnBack
            });
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView.Rows.Clear();

                double x0 = double.Parse(txtX0.Text);
                double epsilon = double.Parse(txtEpsilon.Text);

                double x = x0;
                double xPrev;
                int iteration = 0;

                do
                {
                    iteration++;
                    xPrev = x;

                    double f = Function(x);
                    double fDerivative = FunctionDerivative(x);

                    if (Math.Abs(fDerivative) < 1e-10)
                    {
                        MessageBox.Show("Производная близка к нулю. Метод Ньютона не применим.", "Ошибка");
                        return;
                    }

                    x = x - f / fDerivative;

                    // Добавляем строку в таблицу
                    dataGridView.Rows.Add(
                        iteration,
                        Math.Round(xPrev, 6),
                        Math.Round(f, 6),
                        Math.Round(fDerivative, 6),
                        Math.Round(x, 6),
                        Math.Round(Math.Abs(x - xPrev), 6)
                    );

                } while (Math.Abs(x - xPrev) > epsilon && iteration < 100);

                lblResult.Text = $"Результат: x = {Math.Round(x, 6)}\n" +
                               $"Значение функции: f(x) = {Math.Round(Function(x), 6)}\n" +
                               $"Количество итераций: {iteration}\n" +
                               $"Достигнутая точность: {Math.Abs(x - xPrev)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка расчета");
            }
        }

        // Пример функции: f(x) = x^2 - 2
        private double Function(double x)
        {
            return x * x - 2; // Решение: x = √2 ≈ 1.41421356
        }

        // Производная: f'(x) = 2x
        private double FunctionDerivative(double x)
        {
            return 2 * x;
        }

        private void ShowNewtonInstruction(object sender, EventArgs e)
        {
            string instruction =
                "🔄 МЕТОД НЬЮТОНА (КАСАТЕЛЬНЫХ)\n\n" +
                "ЦЕЛЬ: Быстрый поиск корня уравнения f(x) = 0\n\n" +
                "ПРИНЦИП РАБОТЫ:\n" +
                "• Использует касательные к функции\n" +
                "• Формула: xₙ₊₁ = xₙ - f(xₙ)/f'(xₙ)\n" +
                "• Требует вычисления производной\n\n" +
                "📋 ИНСТРУКЦИЯ:\n" +
                "1. ВВОД ПАРАМЕТРОВ:\n" +
                "   • x₀ - начальное приближение\n" +
                "   • ε - требуемая точность\n\n" +
                "2. УСЛОВИЯ СХОДИМОСТИ:\n" +
                "   • f'(x) ≠ 0 вблизи корня\n" +
                "   • f''(x) непрерывна\n" +
                "   • Начальное приближение должно быть близко к корню\n\n" +
                "3. ЗАПУСК:\n" +
                "   • Нажмите 'Вычислить'\n" +
                "   • Программа покажет все итерации\n\n" +
                "📊 РЕЗУЛЬТАТЫ:\n" +
                "• Таблица итераций с:\n" +
                "  - Текущим x\n" +
                "  - Значением функции f(x)\n" +
                "  - Производной f'(x)\n" +
                "  - Следующим приближением\n" +
                "• Конечное значение корня\n" +
                "• Достигнутая точность\n\n" +
                "⚠️ ОСОБЕННОСТИ:\n" +
                "• Сходится очень быстро (квадратичная сходимость)\n" +
                "• Может расходиться при плохом начальном приближении\n" +
                "• Требует вычисления производной";

            MessageBox.Show(instruction, "Инструкция - Метод Ньютона",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}