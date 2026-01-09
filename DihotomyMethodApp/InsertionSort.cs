using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace DihotomyMethodApp
{
    public class InsertionSort : SortAlgorithm
    {
        public override string Name => "Вставками";

        public override SortResult Sort(int[] array, bool ascending)
        {
            var watch = Stopwatch.StartNew();
            comparisons = 0; swaps = 0; steps.Clear(); AddStep(array);

            for (int i = 1; i < array.Length; i++)
            {
                int key = array[i]; int j = i - 1;
                while (j >= 0 && Compare(array[j], key, ascending) > 0)
                { array[j + 1] = array[j]; swaps++; j--; AddStep(array); }
                array[j + 1] = key; swaps++; AddStep(array);
            }

            watch.Stop();
            return new SortResult { AlgorithmName = Name, ExecutionTime = watch.Elapsed, Comparisons = comparisons, Swaps = swaps, SortedArray = array, Steps = new List<int[]>(steps), IsSorted = true };
        }
    }
}