using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace DihotomyMethodApp
{
    public class ShakerSort : SortAlgorithm
    {
        public override string Name => "Шейкерная";

        public override SortResult Sort(int[] array, bool ascending)
        {
            var watch = Stopwatch.StartNew();
            comparisons = 0; swaps = 0; steps.Clear(); AddStep(array);

            int left = 0, right = array.Length - 1;
            while (left < right)
            {
                for (int i = left; i < right; i++)
                    if (Compare(array[i], array[i + 1], ascending) > 0)
                    { Swap(ref array[i], ref array[i + 1]); AddStep(array); }
                right--;

                for (int i = right; i > left; i--)
                    if (Compare(array[i - 1], array[i], ascending) > 0)
                    { Swap(ref array[i - 1], ref array[i]); AddStep(array); }
                left++;
            }

            watch.Stop();
            return new SortResult { AlgorithmName = Name, ExecutionTime = watch.Elapsed, Comparisons = comparisons, Swaps = swaps, SortedArray = array, Steps = new List<int[]>(steps), IsSorted = true };
        }
    }
}