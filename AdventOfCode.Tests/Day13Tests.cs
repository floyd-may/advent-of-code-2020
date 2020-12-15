using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day13Tests
    {
        [Fact]
        public void Example1()
        {
            var input = new[] {
                "939",
                "7,13,x,x,59,x,31,19",
            };

            var testee = new Day13();

            var result = testee.ComputePart1(input);

            result.Should().Be(295);
        }

        [Theory]
        [InlineData("17,x,13,19", 3417L)]
        [InlineData("67,7,59,61", 754018L)]
        [InlineData("67,x,7,59,61", 779210L)]
        [InlineData("67,7,x,59,61", 1261476L)]
        [InlineData("1789,37,47,1889", 1202161486L)]
        public void Example2(string input, long expected)
        {
            var testee = new Day13();

            var result = testee.ComputePart2(new[] { "derp", input });

            result.Should().Be(expected);
        }
    }

}
