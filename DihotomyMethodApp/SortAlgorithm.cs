using System;
using System.Collections.Generic;

namespace DihotomyMethodApp
{
    public abstract class SortAlgorithm
    {
        public abstract string Name { get; }
        public abstract SortResult Sort(int[] array, bool ascending);

        protected int comparisons = 0;
        protected int swaps = 0;
        protected List<int[]> steps = new List<int[]>();

        protected void AddStep(int[] array) => steps.Add((int[])array.Clone());
        protected void Swap(ref int a, ref int b) { int temp = a; a = b; b = temp; swaps++; }
        protected int Compare(int a, int b, bool ascending) { comparisons++; return ascending ? a.CompareTo(b) : b.CompareTo(a); }
    }
}