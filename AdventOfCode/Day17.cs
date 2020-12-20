using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day17 : SolverBase
    {
        protected override object Part1Solution => SolvePart1();

        protected override object Part2Solution => SolvePart2();

        public long SolvePart1(IEnumerable<string> input = null)
        {
            input = input ?? LoadRawData() ?? new[] {
                ".#.",
                "..#",
                "###",
            };
            var currentState = new HashSet<Coord>();

            foreach(var (line, y) in input.Select((x, idx) => (x, idx)))
            {
                var xCoords = line
                    .Select((c, idx) => (c, idx))
                    .Where(c => c.c == '#')
                    ;

                foreach(var x in xCoords)
                {
                    currentState.Add(new Coord(x.idx, y, 0));
                }
            }

            System.Console.WriteLine($"Initial count = {currentState.Count}");

            foreach(var cycleCount in Enumerable.Range(0, 6))
            {
                var newState = new HashSet<Coord>();

                foreach(var coord in currentState.SelectMany(c => c.GetNeighbors()).Distinct())
                {
                    var activeNeighbors = coord.GetNeighbors()
                        .Where(c => !c.Equals(coord))
                        .Where(c => currentState.Contains(c))
                        .Count();

                    var isActive = currentState.Contains(coord);

                    if(isActive)
                    {
                        isActive = activeNeighbors == 2 || activeNeighbors == 3;
                    }
                    else
                    {
                        isActive = activeNeighbors == 3;
                    }

                    if(isActive)
                    {
                        newState.Add(coord);
                    }
                }

                currentState = newState;
            }

            return currentState.Count;
        }
        public long SolvePart2(IEnumerable<string> input = null)
        {
            input = input ?? LoadRawData() ?? new[] {
                ".#.",
                "..#",
                "###",
            };
            var currentState = new HashSet<Coord2>();

            foreach(var (line, y) in input.Select((x, idx) => (x, idx)))
            {
                var xCoords = line
                    .Select((c, idx) => (c, idx))
                    .Where(c => c.c == '#')
                    ;

                foreach(var x in xCoords)
                {
                    currentState.Add(new Coord2(x.idx, y, 0, 0));
                }
            }

            System.Console.WriteLine($"Initial count = {currentState.Count}");

            foreach(var cycleCount in Enumerable.Range(0, 6))
            {
                var newState = new HashSet<Coord2>();

                foreach(var coord in currentState.SelectMany(c => c.GetNeighbors()).Distinct())
                {
                    var activeNeighbors = coord.GetNeighbors()
                        .Where(c => !c.Equals(coord))
                        .Where(c => currentState.Contains(c))
                        .Count();

                    var isActive = currentState.Contains(coord);

                    if(isActive)
                    {
                        isActive = activeNeighbors == 2 || activeNeighbors == 3;
                    }
                    else
                    {
                        isActive = activeNeighbors == 3;
                    }

                    if(isActive)
                    {
                        newState.Add(coord);
                    }
                }

                currentState = newState;
                System.Console.WriteLine($"Current count = {currentState.Count}");
            }

            return currentState.Count;
        }

        private sealed class Coord : IEquatable<Coord>
        {
            public Coord(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }
            public int X { get; }
            public int Y { get; }
            public int Z { get; }

            public override bool Equals(object obj)
            {
                return Equals(obj as Coord);
            }

            public bool Equals([AllowNull] Coord other)
            {
                if(other == null)
                {
                    return false;
                }

                return X == other.X
                    && Y == other.Y
                    && Z == other.Z;
            }

            public override int GetHashCode()
            {
                return X * 41 + Y * 17 + Z;
            }

            public override string ToString()
            {
                return $"({X},{Y},{Z})";
            }

            public IEnumerable<Coord> GetNeighbors()
            {
                return Enumerable.Range(X - 1, 3)
                    .SelectMany(newx => Enumerable.Range(Y - 1, 3), (nx, ny) => new { nx, ny })
                    .SelectMany(nxy => Enumerable.Range(Z - 1, 3), (nxy, nz) => new Coord(nxy.nx, nxy.ny, nz))
                    .ToArray();
            }
        }
        private sealed class Coord2 : IEquatable<Coord2>
        {
            public Coord2(int x, int y, int z, int w)
            {
                X = x;
                Y = y;
                Z = z;
                W = w;
            }
            public int X { get; }
            public int Y { get; }
            public int Z { get; }
            public int W { get; }

            public override bool Equals(object obj)
            {
                return Equals(obj as Coord2);
            }

            public bool Equals([AllowNull] Coord2 other)
            {
                if(other == null)
                {
                    return false;
                }

                return X == other.X
                    && Y == other.Y
                    && Z == other.Z
                    && W == other.W;
            }

            public override int GetHashCode()
            {
                return X * 41 + Y * 17 + Z * 29 + W;
            }

            public override string ToString()
            {
                return $"({X},{Y},{Z})";
            }

            public IEnumerable<Coord2> GetNeighbors()
            {
                return Enumerable.Range(X - 1, 3)
                    .SelectMany(newx => Enumerable.Range(Y - 1, 3), (nx, ny) => new { nx, ny })
                    .SelectMany(newx => Enumerable.Range(Z - 1, 3), (nxy, nz) => new { nxy.nx, nxy.ny, nz })
                    .SelectMany(nxyz => Enumerable.Range(W - 1, 3), (nxyz, nw) => new Coord2(nxyz.nx, nxyz.ny, nxyz.nz, nw))
                    .ToArray();
            }
        }
    }

}