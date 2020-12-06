using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public abstract class SolverBase
    {
        public IEnumerable<string> LoadRawData()
        {
            var type = this.GetType();
            var resourceName = $"{type.Namespace}.{type.Name}.txt";
            using var stream = type.Assembly.GetManifestResourceStream(resourceName);
            using var rdr = new StreamReader(stream);

            var output = new List<string>();
            while(!rdr.EndOfStream)
            {
                output.Add(rdr.ReadLine());
            }

            return output;
        }

        public IEnumerable<int> LoadIntData()
        {
            var source = LoadRawData();

            return source
                .Select(TryParseInt)
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToArray();
        }

        public void PrintSolution()
        {
            var part1 = Part1Solution;
            var part2 = Part2Solution;

            if(part1 != null)
            {
                Console.WriteLine($"Day {DayNumber} Part 1 solution: {part1}");
            }

            if(part2 != null)
            {
                Console.WriteLine($"Day {DayNumber} Part 2 solution: {part2}");
            }
        }

        private static int? TryParseInt(string candidate)
        {
            if(int.TryParse(candidate, out var num))
            {
                return num;
            }

            return null;
        }

        protected abstract object Part1Solution { get; }
        protected abstract object Part2Solution { get; }
        protected abstract int DayNumber { get; }
    }
}