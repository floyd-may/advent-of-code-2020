using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day2Tests
    {
        [Theory]
        [InlineData("1-3 a: abcde", 1, 3, 'a', "abcde")]
        [InlineData("1-3 b: cdefg", 1, 3, 'b', "cdefg")]
        [InlineData("2-9 c: ccccccccc", 2, 9, 'c', "ccccccccc")]
        public void ParsePasswordLine(string input, int min, int max, char requiredChar, string password)
        {
            var actual = Day2.PasswordRecord.Parse(input);

            actual.Should().BeEquivalentTo(new Day2.PasswordRecord {
                Min = min,
                Max = max,
                RequiredChar = requiredChar,
                Password = password,
            });
        }

        private IEnumerable<Day2.PasswordRecord> Input => new [] {
            Day2.PasswordRecord.Parse("1-3 a: abcde"),
            Day2.PasswordRecord.Parse("1-3 b: cdefg"),
            Day2.PasswordRecord.Parse("2-9 c: ccccccccc"),
        };

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        [InlineData(2, true)]
        public void Part1ValidPassword(int idx, bool expected)
        {
            var testee = Input.ElementAt(idx);

            testee.Part1Valid.Should().Be(expected);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        public void Part2ValidPassword(int idx, bool expected)
        {
            var testee = Input.ElementAt(idx);

            testee.Part2Valid.Should().Be(expected);
        }
    }
}
