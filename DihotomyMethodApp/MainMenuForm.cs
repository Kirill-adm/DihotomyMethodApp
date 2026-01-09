using System;
using System.Drawing;
using System.Windows.Forms;

namespace DihotomyMethodApp
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Главное меню - Лабораторные работы";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.LightGray;

            var label = new Label();
            label.Text = "Выберите лабораторную работу:";
            label.Font = new Font("Arial", 16, FontStyle.Bold);
            label.Location = new Point(60, 20);
            label.Size = new Size(330, 40);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.BackColor = Color.Transparent;

            var btnTask1 = CreateMenuButton("1. Метод дихотомии", 70);
            btnTask1.Click += BtnTask1_Click;

            var btnTask2 = CreateMenuButton("2. Методы решения СЛАУ", 120);
            btnTask2.Click += BtnTask2_Click;

            var btnTask3 = CreateMenuButton("3. Олимпиадные сортировки", 170);
            btnTask3.Click += BtnTask3_Click;

            var btnTask4 = CreateMenuButton("4. Вычисление определенного интеграла", 220);
            btnTask4.Click += BtnTask4_Click;

            var btnAbout = new Button();
            btnAbout.Text = "📘 О программе";
            btnAbout.Location = new Point(175, 280);
            btnAbout.Size = new Size(100, 35);
            btnAbout.Font = new Font("Arial", 10);
            btnAbout.BackColor = Color.LightGreen;
            btnAbout.Click += BtnAbout_Click;

            var btnExit = new Button();
            btnExit.Text = "Выход";
            btnExit.Location = new Point(175, 330);
            btnExit.Size = new Size(100, 35);
            btnExit.Font = new Font("Arial", 10);
            btnExit.BackColor = Color.LightCoral;
            btnExit.Click += BtnExit_Click;

            this.Controls.AddRange(new Control[] {
                label, btnTask1, btnTask2, btnTask3, btnTask4, btnAbout, btnExit
            });
        }

        private Button CreateMenuButton(string text, int y)
        {
            var button = new Button();
            button.Text = text;
            button.Location = new Point(50, y);
            button.Size = new Size(350, 40);
            button.Font = new Font("Arial", 11, FontStyle.Regular);
            button.BackColor = Color.LightBlue;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = Color.DarkBlue;
            button.FlatAppearance.BorderSize = 1;
            return button;
        }

        private void BtnTask1_Click(object sender, EventArgs e)
        {
            var form = new DihotomyMethodForm();
            form.ShowDialog();
        }

        private void BtnTask2_Click(object sender, EventArgs e)
        {
            var form = new SlaeSolvingForm();
            form.ShowDialog();
        }

        private void BtnTask3_Click(object sender, EventArgs e)
        {
            var form = new OlympicSortingForm();
            form.ShowDialog();
        }

        private void BtnTask4_Click(object sender, EventArgs e)
        {
            var form = new IntegralCalculationForm();
            form.ShowDialog();
        }

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            string message = "📚 ПРОГРАММА ДЛЯ ЛАБОРАТОРНЫХ РАБОТ\n\n" +
                           "Содержит 4 задания:\n\n" +
                           "1. 📊 Метод дихотомии\n" +
                           "   • Поиск корней уравнений\n" +
                           "   • Визуализация итераций\n\n" +
                           "2. 🧮 Методы решения СЛАУ\n" +
                           "   • Метод Гаусса\n" +
                           "   • Метод Крамера\n" +
                           "   • Метод простых итераций\n\n" +
                           "3. 🏆 Олимпиадные сортировки\n" +
                           "   • 5 алгоритмов сортировки\n" +
                           "   • Анимация процесса\n" +
                           "   • Сравнение времени\n\n" +
                           "4. ∫ Вычисление определенного интеграла\n" +
                           "   • 3 метода интегрирования\n" +
                           "   • Графики функций\n" +
                           "   • Автоподбор разбиений\n\n" +
                           "Для каждого задания есть подробная инструкция внутри формы.";

            MessageBox.Show(message, "О программе",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}