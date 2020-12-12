using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day11Tests
    {
        [Fact]
        public void Part1Example()
        {
            var input = new[] {
                "L.LL.LL.LL",
                "LLLLLLL.LL",
                "L.L.L..L..",
                "LLLL.LL.LL",
                "L.LL.LL.LL",
                "L.LLLLL.LL",
                "..L.L.....",
                "LLLLLLLLLL",
                "L.LLLLLL.L",
                "L.LLLLL.LL",
            };

            var testee = new Day11();

            testee.ComputePart1Solution(input).Should().Be(37);
        }
    }
}
