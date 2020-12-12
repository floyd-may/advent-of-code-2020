using System;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day12 : SolverBase
    {
        protected override object Part1Solution => ComputePart1Solution();

        protected override object Part2Solution => ComputePart2Solution();

        private int ComputePart1Solution()
        {
            var instructions = LoadRawData()
                .Select(Instruction.Parse);

            var finalState = instructions.Aggregate(new Part1State(), (s, i) => s.UpdateFrom(i));

            return Math.Abs(finalState.X) + Math.Abs(finalState.Y);
        }
        private int ComputePart2Solution()
        {
            var instructions = LoadRawData()
                .Select(Instruction.Parse);

            var finalState = instructions.Aggregate(new Part2State(), (s, i) => s.UpdateFrom(i));

            return Math.Abs(finalState.X) + Math.Abs(finalState.Y);
        }

        public sealed class Instruction
        {
            public char Type { get; }

            public int Count { get; }

            public static Instruction Parse(string input)
            {
                var type = input[0];

                var count = int.Parse(input.Substring(1));

                return new Instruction(type, count);
            }

            public Instruction(char type, int count)
            {
                Type = type;
                Count = count;
            }

            public int TurnCount => (Count / 90) % 4;
        }

        public enum Direction
        {
            N = 0,
            E = 1,
            S = 2,
            W = 3,
        }

        public sealed class Part1State
        {
            public Part1State()
            {
                X = 0;
                Y = 0;
                Dir = Direction.E;
            }
            private Part1State(int x, int y, Direction orientation)
            {
                X = x;
                Y = y;
                Dir = orientation;
            }

            public int X { get; private set; }
            public int Y { get; private set; }
            public Direction Dir { get; private set; }

            public Part1State UpdateFrom(Instruction instr)
            {
                switch(instr.Type)
                {
                    case 'N':
                        return this.With(y: Y + instr.Count);
                    case 'S':
                        return this.With(y: Y - instr.Count);
                    case 'E':
                        return this.With(x: X + instr.Count);
                    case 'W':
                        return this.With(x: X - instr.Count);
                    case 'R':
                        return this.With(dir: (Direction)(((int)this.Dir + instr.TurnCount) % 4));
                    case 'L':
                        return this.With(dir: GetDirectionLeftBy(instr.TurnCount));
                    case 'F':
                        return UpdateFrom(new Instruction(this.Dir.ToString()[0], instr.Count));
                }

                throw new Exception($"wat! type: {instr.Type}, count: {instr.Count}, dir: {this.Dir}");
            }

            private Direction GetDirectionLeftBy(int turnCount)
            {
                var dirAsInt = (((int)this.Dir) + 4 - turnCount) % 4;

                return (Direction)dirAsInt;
            }

            public Part1State With(int? x = null, int? y = null, Direction? dir = null)
            {
                return new Part1State(x ?? this.X, y ?? this.Y, dir ?? this.Dir);
            }
        }

        public sealed class Part2State
        {
            public Part2State()
            {
                X = 0;
                Y = 0;
                WaypointX = 10;
                WaypointY = 1;
            }
            public Part2State(int x, int y, int waypointX, int waypointY)
            {
                X = x;
                Y = y;
                WaypointX = waypointX;
                WaypointY = waypointY;
            }

            public int X { get; private set; }
            public int Y { get; private set; }
            public int WaypointX { get; private set; }
            public int WaypointY { get; private set; }

            public Part2State UpdateFrom(Instruction instr)
            {
                switch(instr.Type)
                {
                    case 'N':
                        return this.With(waypointY: WaypointY + instr.Count);
                    case 'S':
                        return this.With(waypointY: WaypointY - instr.Count);
                    case 'E':
                        return this.With(waypointX: WaypointX + instr.Count);
                    case 'W':
                        return this.With(waypointX: WaypointX - instr.Count);
                    case 'R':
                        return this.RotateWaypoint(instr.TurnCount);
                    case 'L':
                        return this.RotateWaypoint(-instr.TurnCount);
                    case 'F':
                        return GoToWaypoint(instr.Count);
                }

                throw new Exception($"wat! type: {instr.Type}, count: {instr.Count}");
            }

            private Part2State RotateWaypoint(int turnCount)
            {
                var normalizedTurnCount = (turnCount + 4) % 4;
                var dir = (Direction)(normalizedTurnCount);

                switch(dir)
                {
                    case Direction.N:
                        return this;
                    case Direction.E:
                        return this.With(waypointX: this.WaypointY, waypointY: -this.WaypointX);
                    case Direction.W:
                        return this.With(waypointX: -this.WaypointY, waypointY: this.WaypointX);
                    case Direction.S:
                        return this.With(waypointX: -this.WaypointX, waypointY: -this.WaypointY);
                }

                throw new Exception("wat! wonky waypoint rotate");
            }

            private Part2State GoToWaypoint(int times)
            {
                var xOffset = this.WaypointX * times;
                var yOffset = this.WaypointY * times;

                if(this.WaypointX < 0)
                {
                    xOffset = -Math.Abs(xOffset);
                }
                if(this.WaypointY < 0)
                {
                    yOffset = -Math.Abs(yOffset);
                }

                return this.With(x: this.X + xOffset, y: this.Y + yOffset);
            }

            public Part2State With(int? x = null, int? y = null, int? waypointX = null, int? waypointY = null)
            {
                return new Part2State(x ?? this.X, y ?? this.Y, waypointX ?? this.WaypointX, waypointY ?? this.WaypointY);
            }
        }
    }
}