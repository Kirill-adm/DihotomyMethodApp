using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace DihotomyMethodApp
{
    public class QuickSort : SortAlgorithm
    {
        public override string Name => "Быстрая";
        private bool ascending;

        public override SortResult Sort(int[] array, bool ascending)
        {
            var watch = Stopwatch.StartNew();
            comparisons = 0; swaps = 0; steps.Clear(); this.ascending = ascending; AddStep(array);
            QuickSortRecursive(array, 0, array.Length - 1); watch.Stop();
            return new SortResult { AlgorithmName = Name, ExecutionTime = watch.Elapsed, Comparisons = comparisons, Swaps = swaps, SortedArray = array, Steps = new List<int[]>(steps), IsSorted = true };
        }

        private void QuickSortRecursive(int[] array, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(array, low, high);
                QuickSortRecursive(array, low, pi - 1);
                QuickSortRecursive(array, pi + 1, high);
            }
        }

        private int Partition(int[] array, int low, int high)
        {
            int pivot = array[high]; int i = low - 1;
            for (int j = low; j < high; j++)
            {
                comparisons++; bool condition = ascending ? array[j] < pivot : array[j] > pivot;
                if (condition) { i++; Swap(ref array[i], ref array[j]); AddStep(array); }
            }
            Swap(ref array[i + 1], ref array[high]); AddStep(array); return i + 1;
        }
    }
}