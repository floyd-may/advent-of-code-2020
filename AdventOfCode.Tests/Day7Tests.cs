using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day7Tests
    {
        [Fact]
        public void BaseCase()
        {
            var input = new string[] {
                "stupid bags contain 9 shiny gold bags."
            };

            var model = Day7.RuleModel.Parse(input);

            model.PathsToShinyGoldBag.Should().Be(1);
        }

        [Fact]
        public void NoCases()
        {
            var input = new string[] {
                "stupid bags contain 9 shiny potato bags."
            };

            var model = Day7.RuleModel.Parse(input);

            model.PathsToShinyGoldBag.Should().Be(0);
        }

        [Fact]
        public void TwoHops()
        {
            var input = new string[] {
                "smart bags contain 9 stupid bags.",
                "stupid bags contain 9 shiny gold bags.",
            };

            var model = Day7.RuleModel.Parse(input);

            model.PathsToShinyGoldBag.Should().Be(2);
        }

        [Fact]
        public void ExampleModelPathsToShinyGold()
        {
            var input = new[] {
                "light red bags contain 1 bright white bag, 2 muted yellow bags.",
                "dark orange bags contain 3 bright white bags, 4 muted yellow bags.",
                "bright white bags contain 1 shiny gold bag.",
                "muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.",
                "shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.",
                "dark olive bags contain 3 faded blue bags, 4 dotted black bags.",
                "vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.",
                "faded blue bags contain no other bags.",
                "dotted black bags contain no other bags.",
            };

            var model = Day7.RuleModel.Parse(input);

            model.PathsToShinyGoldBag.Should().Be(4);
        }

        [Fact]
        public void ExampleModelRequiredBags()
        {
            var input = new[] {
                "light red bags contain 1 bright white bag, 2 muted yellow bags.",
                "dark orange bags contain 3 bright white bags, 4 muted yellow bags.",
                "bright white bags contain 1 shiny gold bag.",
                "muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.",
                "shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.",
                "dark olive bags contain 3 faded blue bags, 4 dotted black bags.",
                "vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.",
                "faded blue bags contain no other bags.",
                "dotted black bags contain no other bags.",
            };

            var model = Day7.RuleModel.Parse(input);

            model._nodes["dark olive"].RequiredBagCount.Should().Be(7);
            model._nodes["vibrant plum"].RequiredBagCount.Should().Be(11);
            model.RequiredBagsInsideShinyGold.Should().Be(32);
        }
    }
}
