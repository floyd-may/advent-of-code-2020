using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day1Tests
    {
        private IEnumerable<int> ExampleData => new [] {
            1721,
            979,
            366,
            299,
            675,
            1456,
        };

        [Fact]
        public void Part1Example()
        {
            var testee = new Day1();

            var (first, second) = testee.Part1(ExampleData);

            new[] { first, second }
                .Should()
                .BeEquivalentTo(new[] { 1721, 299 });
        }

        [Fact]
        public void Part2Example()
        {
            var testee = new Day1();

            var (first, second, third) = testee.Part2(ExampleData);

            new[] { first, second, third }
                .Should()
                .BeEquivalentTo(new[] { 979, 366, 675 });
        }
    }
}
