using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Day9 : SolverBase
    {
        protected override object Part1Solution => ComputePart1Solution();

        protected override object Part2Solution => ComputePart2Solution();

        protected override int DayNumber => 9;

        public int ComputePart1Solution(int[] data = null, int preambleLength = 25)
        {
            data = data ?? LoadIntData().ToArray();

            var solution = data
                .Select((num, idx) => new { num, idx })
                .Where(x => x.idx > (preambleLength - 1))
                .Select(x => new {
                    x.num,
                    previous25 = data[(x.idx - preambleLength)..(x.idx)]
                })
                .Where(x => !IsValid(x.num, x.previous25))
                .Select(x => x.num)
                .First()
                ;


            return solution;
        }

        public int ComputePart2Solution(int[] data = null, int preambleLength = 25)
        {
            data = data ?? LoadIntData().ToArray();

            var part1Solution = ComputePart1Solution(data, preambleLength);

            var solution = Enumerable.Range(0, data.Length)
                .Select(x => ProbeForSolution(data, part1Solution, x))
                .First(x => x.HasValue);


            return solution.Value;
        }

        private bool IsValid(int value, IList<int> previous25)
        {
            for(var aIdx = 0; aIdx < previous25.Count; aIdx++)
            {
                for(var bIdx = aIdx + 1; bIdx < previous25.Count; bIdx++)
                {
                    var a = previous25[aIdx];
                    var b = previous25[bIdx];

                    if(a + b == value)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private int? ProbeForSolution(int[] data, int part1Solution, int startIdx)
        {
            var idx = startIdx;
            var currentSum = data[idx];
            while(currentSum < part1Solution)
            {
                idx++;
                currentSum += data[idx];
            }

            if(currentSum == part1Solution && idx > startIdx)
            {
                var range = data[startIdx..idx];

                return range.Min() + range.Max();
            }

            return null;
        }
    }
}