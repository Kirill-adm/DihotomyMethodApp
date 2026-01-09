using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace DihotomyMethodApp
{
    public class BubbleSort : SortAlgorithm
    {
        public override string Name => "Пузырьковая";

        public override SortResult Sort(int[] array, bool ascending)
        {
            var watch = Stopwatch.StartNew();
            comparisons = 0; swaps = 0; steps.Clear(); AddStep(array);

            for (int i = 0; i < array.Length - 1; i++)
                for (int j = 0; j < array.Length - i - 1; j++)
                    if (Compare(array[j], array[j + 1], ascending) > 0)
                    { Swap(ref array[j], ref array[j + 1]); AddStep(array); }

            watch.Stop();
            return new SortResult { AlgorithmName = Name, ExecutionTime = watch.Elapsed, Comparisons = comparisons, Swaps = swaps, SortedArray = array, Steps = new List<int[]>(steps), IsSorted = true };
        }
    }
}