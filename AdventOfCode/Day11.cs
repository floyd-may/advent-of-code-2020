using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    public sealed class Day11 : SolverBase
    {
        protected override object Part1Solution => ComputePart1Solution();

        protected override object Part2Solution => ComputePart2Solution();

        public int ComputePart1Solution(IEnumerable<string> lines = null)
        {
            var grid = InitGrid(lines);

            bool changed = true;

            while (changed)
            {
                var (newGrid, newChanged) = CycleGrid(grid);

                changed = newChanged;
                grid = newGrid;
            }

            var coords = Enumerable.Range(0, grid.GetLength(0))
                .SelectMany(x => Enumerable.Range(0, grid.GetLength(1)), (x, y) => new { x, y })
                ;

            return coords
                .Select(coord => grid[coord.x, coord.y].State)
                .Where(x => x == GridState.OccupiedSeat)
                .Count();
        }

        public int ComputePart2Solution(IEnumerable<string> lines = null)
        {
            var grid = InitGrid(lines);

            bool changed = true;

            while (changed)
            {
                var (newGrid, newChanged) = CycleGrid(grid, part2: true);

                changed = newChanged;
                grid = newGrid;
            }

            var coords = Enumerable.Range(0, grid.GetLength(0))
                .SelectMany(x => Enumerable.Range(0, grid.GetLength(1)), (x, y) => new { x, y })
                ;

            return coords
                .Select(coord => grid[coord.x, coord.y].State)
                .Where(x => x == GridState.OccupiedSeat)
                .Count();
        }

        public GridCell[,] InitGrid(IEnumerable<string> lines)
        {
            var data = (lines ?? LoadRawData()).ToArray();

            var width = data[0].Length;
            var height = data.Length;
            var grid = new GridCell[width, height];
            foreach (var (line, y) in data.Select((line, y) => (line, y)))
            {
                foreach (var (cell, x) in line.Select((cell, x) => (cell, x)))
                {
                    var cellValue = cell == '.' ? GridState.Floor : GridState.EmptySeat;

                    grid[x, y] = new GridCell(x, y, cellValue, grid);
                }
            }

            return grid;
        }

        public (GridCell[,], bool) CycleGrid(GridCell[,] grid, bool part2 = false)
        {
            var changed = false;
            var width = grid.GetLength(0);
            var height = grid.GetLength(1);

            var coords = Enumerable.Range(0, width)
                .SelectMany(x => Enumerable.Range(0, height), (x, y) => new { x, y })
                ;
            var newGrid = new GridCell[width, height];
            changed = false;
            foreach(var coord in coords)
            {
                if(part2)
                {
                    changed = grid[coord.x, coord.y].TogglePart2(newGrid) || changed;
                }
                else
                {
                    changed = grid[coord.x, coord.y].TogglePart1(newGrid) || changed;
                }
            }

            return (newGrid, changed);
        }

        public string PrintGrid(GridCell[,] grid)
        {
            var width = grid.GetLength(0);
            var height = grid.GetLength(1);

            var sb = new StringBuilder();
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    var state = grid[x, y].State;
                    var val = state == GridState.Floor
                        ? "."
                        : state == GridState.EmptySeat
                        ? "L"
                        : "#";
                    sb.Append(val);
                }

                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }

        public class GridCell
        {
            public GridCell(int x, int y, GridState state, GridCell[,] grid)
                : this(x, y, state, grid, null)
            {

            }

            private GridCell(int x, int y, GridState state, GridCell[,] grid, (int, int)[] pt2neighbors)
            {
                X = x;
                Y = y;
                Grid = grid;
                State = state;
                _pt2neighbors = pt2neighbors;
            }

            public int X { get; }
            public int Y { get; }
            public GridCell[,] Grid { get; }
            public int Width => Grid.GetLength(0);
            public int Height => Grid.GetLength(1);
            public GridState State { get; }

            private readonly (int, int)[] _pt2neighbors;


            public bool TogglePart1(GridCell[,] newGrid)
            {
                var adjacentCellStates = Enumerable.Range(X - 1, 3)
                    .SelectMany(x => Enumerable.Range(Y - 1, 3), (x, y) => new {x, y})
                    .Where(coord => coord.x >= 0 && coord.x < Width) // clamp X
                    .Where(coord => coord.y >= 0 && coord.y < Height) // clamp Y
                    .Where(coord => !(coord.x == X && coord.y == Y)) // exclude self
                    .Select(coord => Grid[coord.x, coord.y].State)
                    .ToArray();

                if(State == GridState.EmptySeat && !adjacentCellStates.Contains(GridState.OccupiedSeat))
                {
                    newGrid[X, Y] = new GridCell(X, Y, GridState.OccupiedSeat, newGrid);
                    return true;
                }
                if(State == GridState.OccupiedSeat && adjacentCellStates.Where(x => x == GridState.OccupiedSeat).Count() >= 4)
                {
                    newGrid[X, Y] = new GridCell(X, Y, GridState.EmptySeat, newGrid);
                    return true;
                }

                CloneTo(newGrid);
                return false;
            }

            public bool TogglePart2(GridCell[,] newGrid)
            {
                var neighbors = (_pt2neighbors ?? GetNeighborCoords());
                var visibleCellStates = (_pt2neighbors ?? GetNeighborCoords())
                    .Select(coord => Grid[coord.Item1, coord.Item2].State)
                    .ToArray();

                if(State == GridState.EmptySeat && !visibleCellStates.Contains(GridState.OccupiedSeat))
                {
                    newGrid[X, Y] = new GridCell(X, Y, GridState.OccupiedSeat, newGrid, neighbors);
                    return true;
                }
                if(State == GridState.OccupiedSeat && visibleCellStates.Where(x => x == GridState.OccupiedSeat).Count() >= 5)
                {
                    newGrid[X, Y] = new GridCell(X, Y, GridState.EmptySeat, newGrid, neighbors);
                    return true;
                }

                CloneTo(newGrid);
                return false;
            }

            private void CloneTo(GridCell[,] newGrid, (int, int)[] neighbors = null)
            {
                newGrid[X, Y] = new GridCell(X, Y, State, newGrid, neighbors);
            }

            private (int, int)[] GetNeighborCoords()
            {
                var vectors = new[] {
                    GetLineOfSight(X + 1, Y, 1, 0), // right
                    GetLineOfSight(X - 1, Y, -1, 0), // left
                    GetLineOfSight(X, Y + 1, 0, 1), // down
                    GetLineOfSight(X, Y - 1, 0, -1), // up
                    GetLineOfSight(X + 1, Y + 1, 1, 1), // down-right
                    GetLineOfSight(X + 1, Y - 1, 1, -1), // up-right
                    GetLineOfSight(X - 1, Y - 1, -1, -1), // up-left
                    GetLineOfSight(X - 1, Y + 1, -1, 1), // down-left
                };

                var cells = vectors
                    .Select(vector => vector.Select(coord => Grid[coord.Item1, coord.Item2]).FirstOrDefault(cell => cell.State != GridState.Floor))
                    .Where(x => x != null);

                return cells.Select(cell => (cell.X, cell.Y)).ToArray();
            }

            private IEnumerable<(int, int)> GetLineOfSight(int startx, int starty, int stepx, int stepy)
            {
                var x = startx;
                var y = starty;
                while(x >= 0 && x < Width && y >= 0 && y < Height)
                {
                    yield return (x, y);

                    x += stepx;
                    y += stepy;
                }
            }

        }

        public enum GridState
        {
            EmptySeat,
            OccupiedSeat,
            Floor,
        }
    }
}