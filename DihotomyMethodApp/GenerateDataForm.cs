using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DihotomyMethodApp
{
    public partial class GenerateDataForm : Form
    {
        public List<int> GeneratedData { get; private set; }

        public GenerateDataForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Генерация данных";
            this.Size = new Size(350, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblCount = new Label { Text = "Количество элементов:", Location = new Point(10, 20), Width = 150 };
            var nudCount = new NumericUpDown { Location = new Point(170, 18), Width = 150, Minimum = 5, Maximum = 100, Value = 20 };
            var lblMin = new Label { Text = "Минимальное значение:", Location = new Point(10, 50), Width = 150 };
            var nudMin = new NumericUpDown { Location = new Point(170, 48), Width = 150, Minimum = -1000, Maximum = 1000, Value = 1 };
            var lblMax = new Label { Text = "Максимальное значение:", Location = new Point(10, 80), Width = 150 };
            var nudMax = new NumericUpDown { Location = new Point(170, 78), Width = 150, Minimum = -1000, Maximum = 1000, Value = 100 };

            var btnRandom = new Button { Text = "Случайные", Location = new Point(10, 110), Width = 100 };
            var btnSorted = new Button { Text = "Отсортированные", Location = new Point(120, 110), Width = 100 };
            var btnReverse = new Button { Text = "Обратный порядок", Location = new Point(230, 110), Width = 100 };
            var btnCancel = new Button { Text = "Отмена", Location = new Point(120, 140), Width = 100 };

            btnRandom.Click += (s, e) => GenerateData((int)nudCount.Value, (int)nudMin.Value, (int)nudMax.Value, false, false);
            btnSorted.Click += (s, e) => GenerateData((int)nudCount.Value, (int)nudMin.Value, (int)nudMax.Value, true, true);
            btnReverse.Click += (s, e) => GenerateData((int)nudCount.Value, (int)nudMin.Value, (int)nudMax.Value, true, false);
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { lblCount, nudCount, lblMin, nudMin, lblMax, nudMax, btnRandom, btnSorted, btnReverse, btnCancel });
        }

        private void GenerateData(int count, int min, int max, bool sorted, bool ascending)
        {
            if (min >= max) { MessageBox.Show("Минимальное значение должно быть меньше максимального!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            var random = new Random(); GeneratedData = new List<int>();
            if (sorted)
            {
                int step = Math.Max(1, (max - min) / Math.Max(1, count - 1));
                for (int i = 0; i < count; i++)
                {
                    int value = ascending ? min + i * step : max - i * step;
                    GeneratedData.Add(Math.Min(value, max));
                }
            }
            else for (int i = 0; i < count; i++) GeneratedData.Add(random.Next(min, max + 1));
            this.DialogResult = DialogResult.OK; this.Close();
        }
    }
}