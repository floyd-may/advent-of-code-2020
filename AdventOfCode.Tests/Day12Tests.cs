using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Part2State = AdventOfCode.Day12.Part2State;

namespace AdventOfCode.Tests
{
    public sealed class Day12Tests
    {
        [Fact]
        public void TurnLeft()
        {
            var st = new Day12.Part1State();

            st.UpdateFrom(Day12.Instruction.Parse("L90")).Dir.Should().Be(Day12.Direction.N);
        }

        public void Part1Example()
        {
            var instructions = new[] {
                "F10",
                "N3",
                "F7",
                "R90",
                "F11",
                }.Select(Day12.Instruction.Parse)
                .ToArray();

            var finalState = instructions.Aggregate(new Day12.Part1State(), (s, i) => s.UpdateFrom(i));

            var mhDist = Math.Abs(finalState.X) + Math.Abs(finalState.Y);

            mhDist.Should().Be(25);
        }

        public void Part2Example()
        {
            var instructions = new[] {
                "F10",
                "N3",
                "F7",
                "R90",
                "F11",
                }.Select(Day12.Instruction.Parse)
                .ToArray();

            var finalState = instructions.Aggregate(new Day12.Part2State(), (s, i) => s.UpdateFrom(i));

            var mhDist = Math.Abs(finalState.X) + Math.Abs(finalState.Y);

            mhDist.Should().Be(286);
        }

        [Fact]
        public void Part2FirstHandfulFromFile()
        {
            var instructionsWithExpectedState = new [] {
                ( "F92", new Part2State(920, 92, 10, 1) ),
                ( "R180", new Part2State(920, 92, -10, -1) ),
                ( "S1", new Part2State(920, 92, -10, -2) ),
                ( "F64", new Part2State(280, -36, -10, -2) ),
                ( "R90", new Part2State(280, -36, -2, 10) ),
                ( "L90", new Part2State(280, -36, -10, -2) ),
            };

            var instructions = instructionsWithExpectedState.Select(x => x.Item1).ToArray();

            var output = new List<(string, Part2State)>();

            for(int i = 0; i < instructions.Length; i++)
            {
                var state = instructions.Take(i + 1).Aggregate(new Part2State(), (s, i) => s.UpdateFrom(Day12.Instruction.Parse(i)));
                output.Add((instructionsWithExpectedState[i].Item1, state));
            }

            output.Should().BeEquivalentTo(instructionsWithExpectedState);
        }
    }

}
