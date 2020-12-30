namespace AdventOfCode
{
    public sealed class Day19 : SolverBase
    {
        protected override object Part1Solution =>
            AdventOfCode.Fs.Day19.solvePart1(LoadRawData());

        protected override object Part2Solution =>
            AdventOfCode.Fs.Day19.solvePart2(LoadRawData());
    }
}