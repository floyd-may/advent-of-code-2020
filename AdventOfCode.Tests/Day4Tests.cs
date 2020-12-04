using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day4Tests
    {
        [Fact]
        public void SampleValidate()
        {
            var record = new[] {
                "iyr:2020",
                "byr:1968",
                "ecl:gry",
                "eyr:2030",
                "hcl:#1976b0",
                "cid:127",
                "pid:701862616",
                "hgt:161cm",
            };

            var testee = new Day4();

            record.All(x => testee.ValidateField(x)).Should().BeTrue();
        }
    }
}
