using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace DihotomyMethodApp
{
    public class BogoSort : SortAlgorithm
    {
        public override string Name => "BOGO";
        private Random random = new Random();

        public override SortResult Sort(int[] array, bool ascending)
        {
            var watch = Stopwatch.StartNew();
            comparisons = 0; swaps = 0; steps.Clear(); AddStep(array);
            int attempts = 0, maxAttempts = 10000;
            while (!IsSorted(array, ascending) && attempts < maxAttempts)
            { Shuffle(array); attempts++; AddStep(array); }
            watch.Stop();
            return new SortResult { AlgorithmName = Name, ExecutionTime = watch.Elapsed, Comparisons = comparisons, Swaps = swaps, SortedArray = array, Steps = new List<int[]>(steps), IsSorted = IsSorted(array, ascending) };
        }

        private bool IsSorted(int[] array, bool ascending)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                comparisons++;
                if ((ascending && array[i] > array[i + 1]) || (!ascending && array[i] < array[i + 1])) return false;
            }
            return true;
        }

        private void Shuffle(int[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            { int j = random.Next(i + 1); Swap(ref array[i], ref array[j]); }
        }
    }
}