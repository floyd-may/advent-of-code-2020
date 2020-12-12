using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day10 : SolverBase
    {
        protected override object Part1Solution => ComputePart1Solution();
        protected override object Part2Solution => ComputeFullPart2Solution();

        public int ComputePart1Solution(int[] input = null)
        {
            var orderedInput = (input ?? LoadIntData().AsEnumerable())
                .OrderBy(x => x)
                .ToList();

            // add the first item - the outlet
            orderedInput.Insert(0, 0);

            // add the last item - the device itself
            orderedInput.Add(orderedInput.Last() + 3);

            var deltas = orderedInput.Zip(
                orderedInput.Skip(1),
                (first, second) => second - first
                )
                .Where(x => x == 1 || x == 3);

            return deltas
                .GroupBy(x => x)
                .Select(x => x.Count())
                .Aggregate(1, (acc, val) => acc * val);
        }

        public long ComputeFullPart2Solution(int[] input = null)
        {
            var orderedInput = (input ?? LoadIntData().AsEnumerable())
                .OrderBy(x => x)
                .ToList();

            // add the last item - the device itself
            orderedInput.Add(orderedInput.Last() + 3);

            return ComputePart2Solutions(orderedInput.ToArray());
        }

        public long ComputePart2Solutions(int[] input)
        {
            var memos = new Dictionary<int, long> { { 0, 1 } };

            foreach(var x in input)
            {
                memos[x] = new[] { 1, 2, 3 }
                    .Select(i => memos.TryGetValue(x - i, out var prev) ? prev : 0)
                    .Sum();
            }

            return memos[input.Last()];
        }

    }
}