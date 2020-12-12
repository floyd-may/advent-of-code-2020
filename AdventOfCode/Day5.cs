using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day5 : SolverBase
    {
        public Day5()
        {
            EncodedSeatNumbers = LoadRawData()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Select(ch => _map[ch]).ToArray())
                .Select(x => new string(x))
                .Select(x => Convert.ToInt32(x, 2))
                .OrderBy(x => x)
                .ToArray();
        }
        private readonly Dictionary<char, char> _map = new Dictionary<char, char> {
            { 'F', '0' },
            { 'B', '1' },
            { 'R', '1' },
            { 'L', '0' },
        };

        public IEnumerable<int> EncodedSeatNumbers { get; }

        protected override object Part1Solution => EncodedSeatNumbers.Max();

        protected override object Part2Solution => EncodedSeatNumbers
                .Select((seat, idx) => new { seat, expected = idx + EncodedSeatNumbers.First() })
                .Where(x => x.seat > x.expected)
                .Select(x => x.seat)
                .First() - 1;
    }
}