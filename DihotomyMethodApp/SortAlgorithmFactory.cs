using System;

namespace DihotomyMethodApp
{
    public static class SortAlgorithmFactory
    {
        public static SortAlgorithm CreateAlgorithm(string name)
        {
            switch (name)
            {
                case "Пузырьковая":
                    return new BubbleSort();
                case "Вставками":
                    return new InsertionSort();
                case "Шейкерная":
                    return new ShakerSort();
                case "Быстрая":
                    return new QuickSort();
                case "BOGO":
                    return new BogoSort();
                default:
                    return new BubbleSort();
            }
        }
    }
}