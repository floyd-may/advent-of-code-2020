using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day15 : SolverBase
    {
        protected override object Part1Solution => ComputePart1(new[] { 11,18,0,20,1,7,16 });

        protected override object Part2Solution => ComputePart2(new[] { 11,18,0,20,1,7,16 });

        public int ComputePart1(IEnumerable<int> input)
        {
            // turns are one-based
            return RunPart1Game(input).ElementAt(2019);
        }

        public int ComputePart2(IEnumerable<int> input)
        {
            // turns are one-based
            return RunPart1Game(input).ElementAt(30000000 - 1);
        }

        public IEnumerable<int> RunPart1Game(IEnumerable<int> startingNumbers)
        {
            var dict = new Dictionary<int, Part1GameState>();
            var lastSeen = 0;
            var pos = 1;

            foreach(var n in startingNumbers)
            {
                yield return n;

                lastSeen = n;
                dict[lastSeen] = new Part1GameState(pos);

                pos++;
            }

            while(true)
            {
                var lastSeenState = dict[lastSeen];

                int curValue;
                if(!lastSeenState.SeenBefore)
                {
                    curValue = 0;
                }
                else {
                    curValue = lastSeenState.Delta;
                }

                yield return curValue;

                if(dict.TryGetValue(curValue, out var curState))
                {
                    dict[curValue] = curState.SeenAgain(pos);
                }
                else
                {
                    dict[curValue] = new Part1GameState(pos);
                }

                lastSeen = curValue;
                pos++;
            }
        }

        public class Part1GameState
        {
            public Part1GameState(int seenAtTurn)
            {
                SeenBefore = false;
                SeenAtTurn = seenAtTurn;
            }
            private Part1GameState(int seenAtTurn, bool seenBefore, int delta)
            {
                SeenAtTurn = seenAtTurn;
                SeenBefore = seenBefore;
                Delta = delta;
            }

            public Part1GameState SeenAgain(int seenAtTurn)
            {
                return new Part1GameState(seenAtTurn, true, seenAtTurn - SeenAtTurn);
            }

            public bool SeenBefore { get; }
            public int SeenAtTurn { get; }
            public int Delta { get; }
        }
    }
}