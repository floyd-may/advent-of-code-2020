using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day15Tests
    {
        [Fact]
        public void Part1Example1FirstFew()
        {
            var input = new[] {
                0, 3, 6
            };

            var testee = new Day15();

            testee.RunPart1Game(input).Take(8).Should().BeEquivalentTo(new[] {
                0, 3, 6, 0, 3, 3, 1, 0
            });
        }

        [Fact]
        public void Part1Example1()
        {
            var input = new[] {
                0, 3, 6
            };

            var testee = new Day15();

            testee.ComputePart1(input).Should().Be(436);
        }
    }
}
