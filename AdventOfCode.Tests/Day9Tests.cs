using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day9Tests
    {
        [Fact]
        public void Part2FromSample()
        {
            var input = new [] {
                35, 20, 15, 25, 47, 40, 62, 55, 65, 95,
                102, 117, 150, 182, 127, 219, 299, 277, 309, 576,
            };

            var testee = new Day9();

            testee.ComputePart2Solution(input, preambleLength: 5).Should().Be(62);
        }

    }
}
