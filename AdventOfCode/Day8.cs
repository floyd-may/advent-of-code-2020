using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day8 : SolverBase
    {
        protected override object Part1Solution => ComputePart1();

        protected override object Part2Solution => ComputePart2();

        public sealed class MachineState
        {
            public int Accumulator { get; private set; }
            public int InstructionPointer { get; private set; }

            public MachineState WithAccumulator(int accumulator)
            {
                return new MachineState
                {
                    Accumulator = accumulator,
                    InstructionPointer = InstructionPointer
                };
            }

            public MachineState WithInstructionPointer(int instructionPointer)
            {
                return new MachineState
                {
                    Accumulator = Accumulator,
                    InstructionPointer = instructionPointer
                };
            }
        }
        public abstract class Instruction
        {
            protected Instruction(int argument)
            {
                Argument = argument;
            }
            public int Argument { get; }
            public abstract MachineState Execute(MachineState state);

            public static Instruction Parse(string spec)
            {
                var parts = spec.Split(" ");

                var op = parts[0];
                var arg = int.Parse(parts[1].TrimStart('+'));

                switch(op)
                {
                    case "acc": return new Acc(arg);
                    case "jmp": return new Jmp(arg);
                    case "nop": return new Nop(arg);
                }

                throw new ArgumentException($"Unable to parse instruction {spec}");
            }

            public Instruction SwapJmpAndNop()
            {
                if(this is Jmp)
                {
                    return new Nop(Argument);
                }

                if(this is Nop)
                {
                    return new Jmp(Argument);
                }

                return this;
            }
        }

        public sealed class Acc : Instruction
        {
            public Acc(int argument) : base(argument) {}
            public override MachineState Execute(MachineState state)
            {
                return state
                    .WithAccumulator(state.Accumulator + Argument)
                    .WithInstructionPointer(state.InstructionPointer + 1);
            }
        }

        public sealed class Jmp : Instruction
        {
            public Jmp(int argument) : base(argument) {}
            public override MachineState Execute(MachineState state)
            {
                return state.WithInstructionPointer(state.InstructionPointer + Argument);
            }
        }

        public sealed class Nop : Instruction
        {
            public Nop(int argument) : base(argument) {}
            public override MachineState Execute(MachineState state)
            {
                return state.WithInstructionPointer(state.InstructionPointer + 1);
            }
        }

        public sealed class InstructionState
        {
            public bool Visited { get; set; }
        }

        public IEnumerable<Instruction> Instructions => LoadRawData()
            .Select(Instruction.Parse)
            .ToArray();

        private int ComputePart1()
        {
            var instructions = Instructions
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

            return state.Accumulator;
        }

        public int ComputePart2(IEnumerable<Instruction> input = null)
        {
            var instructions = (input ?? Instructions)
                .Select(x => new { Instruction = x, State = new Day8.InstructionState() })
                .ToArray();

            var state = new Day8.MachineState();

            var visitedInstructions = new List<int>();
            while(!instructions[state.InstructionPointer].State.Visited)
            {
                visitedInstructions.Add(state.InstructionPointer);
                var cell = instructions[state.InstructionPointer];
                state = cell.Instruction.Execute(state);
                cell.State.Visited = true;
            }

            var instructionsToSwap = visitedInstructions
                .Where(x => instructions[x].Instruction is Jmp || instructions[x].Instruction is Nop);

            var instructionSets = instructionsToSwap
                .Select(x => {
                    var newSet = instructions.Select(x => x.Instruction).ToArray();

                    newSet[x] = newSet[x].SwapJmpAndNop();

                    return newSet;
                });

            return instructionSets
                .Select(AttemptHalt)
                .First(x => x.HasValue)
                .Value;
        }

        private int? AttemptHalt(IEnumerable<Instruction> instructions)
        {
            var cells = instructions
                .Select(x => new { Instruction = x, State = new Day8.InstructionState() })
                .ToArray();

            var state = new Day8.MachineState();
            while(state.InstructionPointer < cells.Length && !cells[state.InstructionPointer].State.Visited)
            {
                var cell = cells[state.InstructionPointer];
                state = cell.Instruction.Execute(state);
                cell.State.Visited = true;
            }

            if(state.InstructionPointer == cells.Count())
            {
                return state.Accumulator;
            }

            return null;
        }

    }
}