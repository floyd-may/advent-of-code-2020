using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintDay4Solution();
        }

        private static void PrintDay4Solution()
        {
            var day4 = new Day4();

            var recordsWithRequiredFields = day4.RecordsWithRequiredFields
                .Count();

            Console.WriteLine($"Day 4, part 1 solution: {recordsWithRequiredFields}");

            var validRecords = day4.RecordsWithRequiredFields
                .Select(x => x.All(f => day4.ValidateField(f)))
                .Where(x => x)
                .Count();

            Console.WriteLine($"Day 4, part 2 solution: {validRecords}");
        }

        private static void PrintDay3Solution()
        {
            var day3 = new Day3();
            var input = day3.LoadRawData();
            var model = Day3.ForestModel.Parse(input);

            var coords = Enumerable.Range(0, model.Height)
                .Select(y => (y * 3, y));

            var treeCount = coords
                .Select(coord => model.HasTree(coord))
                .Where(x => x)
                .Count();

            Console.WriteLine($"Day 3, part 1 solution: {treeCount}");

            var slopes = new[] {
                day3.GenerateSlope(1, 1, model.Height),
                day3.GenerateSlope(3, 1, model.Height),
                day3.GenerateSlope(5, 1, model.Height),
                day3.GenerateSlope(7, 1, model.Height),
                day3.GenerateSlope(1, 2, model.Height),
            };

            var treeCounts = slopes
                .Select(x => (long)day3.TreeCountForSlope(x, model));

            var part2Solution = treeCounts.Aggregate((a, b) => a * b);

            Console.WriteLine($"Day 3, part 2 solution: {part2Solution}");
        }

        private static void PrintDay2Solution()
        {
            var day2 = new Day2();
            var input = day2.LoadRawData()
                .Select(x => Day2.PasswordRecord.Parse(x))
                .ToArray();

            var part1Solution = input
                .Where(x => x.Part1Valid)
                .Count();

            Console.WriteLine($"Day 2, part 1 solution: {part1Solution}");
            var part2Solution = input
                .Where(x => x.Part2Valid)
                .Count();

            Console.WriteLine($"Day 2, part 2 solution: {part2Solution}");
        }

        private static void PrintDay1Solution()
        {
            var day1 = new Day1();
            var input = day1.LoadIntData();
            var (part1Left, part1Right) = day1.Part1(input);

            Console.WriteLine($"Day 1, part 1 solution: {part1Left * part1Right}");

            var (p2left, p2mid, p2right) = day1.Part2(input);

            Console.WriteLine($"Day 1, part 2 solution: {p2left * p2mid * p2right}");
        }
    }
}
