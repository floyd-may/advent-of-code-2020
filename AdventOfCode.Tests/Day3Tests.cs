using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day3Tests
    {
        public IEnumerable<string> ExampleInput => new[] {
            // width: 11
            "..##.......", // 0
            "#...#...#..", // 1
            ".#....#..#.", // 2
            "..#.#...#.#", // 3
            ".#...##..#.", // 4
            "..#.##.....", // 5
            ".#.#.#....#", // 6
            ".#........#", // 7
            "#.##...#...", // 8
            "#...##....#", // 9
            ".#..#...#.#", // 10
        };

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(2, 0, true)]
        [InlineData(0, 9, true)]
        [InlineData(9, 10, false)]
        [InlineData(20, 10, false)]
        public void ValidateCoordinates(int x, int y, bool expected)
        {
            var model = Day3.ForestModel.Parse(ExampleInput);

            model.HasTree((x, y)).Should().Be(expected);
        }

        [Fact]
        public void GeneratesProperCoordinates()
        {
            var day3 = new Day3();

        }

    }
}
