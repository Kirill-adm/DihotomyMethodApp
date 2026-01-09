using System;
using System.Collections.Generic;

namespace DihotomyMethodApp
{
    public class SortResult
    {
        public string AlgorithmName { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public int Comparisons { get; set; }
        public int Swaps { get; set; }
        public int[] SortedArray { get; set; }
        public List<int[]> Steps { get; set; } = new List<int[]>();
        public bool IsSorted { get; set; }
    }
}