using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day6 : SolverBase
    {
        public sealed class Group
        {
            private readonly char[] _allChars;
            private readonly int _groupSize;

            public Group(string rawInput)
            {
                _allChars = rawInput
                    .Where(x => !char.IsWhiteSpace(x))
                    .ToArray();

                _groupSize = rawInput.Split("\n").Length;
            }

            public int Part1Sum => _allChars
                .Distinct()
                .Count();

            public int Part2Sum => _allChars
                .GroupBy(x => x)
                .Where(x => x.Count() == _groupSize)
                .Count();
        }

        public IEnumerable<Group> Groups => string.Join("\n", LoadRawData())
            .Split("\n\n")
            .Select(x => new Group(x));

        protected override object Part1Solution => Groups
            .Select(x => x.Part1Sum)
            .Sum();

        protected override object Part2Solution => Groups
            .Select(x => x.Part2Sum)
            .Sum();

        protected override int DayNumber => 6;
    }
}