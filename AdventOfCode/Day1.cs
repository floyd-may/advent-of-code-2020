using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day1 : SolverBase
    {
        protected override object Part1Solution => Part1Full();

        protected override object Part2Solution => Part2Full();

        private int Part1Full()
        {
            var (a, b) = Part1(LoadIntData());

            return a * b;
        }

        private int Part2Full()
        {
            var (a, b, c) = Part2(LoadIntData());

            return a * b * c;
        }

        public (int, int) Part1(IEnumerable<int> input)
        {
            var indexedInput = input.Select((val, idx) => new { val, idx });

            var potentialSolutions = indexedInput
                .SelectMany(
                    x => indexedInput.Where(right => right.idx > x.idx),
                    (left, right) => new { left = left.val, right = right.val });

            var solution = potentialSolutions
                .Single(x => x.left + x.right == 2020);

            return (solution.left, solution.right);
        }

        public (int, int, int) Part2(IEnumerable<int> input)
        {
            var indexedInput = input.Select((val, idx) => new { val, idx });

            var potentialSolutions = indexedInput
                .SelectMany(
                    x => indexedInput.Where(mid => mid.idx > x.idx),
                    (left, mid) => new { left, mid })
                .SelectMany(
                    x => indexedInput.Where(right => right.idx > x.mid.idx),
                    (x, right) => new { left = x.left.val, mid = x.mid.val, right = right.val }
                );

            var solution = potentialSolutions
                .Single(x => x.left + x.mid + x.right == 2020);

            return (solution.left, solution.mid, solution.right);
        }
    }
}