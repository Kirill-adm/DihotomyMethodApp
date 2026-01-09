using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace DihotomyMethodApp
{
    public partial class OlympicSortingForm : Form
    {
        private List<int> data = new List<int>();
        private List<SortResult> results = new List<SortResult>();
        private Dictionary<string, Panel> visualizerPanels = new Dictionary<string, Panel>();
        private Color[] barColors = { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Purple };
        private DataGridView dataGridView;
        private Panel visualizationPanel;
        private Panel algorithmsPanel;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadExcelToolStripMenuItem;
        private ToolStripMenuItem loadGoogleToolStripMenuItem;
        private ToolStripMenuItem generateToolStripMenuItem;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem sortToolStripMenuItem;
        private ToolStripMenuItem startSortToolStripMenuItem;
        private ToolStripMenuItem compareToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem instructionToolStripMenuItem;

        public OlympicSortingForm()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeVisualizationPanels();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.menuStrip = new MenuStrip();

            this.fileToolStripMenuItem = new ToolStripMenuItem("Файл");
            this.loadExcelToolStripMenuItem = new ToolStripMenuItem("Загрузить из Excel");
            this.loadGoogleToolStripMenuItem = new ToolStripMenuItem("Загрузить из Google Sheets");
            this.generateToolStripMenuItem = new ToolStripMenuItem("Сгенерировать данные");
            this.clearToolStripMenuItem = new ToolStripMenuItem("Очистить");
            this.exitToolStripMenuItem = new ToolStripMenuItem("Выход");

            this.sortToolStripMenuItem = new ToolStripMenuItem("Сортировка");
            this.startSortToolStripMenuItem = new ToolStripMenuItem("Начать сортировку");
            this.compareToolStripMenuItem = new ToolStripMenuItem("Сравнить алгоритмы");

            this.helpToolStripMenuItem = new ToolStripMenuItem("Справка");
            this.instructionToolStripMenuItem = new ToolStripMenuItem("Инструкция");
            this.instructionToolStripMenuItem.Click += InstructionToolStripMenuItem_Click;
            this.helpToolStripMenuItem.DropDownItems.Add(instructionToolStripMenuItem);

            this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                loadExcelToolStripMenuItem, loadGoogleToolStripMenuItem, new ToolStripSeparator(),
                generateToolStripMenuItem, new ToolStripSeparator(), clearToolStripMenuItem,
                new ToolStripSeparator(), exitToolStripMenuItem });

            this.sortToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                startSortToolStripMenuItem, compareToolStripMenuItem });

            this.menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, sortToolStripMenuItem, helpToolStripMenuItem });

            this.dataGridView = new DataGridView();
            this.dataGridView.Dock = DockStyle.Top;
            this.dataGridView.Height = 150;
            this.dataGridView.Columns.Add("Value", "Значение");
            this.dataGridView.AllowUserToAddRows = true;
            this.dataGridView.AllowUserToDeleteRows = true;

            this.visualizationPanel = new Panel();
            this.visualizationPanel.Dock = DockStyle.Fill;
            this.visualizationPanel.AutoScroll = true;
            this.visualizationPanel.BackColor = Color.White;
            this.visualizationPanel.BorderStyle = BorderStyle.FixedSingle;

            this.algorithmsPanel = new Panel();
            this.algorithmsPanel.Dock = DockStyle.Bottom;
            this.algorithmsPanel.Height = 120;
            this.algorithmsPanel.BackColor = Color.LightGray;
            this.algorithmsPanel.BorderStyle = BorderStyle.FixedSingle;

            string[] algorithms = { "Пузырьковая", "Вставками", "Шейкерная", "Быстрая", "BOGO" };
            int xPos = 10;
            foreach (var algo in algorithms)
            {
                var checkBox = new CheckBox();
                checkBox.Text = algo;
                checkBox.Location = new Point(xPos, 20);
                checkBox.Width = 120;
                checkBox.Checked = true;
                this.algorithmsPanel.Controls.Add(checkBox);
                xPos += 130;
            }

            var ascendingRadio = new RadioButton();
            ascendingRadio.Text = "По возрастанию";
            ascendingRadio.Location = new Point(10, 60);
            ascendingRadio.Checked = true;
            this.algorithmsPanel.Controls.Add(ascendingRadio);

            var descendingRadio = new RadioButton();
            descendingRadio.Text = "По убыванию";
            descendingRadio.Location = new Point(150, 60);
            this.algorithmsPanel.Controls.Add(descendingRadio);

            this.Controls.Add(this.visualizationPanel);
            this.Controls.Add(this.algorithmsPanel);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.menuStrip);

            this.MainMenuStrip = this.menuStrip;
            this.Text = "Олимпиадные сортировки";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

            this.loadExcelToolStripMenuItem.Click += LoadExcelToolStripMenuItem_Click;
            this.loadGoogleToolStripMenuItem.Click += LoadGoogleToolStripMenuItem_Click;
            this.generateToolStripMenuItem.Click += GenerateToolStripMenuItem_Click;
            this.clearToolStripMenuItem.Click += ClearToolStripMenuItem_Click;
            this.exitToolStripMenuItem.Click += (s, e) => Application.Exit();
            this.startSortToolStripMenuItem.Click += StartSortToolStripMenuItem_Click;
            this.compareToolStripMenuItem.Click += CompareToolStripMenuItem_Click;

            AddSampleData();
        }

        private void InitializeDataGridView()
        {
            dataGridView.CellEndEdit += DataGridView_CellEndEdit;
            dataGridView.RowsAdded += (s, e) => ValidateAllData();
            dataGridView.DataError += (s, e) => MessageBox.Show("Некорректное значение! Введите целое число.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void InitializeVisualizationPanels()
        {
            string[] algorithms = { "Пузырьковая", "Вставками", "Шейкерная", "Быстрая", "BOGO" };
            int panelHeight = 130;
            int spacing = 10;
            int startY = 10;

            for (int i = 0; i < algorithms.Length; i++)
            {
                var panel = new Panel();
                panel.Height = panelHeight;
                panel.Width = visualizationPanel.Width - 40;
                panel.Location = new Point(20, startY + i * (panelHeight + spacing));
                panel.BackColor = Color.White;
                panel.BorderStyle = BorderStyle.FixedSingle;
                panel.Tag = algorithms[i];
                panel.Paint += Panel_Paint;

                var label = new Label();
                label.Text = algorithms[i];
                label.Location = new Point(5, 5);
                label.Font = new Font("Arial", 10, FontStyle.Bold);
                label.ForeColor = barColors[i % barColors.Length];
                label.Width = 120;
                panel.Controls.Add(label);

                var timeLabel = new Label();
                timeLabel.Name = "timeLabel";
                timeLabel.Location = new Point(130, 5);
                timeLabel.Width = 250;
                timeLabel.Font = new Font("Arial", 9);
                panel.Controls.Add(timeLabel);

                visualizerPanels[algorithms[i]] = panel;
                visualizationPanel.Controls.Add(panel);
            }
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel panel && panel.Tag is string algorithmName)
                DrawVisualization(e.Graphics, panel, algorithmName);
        }

        private void AddSampleData()
        {
            Random rnd = new Random();
            for (int i = 0; i < 15; i++)
                dataGridView.Rows.Add(rnd.Next(1, 101));
        }

        private void ValidateAllData()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                if (!int.TryParse(row.Cells[0].Value?.ToString(), out _))
                    row.Cells[0].Style.BackColor = Color.LightPink;
                else
                    row.Cells[0].Style.BackColor = Color.White;
            }
        }

        private void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (!int.TryParse(cell.Value?.ToString(), out _))
            {
                cell.Style.BackColor = Color.LightPink;
                MessageBox.Show("Введите целое число!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                cell.Style.BackColor = Color.White;
        }

        private List<int> GetDataFromGrid()
        {
            var list = new List<int>();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                if (int.TryParse(row.Cells[0].Value?.ToString(), out int value))
                    list.Add(value);
            }
            return list;
        }

        private void UpdateDataInGrid(List<int> newData)
        {
            dataGridView.Rows.Clear();
            foreach (var value in newData)
                dataGridView.Rows.Add(value);
        }

        private void LoadExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV Files|*.csv|All Files|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                    LoadCSVFile(ofd.FileName);
            }
        }

        private void LoadGoogleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV Files|*.csv|All Files|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                    LoadCSVFile(ofd.FileName);
            }
        }

        private void LoadCSVFile(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                var dataList = new List<int>();
                foreach (var line in lines)
                {
                    var separators = new char[] { ',', ';', '\t' };
                    var values = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var value in values)
                        if (int.TryParse(value.Trim(), out int num))
                            dataList.Add(num);
                }
                if (dataList.Count > 0)
                {
                    UpdateDataInGrid(dataList);
                    MessageBox.Show($"Загружено {dataList.Count} элементов из CSV", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Не найдено числовых данных в файле", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки CSV: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new GenerateDataForm())
                if (dialog.ShowDialog() == DialogResult.OK)
                    UpdateDataInGrid(dialog.GeneratedData);
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Очистить все данные?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dataGridView.Rows.Clear();
                data.Clear();
                results.Clear();
                foreach (var panel in visualizerPanels.Values)
                {
                    panel.Invalidate();
                    var timeLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "timeLabel");
                    if (timeLabel != null) timeLabel.Text = "";
                }
            }
        }

        private async void StartSortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data = GetDataFromGrid();
            if (data.Count == 0)
            {
                MessageBox.Show("Нет данных для сортировки!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (data.Count > 100 && MessageBox.Show($"Вы выбрали {data.Count} элементов. BOGO сортировка может работать очень долго.\nПродолжить?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            bool ascending = true;
            foreach (Control control in algorithmsPanel.Controls)
                if (control is RadioButton radio && radio.Checked)
                {
                    if (radio.Text == "По убыванию") ascending = false;
                    break;
                }

            results.Clear();
            var selectedAlgorithms = new List<string>();
            foreach (CheckBox cb in algorithmsPanel.Controls.OfType<CheckBox>())
                if (cb.Checked) selectedAlgorithms.Add(cb.Text);

            if (selectedAlgorithms.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один алгоритм!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var panel in visualizerPanels.Values)
            {
                var timeLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "timeLabel");
                if (timeLabel != null) timeLabel.Text = "Выполняется...";
                panel.Invalidate();
            }

            await Task.Run(() => RunSortingAlgorithms(data.ToArray(), ascending, selectedAlgorithms));
            UpdateVisualization();
            MessageBox.Show("Сортировка завершена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RunSortingAlgorithms(int[] array, bool ascending, List<string> algorithms)
        {
            foreach (var algoName in algorithms)
            {
                try
                {
                    // ИСПРАВЛЕННАЯ СТРОКА - убрано SortVisualization
                    var algorithm = SortAlgorithmFactory.CreateAlgorithm(algoName);
                    var result = algorithm.Sort((int[])array.Clone(), ascending);
                    this.Invoke(new Action(() =>
                    {
                        results.Add(result);
                        UpdateAlgorithmPanel(algoName, result);
                    }));
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() => MessageBox.Show($"Ошибка в алгоритме {algoName}: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }
        }

        private void UpdateAlgorithmPanel(string algorithmName, SortResult result)
        {
            if (visualizerPanels.ContainsKey(algorithmName))
            {
                var panel = visualizerPanels[algorithmName];
                var timeLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "timeLabel");
                if (timeLabel != null)
                {
                    string sortedStatus = result.IsSorted ? "✓" : "✗";
                    timeLabel.Text = $"Время: {result.ExecutionTime.TotalMilliseconds:F2} мс | Сравнения: {result.Comparisons} | Обмены: {result.Swaps} {sortedStatus}";
                }
                panel.Invalidate();
            }
        }

        private void UpdateVisualization()
        {
            foreach (var panel in visualizerPanels.Values)
                panel.Invalidate();
        }

        private void DrawVisualization(Graphics g, Panel panel, string algorithmName)
        {
            var result = results.FirstOrDefault(r => r.AlgorithmName == algorithmName);
            if (result == null || result.SortedArray == null || result.SortedArray.Length == 0)
            {
                g.DrawString("Нет данных", new Font("Arial", 12), Brushes.Gray, panel.Width / 2 - 40, panel.Height / 2 - 10);
                return;
            }

            int width = panel.Width - 20;
            int height = panel.Height - 40;
            int barCount = result.SortedArray.Length;
            if (barCount == 0) return;
            int barWidth = Math.Max(1, width / barCount - 1);
            int maxValue = result.SortedArray.Max();
            if (maxValue == 0) maxValue = 1;

            Color color;
            int index = -1;
            string[] keys = visualizerPanels.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
                if (keys[i] == algorithmName) { index = i; break; }
            if (index >= 0 && index < barColors.Length) color = barColors[index]; else color = Color.Black;

            for (int i = 0; i < barCount; i++)
            {
                int barHeight = (int)((double)result.SortedArray[i] / maxValue * height);
                int x = i * (barWidth + 1) + 10;
                int y = panel.Height - barHeight - 30;
                using (var brush = new SolidBrush(color))
                    g.FillRectangle(brush, x, y, barWidth, barHeight);
                g.DrawRectangle(Pens.Black, x, y, barWidth, barHeight);
                if (barCount <= 20)
                    g.DrawString(result.SortedArray[i].ToString(), new Font("Arial", 8), Brushes.Black, x, y - 15);
            }

            using (var font = new Font("Arial", 9, FontStyle.Bold))
                g.DrawString(algorithmName, font, new SolidBrush(color), 5, 5);
        }

        private void CompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (results.Count == 0)
            {
                MessageBox.Show("Сначала выполните сортировку!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fastest = results.OrderBy(r => r.ExecutionTime).First();
            string message = "Результаты сравнения алгоритмов:\n\n";
            foreach (var result in results.OrderBy(r => r.ExecutionTime))
            {
                string fastestMark = (result.AlgorithmName == fastest.AlgorithmName) ? " ← самый быстрый" : "";
                message += $"{result.AlgorithmName}: {result.ExecutionTime.TotalMilliseconds:F2} мс (сравнений: {result.Comparisons}, обменов: {result.Swaps}){fastestMark}\n";
            }

            MessageBox.Show(message, "Сравнение алгоритмов", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InstructionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string instruction = "🏆 ОЛИМПИАДНЫЕ СОРТИРОВКИ\n\n" +
                               "АЛГОРИТМЫ:\n1. 🔴 Пузырьковая - O(n²), учебный\n2. 🔵 Вставками - O(n²), эффективен для почти отсортированных\n" +
                               "3. 🟢 Шейкерная - улучшенная пузырьковая\n4. 🟠 Быстрая - O(n log n), самый популярный\n5. 🟣 BOGO - демонстрационный, крайне медленный\n\n" +
                               "📋 ИНСТРУКЦИЯ:\n1. Введите числа в таблицу (или сгенерируйте)\n2. Выберите алгоритмы галочками\n3. Выберите направление сортировки\n" +
                               "4. Нажмите 'Начать сортировку'\n5. Наблюдайте анимацию и сравнивайте результаты\n\n⚠️ ВНИМАНИЕ:\n• Для BOGO ограничьте 15 элементами\n" +
                               "• Большие массивы могут работать долго";

            MessageBox.Show(instruction, "Инструкция - Олимпиадные сортировки", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}