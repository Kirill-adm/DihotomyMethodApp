using System;

private double[] SolveIterations(double[,] A, double[] b, int maxIterations, double epsilon, ref string solutionText)
{
    int n = b.Length;

    // Проверка диагонального преобладания
    solutionText += "Проверка диагонального преобладания:\n";
    bool hasDominance = true;
    for (int i = 0; i < n; i++)
    {
        double sum = 0;
        for (int j = 0; j < n; j++)
        {
            if (i != j) sum += Math.Abs(A[i, j]);
        }
        if (Math.Abs(A[i, i]) <= sum)
        {
            hasDominance = false;
            solutionText += $"⚠️ В строке {i + 1} нет диагонального преобладания\n";
        }
    }

    if (!hasDominance)
        solutionText += "\n⚠️ Внимание: Отсутствие диагонального преобладания может привести к расходимости метода!\n\n";

    double[] x = new double[n];
    double[] xNew = new double[n];
    // Инициализация нулями вместо Array.Fill (для совместимости)
    for (int i = 0; i < n; i++)
    {
        x[i] = 0;
        xNew[i] = 0;
    }

    solutionText += $"Начальное приближение: все x = 0\n";
    solutionText += $"Максимальное число итераций: {maxIterations}\n";
    solutionText += $"Точность: {epsilon}\n\n";
    solutionText += "Итерационный процесс:\n";

    for (int iter = 0; iter < maxIterations; iter++)
    {
        for (int i = 0; i < n; i++)
        {
            double sum = 0;
            for (int j = 0; j < n; j++)
            {
                if (i != j) sum += A[i, j] * x[j];
            }
            xNew[i] = (b[i] - sum) / A[i, i];
        }

        // Вычисление максимальной разности
        double maxDiff = 0;
        for (int i = 0; i < n; i++)
        {
            maxDiff = Math.Max(maxDiff, Math.Abs(xNew[i] - x[i]));
        }

        solutionText += $"\nИтерация {iter + 1}:\n";
        for (int i = 0; i < n; i++)
        {
            solutionText += $"  x{i + 1} = {xNew[i]:F6}\n";
        }
        solutionText += $"  Макс. изменение: {maxDiff:0.000000E+00}\n"; // Исправленная строка!

        if (maxDiff < epsilon)
        {
            solutionText += $"\n✅ Сходимость достигнута за {iter + 1} итераций\n";
            return xNew; // Возвращаем последнее приближение
        }

        // Копируем новое приближение для следующей итерации
        for (int i = 0; i < n; i++) // Вместо Array.Copy для совместимости
        {
            x[i] = xNew[i];
        }
    }

    solutionText += $"\n⚠️ Достигнуто максимальное число итераций ({maxIterations})\n";
    solutionText += "Возможно, требуется больше итераций или метод расходится.\n";
    return xNew; // Возвращаем последнее приближение
}