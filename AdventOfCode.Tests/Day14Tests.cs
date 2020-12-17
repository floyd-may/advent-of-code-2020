using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day14Tests
    {
        [Fact]
        public void Part1Example1()
        {
            var input = new[] {
                "mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X",
                "mem[8] = 11",
                "mem[7] = 101",
                "mem[8] = 0",
            };

            var testee = new Day14();

            testee.ComputePart1Solution(input).Should().Be(165);

        }

        [Fact]
        public void Part2Example1()
        {
            var input = new[] {
                "mask = 000000000000000000000000000000X1001X",
                "mem[42] = 100",
                "mask = 00000000000000000000000000000000X0XX",
                "mem[26] = 1",
            };

            var testee = new Day14();

            var machine = testee.RunPart2(input);
            machine.GetSumOfMemoryLocations().Should().Be(208);

            machine.GetAddresses()
                .Should().BeEquivalentTo(new[] {
                    16,17,18,19,24,25,26,27,58,59
                });
        }

        [Fact]
        public void Part2Contrived1()
        {
            var input = new[] {
                "mask = 000000000000000000000000000000X1001X",
                "mem[42] = 100",
                "mask = 000000000000000000X00X00000X0000X0XX",
                "mem[26] = 1",
            };

            var testee = new Day14();

            testee.ComputePart2Solution(input).Should().Be(264);
        }

        //[Fact(Skip = "dangit i have a bug somewhere")]
        [Fact]
        public void Part2Actual()
        {
            var testee = new Day14();

            testee.ComputePart2Solution().Should().BeLessThan(3440662844572L);
        }

        [Fact]
        public void ComputeFloatingValsA()
        {
            var ranks = new[] { 0 };

            var expected = new[] { 0, 1 };

            AssertFloatingValues(ranks, expected);
        }

        [Fact]
        public void ComputeFloatingValsB()
        {
            var ranks = new[] { 7, 5, 3 };

            var expected = new[] { 0, 8, 32, 40, 128, 136, 160, 168 };

            AssertFloatingValues(ranks, expected);
        }

        [Fact]
        public void ComputeFloatingValsC()
        {
            var ranks = new[] { 10, 1 };

            var expected = new[] { 0, 1024, 2, 1026 };

            AssertFloatingValues(ranks, expected);
        }

        [Fact]
        public void ComputeFloatingValsD()
        {
            var ranks = new[] { 10, 9, 8, 3, 2, 1 };

            var expected = Enumerable.Range(0, 8)
                .SelectMany(
                    x => Enumerable.Range(0, 8),
                    (upper, lower) => upper << 8 | lower << 1
                    )
                    .ToArray();

            AssertFloatingValues(ranks, expected);
        }

        private static void AssertFloatingValues(int[] ranks, int[] expected)
        {
            Day14.Part2Machine.GetFloatingValues(0, ranks, 0)
                .Should().BeEquivalentTo(expected);
        }
    }
}
