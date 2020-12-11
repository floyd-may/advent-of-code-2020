using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day10Tests
    {
        [Fact]
        public void Part1Example1()
        {
            var testee = new Day10();

            var actual = testee.ComputePart1Solution(new[] {
                16, 10, 15, 5, 1, 11, 7, 19, 6, 12, 4,
            });

            actual.Should().Be(35);
        }

        [Fact]
        public void Part2Example1()
        {
            var testee = new Day10();

            var actual = testee.ComputeFullPart2Solution(new[] {
                16, 10, 15, 5, 1, 11, 7, 19, 6, 12, 4,
            });

            actual.Should().Be(8);
        }

        [Fact]
        public void Part2Example2()
        {
            var testee = new Day10();

            var actual = testee.ComputeFullPart2Solution(new[] {
                28, 33, 18, 42, 31, 14,
                46, 20, 48, 47, 24, 23,
                49, 45, 19, 38, 39, 11,
                1, 32, 25, 35, 8, 17,
                7, 9, 4, 2, 34, 10, 3,
            });

            actual.Should().Be(19208);
        }

        [Theory]
        [InlineData(2, 1, 2)]
        [InlineData(4, 1, 2, 3)]
        [InlineData(8, 1, 4, 5, 6, 7, 10, 11, 12, 15, 16, 19)]
        public void ComputePossibleSolutionCases(int expected, params int[] values)
        {
            var testee = new Day10();

            testee.ComputePart2Solutions(values).Should().Be(expected);
        }
    }
}
