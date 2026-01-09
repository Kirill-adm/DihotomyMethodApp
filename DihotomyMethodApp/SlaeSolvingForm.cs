using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DihotomyMethodApp
{
    public partial class SlaeSolvingForm : Form
    {
        // Основные элементы управления
        private DataGridView dgvMatrix;
        private DataGridView dgvResults;
        private Panel controlPanel;
        private TextBox txtLog;
        private MenuStrip menuStrip;
        private NumericUpDown nudSize;
        private ComboBox cmbMethod;
        private ProgressBar progressBar;

        // Данные матрицы
        private double[,] matrixA;
        private double[] vectorB;
        private double[] vectorX;

        public SlaeSolvingForm()
        {
            InitializeComponent();
            InitializeDataGrid(3); // Начальный размер 3x3
        }

        private void InitializeComponent()
        {
            // Основные настройки формы
            this.Text = "Методы решения СЛАУ";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);

            // Создание меню
            CreateMenuStrip();

            // Левая панель управления
            controlPanel = new Panel
            {
                Location = new Point(20, 50),
                Size = new Size(300, 700),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke
            };

            // Элементы управления на левой панели
            var lblTitle = new Label
            {
                Text = "УПРАВЛЕНИЕ СЛАУ",
                Location = new Point(20, 20),
                Size = new Size(260, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblSize = new Label
            {
                Text = "Размер матрицы (N):",
                Location = new Point(20, 70),
                Size = new Size(150, 25)
            };

            nudSize = new NumericUpDown
            {
                Location = new Point(180, 68),
                Size = new Size(100, 25),
                Minimum = 2,
                Maximum = 50,
                Value = 3
            };
            nudSize.ValueChanged += (s, e) => InitializeDataGrid((int)nudSize.Value);

            var lblMethod = new Label
            {
                Text = "Метод решения:",
                Location = new Point(20, 110),
                Size = new Size(150, 25)
            };

            cmbMethod = new ComboBox
            {
                Location = new Point(180, 108),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbMethod.Items.AddRange(new object[] { "Гаусса", "Жордана-Гаусса", "Крамера" });
            cmbMethod.SelectedIndex = 0;

            // Кнопки управления
            var btnGenerate = new Button
            {
                Text = "Сгенерировать A,B",
                Location = new Point(20, 160),
                Size = new Size(260, 35),
                BackColor = Color.LightBlue
            };
            btnGenerate.Click += BtnGenerate_Click;

            var btnClear = new Button
            {
                Text = "Очистить матрицу",
                Location = new Point(20, 205),
                Size = new Size(260, 35),
                BackColor = Color.LightCoral
            };
            btnClear.Click += BtnClear_Click;

            var btnSolve = new Button
            {
                Text = "РЕШИТЬ СЛАУ",
                Location = new Point(20, 250),
                Size = new Size(260, 45),
                BackColor = Color.LightGreen,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnSolve.Click += async (s, e) => await BtnSolve_ClickAsync(s, e);

            var btnExport = new Button
            {
                Text = "Экспорт в Excel",
                Location = new Point(20, 310),
                Size = new Size(125, 35),
                BackColor = Color.LightGoldenrodYellow
            };
            btnExport.Click += BtnExport_Click;

            var btnLoadExcel = new Button
            {
                Text = "Загрузить Excel",
                Location = new Point(155, 310),
                Size = new Size(125, 35),
                BackColor = Color.LightSkyBlue
            };
            btnLoadExcel.Click += BtnLoadExcel_Click;

            progressBar = new ProgressBar
            {
                Location = new Point(20, 360),
                Size = new Size(260, 20),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            var lblResultsTitle = new Label
            {
                Text = "РЕЗУЛЬТАТЫ:",
                Location = new Point(20, 400),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            // Таблица для результатов
            dgvResults = new DataGridView
            {
                Location = new Point(20, 430),
                Size = new Size(260, 250),
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            dgvResults.Columns.Add("Variable", "Переменная");
            dgvResults.Columns.Add("Value", "Значение");
            dgvResults.Columns[0].Width = 120;
            dgvResults.Columns[1].Width = 120;

            // Добавление элементов на левую панель
            controlPanel.Controls.AddRange(new Control[] {
                lblTitle, lblSize, nudSize, lblMethod, cmbMethod,
                btnGenerate, btnClear, btnSolve, btnExport, btnLoadExcel,
                progressBar, lblResultsTitle, dgvResults
            });

            // Центральная область - матрица коэффициентов
            var matrixPanel = new Panel
            {
                Location = new Point(340, 50),
                Size = new Size(500, 350),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var lblMatrix = new Label
            {
                Text = "МАТРИЦА КОЭФФИЦИЕНТОВ A и ВЕКТОР B",
                Location = new Point(10, 10),
                Size = new Size(480, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            dgvMatrix = new DataGridView
            {
                Location = new Point(10, 50),
                Size = new Size(480, 290),
                BackgroundColor = Color.White,
                RowHeadersVisible = true,
                AllowUserToAddRows = false,
                ColumnHeadersHeight = 30,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            };

            matrixPanel.Controls.AddRange(new Control[] { lblMatrix, dgvMatrix });

            // Нижняя панель - лог выполнения
            var logPanel = new Panel
            {
                Location = new Point(340, 420),
                Size = new Size(820, 330),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var lblLog = new Label
            {
                Text = "ЛОГ ВЫПОЛНЕНИЯ И ДЕТАЛИ РАСЧЕТА",
                Location = new Point(10, 10),
                Size = new Size(800, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            txtLog = new TextBox
            {
                Location = new Point(10, 50),
                Size = new Size(800, 270),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Consolas", 9),
                BackColor = Color.Black,
                ForeColor = Color.Lime
            };

            logPanel.Controls.AddRange(new Control[] { lblLog, txtLog });

            // Добавление всех панелей на форму
            this.Controls.AddRange(new Control[] { controlPanel, matrixPanel, logPanel, menuStrip });
            this.MainMenuStrip = menuStrip;
        }

        private void CreateMenuStrip()
        {
            menuStrip = new MenuStrip();

            // Меню "Файл"
            var fileMenu = new ToolStripMenuItem("Файл");
            var loadExcelItem = new ToolStripMenuItem("Загрузить из Excel");
            var loadGoogleItem = new ToolStripMenuItem("Загрузить из Google Sheets");
            var generateItem = new ToolStripMenuItem("Сгенерировать данные");
            var exportItem = new ToolStripMenuItem("Экспорт результатов");
            var clearItem = new ToolStripMenuItem("Очистить все");
            var exitItem = new ToolStripMenuItem("Выход");

            loadExcelItem.Click += BtnLoadExcel_Click;
            loadGoogleItem.Click += (s, e) => MessageBox.Show("Функция загрузки из Google Sheets в разработке", "Инфо");
            generateItem.Click += BtnGenerate_Click;
            exportItem.Click += BtnExport_Click;
            clearItem.Click += BtnClear_Click;
            exitItem.Click += (s, e) => this.Close();

            fileMenu.DropDownItems.AddRange(new ToolStripItem[] {
                loadExcelItem, loadGoogleItem,
                new ToolStripSeparator(),
                generateItem, exportItem,
                new ToolStripSeparator(),
                clearItem, exitItem
            });

            // Меню "Решение"
            var solveMenu = new ToolStripMenuItem("Решение");
            var solveGaussItem = new ToolStripMenuItem("Метод Гаусса");
            var solveJordanItem = new ToolStripMenuItem("Метод Жордана-Гаусса");
            var solveCramerItem = new ToolStripMenuItem("Метод Крамера");
            var compareItem = new ToolStripMenuItem("Сравнить все методы");

            solveGaussItem.Click += async (s, e) => await SolveWithMethodAsync("Гаусса");
            solveJordanItem.Click += async (s, e) => await SolveWithMethodAsync("Жордана-Гаусса");
            solveCramerItem.Click += async (s, e) => await SolveWithMethodAsync("Крамера");
            compareItem.Click += async (s, e) => await CompareAllMethodsAsync();

            solveMenu.DropDownItems.AddRange(new ToolStripItem[] {
                solveGaussItem, solveJordanItem, solveCramerItem,
                new ToolStripSeparator(),
                compareItem
            });

            // Меню "Справка"
            var helpMenu = new ToolStripMenuItem("Справка");
            var instructionItem = new ToolStripMenuItem("Инструкция");
            var aboutItem = new ToolStripMenuItem("О программе");

            instructionItem.Click += ShowInstruction;
            aboutItem.Click += ShowAbout;

            helpMenu.DropDownItems.AddRange(new ToolStripItem[] { instructionItem, aboutItem });

            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, solveMenu, helpMenu });
        }

        private void InitializeDataGrid(int size)
        {
            dgvMatrix.Columns.Clear();
            dgvMatrix.Rows.Clear();

            // Добавляем столбцы для матрицы A
            for (int i = 0; i < size; i++)
            {
                dgvMatrix.Columns.Add($"col{i}", $"x{i + 1}");
                dgvMatrix.Columns[i].Width = 70;
            }

            // Добавляем столбец для вектора B
            dgvMatrix.Columns.Add("colB", "b");
            dgvMatrix.Columns[size].Width = 70;

            // Добавляем строки
            for (int i = 0; i < size; i++)
            {
                dgvMatrix.Rows.Add();
                dgvMatrix.Rows[i].HeaderCell.Value = $"Ур. {i + 1}";
            }

            // Инициализируем случайными значениями
            Random rnd = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    dgvMatrix.Rows[i].Cells[j].Value = rnd.Next(-10, 10).ToString();
                }
                dgvMatrix.Rows[i].Cells[size].Value = rnd.Next(-20, 20).ToString();
            }
        }

        private async Task BtnSolve_ClickAsync(object sender, EventArgs e)
        {
            await SolveWithMethodAsync(cmbMethod.SelectedItem.ToString());
        }

        private async Task SolveWithMethodAsync(string methodName)
        {
            try
            {
                // Показываем прогресс
                progressBar.Visible = true;
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Запуск метода {methodName}...\r\n");

                // Считываем данные из таблицы
                int n = (int)nudSize.Value;
                matrixA = new double[n, n];
                vectorB = new double[n];

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (!double.TryParse(dgvMatrix.Rows[i].Cells[j].Value?.ToString(), out matrixA[i, j]))
                            throw new FormatException($"Неверное значение в A[{i + 1},{j + 1}]");
                    }

                    if (!double.TryParse(dgvMatrix.Rows[i].Cells[n].Value?.ToString(), out vectorB[i]))
                        throw new FormatException($"Неверное значение в B[{i + 1}]");
                }

                // Запускаем решение в отдельном потоке
                var watch = System.Diagnostics.Stopwatch.StartNew();
                string details = "";

                vectorX = await Task.Run(() =>
                {
                    switch (methodName)
                    {
                        case "Гаусса":
                            return SolveGauss(matrixA, vectorB, ref details);
                        case "Жордана-Гаусса":
                            return SolveJordanGauss(matrixA, vectorB, ref details);
                        case "Крамера":
                            return SolveCramer(matrixA, vectorB, ref details);
                        default:
                            throw new ArgumentException("Неизвестный метод");
                    }
                });

                watch.Stop();

                // Обновляем интерфейс
                UpdateResults();
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Метод {methodName} выполнен за {watch.Elapsed.TotalMilliseconds:F2} мс\r\n");
                txtLog.AppendText($"Детали:\r\n{details}\r\n");
                txtLog.AppendText("=".PadRight(60, '=') + "\r\n");
                txtLog.ScrollToCaret();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка решения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ОШИБКА: {ex.Message}\r\n");
            }
            finally
            {
                progressBar.Visible = false;
            }
        }

        private async Task CompareAllMethodsAsync()
        {
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Сравнение всех методов...\r\n");

            var methods = new[] { "Гаусса", "Жордана-Гаусса", "Крамера" };
            var results = new Dictionary<string, (double[] solution, TimeSpan time)>();

            foreach (var method in methods)
            {
                try
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    string details = "";
                    var solution = await Task.Run(() =>
                    {
                        switch (method)
                        {
                            case "Гаусса":
                                return SolveGauss(matrixA, vectorB, ref details);
                            case "Жордана-Гаусса":
                                return SolveJordanGauss(matrixA, vectorB, ref details);
                            case "Крамера":
                                return SolveCramer(matrixA, vectorB, ref details);
                            default:
                                return null;
                        }
                    });
                    watch.Stop();

                    results[method] = (solution, watch.Elapsed);
                    txtLog.AppendText($"  {method}: {watch.Elapsed.TotalMilliseconds:F2} мс\r\n");
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"  {method}: ОШИБКА - {ex.Message}\r\n");
                }
            }

            txtLog.AppendText($"\r\nРЕЗЮМЕ СРАВНЕНИЯ:\r\n");
            foreach (var kvp in results.OrderBy(r => r.Value.time))
            {
                txtLog.AppendText($"  {kvp.Key}: {kvp.Value.time.TotalMilliseconds:F2} мс\r\n");
            }
            txtLog.AppendText("=".PadRight(60, '=') + "\r\n");
        }

        // Реализация метода Гаусса
        private double[] SolveGauss(double[,] A, double[] B, ref string details)
        {
            int n = B.Length;
            details = "МЕТОД ГАУССА:\r\n";
            details += "Прямой ход (приведение к треугольному виду):\r\n";

            double[,] a = (double[,])A.Clone();
            double[] b = (double[])B.Clone();

            for (int k = 0; k < n - 1; k++)
            {
                // Поиск главного элемента
                int maxRow = k;
                double maxVal = Math.Abs(a[k, k]);
                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(a[i, k]) > maxVal)
                    {
                        maxVal = Math.Abs(a[i, k]);
                        maxRow = i;
                    }
                }

                if (maxRow != k)
                {
                    details += $"  Меняем строки {k + 1} и {maxRow + 1}\r\n";
                    for (int j = k; j < n; j++)
                    {
                        (a[k, j], a[maxRow, j]) = (a[maxRow, j], a[k, j]);
                    }
                    (b[k], b[maxRow]) = (b[maxRow], b[k]);
                }

                // Исключение переменных
                for (int i = k + 1; i < n; i++)
                {
                    double factor = a[i, k] / a[k, k];
                    details += $"  Из строки {i + 1} вычитаем строку {k + 1} * {factor:F4}\r\n";

                    for (int j = k; j < n; j++)
                    {
                        a[i, j] -= factor * a[k, j];
                    }
                    b[i] -= factor * b[k];
                }
            }

            // Обратный ход
            details += "\r\nОбратный ход:\r\n";
            double[] x = new double[n];

            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < n; j++)
                {
                    sum += a[i, j] * x[j];
                }
                x[i] = (b[i] - sum) / a[i, i];
                details += $"  x{i + 1} = ({b[i]:F4} - {sum:F4}) / {a[i, i]:F4} = {x[i]:F6}\r\n";
            }

            return x;
        }

        // Реализация метода Жордана-Гаусса
        private double[] SolveJordanGauss(double[,] A, double[] B, ref string details)
        {
            int n = B.Length;
            details = "МЕТОД ЖОРДАНА-ГАУССА:\r\n";
            details += "Приведение к диагональному виду:\r\n";

            double[,] a = (double[,])A.Clone();
            double[] b = (double[])B.Clone();

            for (int k = 0; k < n; k++)
            {
                // Нормализация k-й строки
                double pivot = a[k, k];
                if (Math.Abs(pivot) < 1e-10)
                    throw new InvalidOperationException("Матрица вырожденная");

                for (int j = 0; j < n; j++)
                {
                    a[k, j] /= pivot;
                }
                b[k] /= pivot;

                details += $"  Нормализуем строку {k + 1} (делим на {pivot:F4})\r\n";

                // Исключение k-го столбца из других строк
                for (int i = 0; i < n; i++)
                {
                    if (i != k)
                    {
                        double factor = a[i, k];
                        if (Math.Abs(factor) > 1e-10)
                        {
                            details += $"  Из строки {i + 1} вычитаем строку {k + 1} * {factor:F4}\r\n";

                            for (int j = 0; j < n; j++)
                            {
                                a[i, j] -= factor * a[k, j];
                            }
                            b[i] -= factor * b[k];
                        }
                    }
                }
            }

            return b; // После преобразования b содержит решение
        }

        // Реализация метода Крамера
        private double[] SolveCramer(double[,] A, double[] B, ref string details)
        {
            int n = B.Length;
            details = "МЕТОД КРАМЕРА:\r\n";

            if (n > 10)
                throw new InvalidOperationException("Метод Крамера не рекомендуется для n > 10 из-за сложности вычислений");

            double detA = Determinant(A);
            details += $"  det(A) = {detA:F6}\r\n";

            if (Math.Abs(detA) < 1e-10)
                throw new InvalidOperationException("Матрица вырожденная");

            double[] x = new double[n];
            for (int i = 0; i < n; i++)
            {
                double[,] Ai = ReplaceColumn(A, B, i);
                double detAi = Determinant(Ai);
                x[i] = detAi / detA;
                details += $"  det(A{i + 1}) = {detAi:F6}, x{i + 1} = {detAi:F6}/{detA:F6} = {x[i]:F6}\r\n";
            }

            return x;
        }

        private double Determinant(double[,] matrix)
        {
            int n = matrix.GetLength(0);

            if (n == 1) return matrix[0, 0];
            if (n == 2) return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

            double det = 0;
            for (int j = 0; j < n; j++)
            {
                double[,] minor = GetMinor(matrix, 0, j);
                det += matrix[0, j] * Math.Pow(-1, j) * Determinant(minor);
            }
            return det;
        }

        private double[,] GetMinor(double[,] matrix, int row, int col)
        {
            int n = matrix.GetLength(0);
            double[,] minor = new double[n - 1, n - 1];

            int mRow = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == row) continue;

                int mCol = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == col) continue;
                    minor[mRow, mCol] = matrix[i, j];
                    mCol++;
                }
                mRow++;
            }
            return minor;
        }

        private double[,] ReplaceColumn(double[,] A, double[] B, int col)
        {
            int n = B.Length;
            double[,] result = new double[n, n];
            Array.Copy(A, result, A.Length);

            for (int i = 0; i < n; i++)
                result[i, col] = B[i];

            return result;
        }

        private void UpdateResults()
        {
            dgvResults.Rows.Clear();
            if (vectorX != null)
            {
                for (int i = 0; i < vectorX.Length; i++)
                {
                    dgvResults.Rows.Add($"x{i + 1}", Math.Round(vectorX[i], 6));
                }
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            InitializeDataGrid((int)nudSize.Value);
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Матрица сгенерирована (размер {nudSize.Value}x{nudSize.Value})\r\n");
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            dgvMatrix.Rows.Clear();
            dgvResults.Rows.Clear();
            txtLog.Clear();
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Все данные очищены\r\n");
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "CSV файлы|*.csv|Текстовые файлы|*.txt";
                    sfd.Title = "Экспорт результатов";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName))
                        {
                            // Записываем матрицу A
                            sw.WriteLine("Матрица коэффициентов A:");
                            for (int i = 0; i < (int)nudSize.Value; i++)
                            {
                                for (int j = 0; j < (int)nudSize.Value; j++)
                                {
                                    sw.Write($"{dgvMatrix.Rows[i].Cells[j].Value}\t");
                                }
                                sw.WriteLine();
                            }

                            sw.WriteLine("\nВектор B:");
                            for (int i = 0; i < (int)nudSize.Value; i++)
                            {
                                sw.WriteLine($"{dgvMatrix.Rows[i].Cells[(int)nudSize.Value].Value}");
                            }

                            if (vectorX != null)
                            {
                                sw.WriteLine("\nРешение X:");
                                for (int i = 0; i < vectorX.Length; i++)
                                {
                                    sw.WriteLine($"x{i + 1} = {vectorX[i]}");
                                }
                            }
                        }
                        MessageBox.Show($"Данные экспортированы в файл:\n{sfd.FileName}", "Экспорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoadExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV файлы|*.csv|Excel файлы|*.xlsx;*.xls|Текстовые файлы|*.txt";
                ofd.Title = "Загрузка данных из файла";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Простая загрузка CSV
                        var lines = System.IO.File.ReadAllLines(ofd.FileName);
                        int rowCount = Math.Min(lines.Length, (int)nudSize.Value);

                        for (int i = 0; i < rowCount; i++)
                        {
                            var values = lines[i].Split(',');
                            for (int j = 0; j < Math.Min(values.Length, (int)nudSize.Value + 1); j++)
                            {
                                if (j < dgvMatrix.Columns.Count)
                                {
                                    dgvMatrix.Rows[i].Cells[j].Value = values[j].Trim();
                                }
                            }
                        }

                        txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Данные загружены из {ofd.FileName}\r\n");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ShowInstruction(object sender, EventArgs e)
        {
            string instruction = "ИНСТРУКЦИЯ ПО РЕШЕНИЮ СЛАУ\n\n" +
                               "1. Установите размер матрицы N (от 2 до 50)\n" +
                               "2. Введите значения матрицы A и вектора B в таблице\n" +
                               "3. Выберите метод решения из выпадающего списка\n" +
                               "4. Нажмите 'РЕШИТЬ СЛАУ' или используйте меню 'Решение'\n" +
                               "5. Результаты появятся в правой таблице\n" +
                               "6. Детали расчета можно посмотреть в логе\n\n" +
                               "ДОСТУПНЫЕ МЕТОДЫ:\n" +
                               "• Метод Гаусса - классический метод исключения\n" +
                               "• Метод Жордана-Гаусса - модификация с диагонализацией\n" +
                               "• Метод Крамера - через определители (для n ≤ 10)\n\n" +
                               "ДОПОЛНИТЕЛЬНЫЕ ФУНКЦИИ:\n" +
                               "• Генерация случайной матрицы\n" +
                               "• Загрузка из CSV/Excel файлов\n" +
                               "• Экспорт результатов\n" +
                               "• Сравнение времени работы методов";

            MessageBox.Show(instruction, "Инструкция", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAbout(object sender, EventArgs e)
        {
            string about = "ПРИЛОЖЕНИЕ ДЛЯ РЕШЕНИЯ СЛАУ\n\n" +
                          "Разработано в рамках лабораторной работы\n" +
                          "© 2024 Кафедра вычислительной математики\n\n" +
                          "ФУНКЦИОНАЛ:\n" +
                          "• Решение систем линейных уравнений\n" +
                          "• 3 численных метода\n" +
                          "• Асинхронные вычисления\n" +
                          "• Визуализация процесса\n" +
                          "• Импорт/экспорт данных";

            MessageBox.Show(about, "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}