using System.Linq;

namespace AdventOfCode
{
    public sealed class Day18 : SolverBase
    {
        protected override object Part1Solution => LoadRawData()
            .Select(x => (long)AdventOfCode.Fs.Day18.computePart1(x))
            .Sum();

        protected override object Part2Solution => null;
    }
}