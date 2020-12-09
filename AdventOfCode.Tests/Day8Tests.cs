using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests
{
    public sealed class Day8Tests
    {
        [Fact]
        public void Part1Example()
        {
            var input = new[] {
                "nop +1",
                "acc +1",
                "jmp +4",
                "acc +3",
                "jmp -3",
                "acc -99",
                "acc +1",
                "jmp -4",
                "acc +6",
            };

            var instructions = input
                .Select(Day8.Instruction.Parse)
                .Select(x => new { Instruction = x, State = new Day8.InstructionState() })
                .ToArray();

            var state = new Day8.MachineState();

            var iterations = 0;
            while(!instructions[state.InstructionPointer].State.Visited)
            {
                iterations++;
                var cell = instructions[state.InstructionPointer];
                state = cell.Instruction.Execute(state);
                cell.State.Visited = true;
            }

            iterations.Should().Be(7);
            state.Accumulator.Should().Be(5);
        }

        [Fact]
        public void Part2Example()
        {
            var input = new[] {
                "nop +1",
                "acc +1",
                "jmp +4",
                "acc +3",
                "jmp -3",
                "acc -99",
                "acc +1",
                "jmp -4",
                "acc +6",
            };

            var instructions = input
                .Select(Day8.Instruction.Parse);

            new Day8().ComputePart2(instructions).Should().Be(8);
        }
    }
}
