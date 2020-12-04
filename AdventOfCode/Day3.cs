using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day3 : ResourceLoader
    {
        public sealed class ForestModel
        {
            private List<List<bool>> _lines;
            public static ForestModel Parse(IEnumerable<string> input)
            {
                var lines = input
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.AsEnumerable().Select(x => x == '#' ? true : false).ToList())
                    .ToList();

                return new ForestModel(lines);
            }

            public int Height => _lines.Count;
            public int Width => _lines[0].Count;

            private ForestModel(List<List<bool>> lines)
            {
                _lines = lines;
            }

            public bool HasTree((int, int) coord)
            {
                var (x, y) = coord;

                return _lines[y][x % Width];
            }
        }

        public IEnumerable<(int, int)> GenerateSlope(int xRun, int yRun, int yLimit)
        {
            var x = 0;
            var y = 0;
            while(y < yLimit)
            {
                yield return (x, y);

                x += xRun;
                y += yRun;
            }
        }

        public int TreeCountForSlope(IEnumerable<(int, int)> coords, ForestModel model)
        {
            return coords
                .Select(coord => model.HasTree(coord))
                .Where(x => x)
                .Count();
        }
    }
}