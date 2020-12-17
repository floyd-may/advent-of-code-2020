using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day14 : SolverBase
    {
        protected override object Part1Solution => ComputePart1Solution();

        protected override object Part2Solution => ComputePart2Solution();

        public long ComputePart1Solution(IEnumerable<string> input = null)
        {
            input = input ?? LoadRawData();
            var machine = new Part1Machine();

            foreach(var instruction in input)
            {
                if(instruction.StartsWith("mask = "))
                {
                    var maskBitsLsbFirst = instruction.Split(' ').Last().Reverse();

                    var onesMask = maskBitsLsbFirst
                        .Select((bit, idx) => (bit == '1' ? 1L : 0L) << idx)
                        .Sum();
                    var zerosMask = ~maskBitsLsbFirst
                        .Select((bit, idx) => (bit == '0' ? 1L : 0L) << idx)
                        .Sum();

                    machine.SetMask(onesMask, zerosMask);
                }
                else {
                    var value = long.Parse(instruction.Split(' ').Last());

                    var addressTxt = string.Join("", instruction.Skip(4).TakeWhile(x => char.IsDigit(x)));

                    var address = long.Parse(addressTxt);

                    machine.SetValue(address, value);
                }

            }

            return machine.GetSumOfMemoryLocations();
        }

        public long ComputePart2Solution(IEnumerable<string> input = null)
        {
            return RunPart2(input).GetSumOfMemoryLocations();
        }

        public Part2Machine RunPart2(IEnumerable<string> input = null)
        {
            input = input ?? LoadRawData();
            var machine = new Part2Machine();

            foreach(var instruction in input)
            {
                if(instruction.StartsWith("mask = "))
                {
                    var maskBits = instruction.Split(' ').Last();

                    machine.SetMask(maskBits);
                }
                else {
                    var value = long.Parse(instruction.Split(' ').Last());

                    var addressTxt = string.Join("", instruction.Skip(4).TakeWhile(x => char.IsDigit(x)));

                    var address = long.Parse(addressTxt);

                    machine.SetValue(address, value);
                }

                System.Console.WriteLine($"Current solution: {machine.GetSumOfMemoryLocations()}");
            }

            return machine;
        }

        public sealed class Part1Machine
        {
            private readonly Dictionary<long, long> _memory = new Dictionary<long, long>();

            private long _onesMask;
            private long _zerosMask = -1L;

            public void SetMask(long onesMask, long zerosMask)
            {
                _onesMask = onesMask;
                _zerosMask = zerosMask;

            }

            public void SetValue(long address, long value)
            {
                _memory[address] = (value | _onesMask) & _zerosMask;
            }

            public long GetSumOfMemoryLocations()
            {
                return _memory.Values.Sum();
            }
        }

        public sealed class Part2Machine
        {
            private readonly Dictionary<long, long> _memory = new Dictionary<long, long>();

            private long _onesMask;
            private int[] _floatMaskBitRanks;

            private long _floatInverseMask;

            public void SetMask(IEnumerable<char> maskBits)
            {
                var maskBitsLsbFirst = maskBits.Reverse();

                _onesMask = maskBitsLsbFirst
                    .Select((bit, idx) => (bit == '1' ? 1L : 0L) << idx)
                    .Sum();

                _floatMaskBitRanks = maskBitsLsbFirst
                    .Select((c, pos) => new { c, pos })
                    .Where(x => x.c == 'X')
                    .Select(x => x.pos)
                    .ToArray();

                _floatInverseMask = ~_floatMaskBitRanks
                    .Select(x => 1L << x)
                    .Sum();
            }

            public void SetValue(long address, long value)
            {
                address = (address & _floatInverseMask) | _onesMask;

                foreach(var floatVal in GetFloatingValues(0L, _floatMaskBitRanks, 0))
                {
                    var toWriteAddr = address | floatVal;

                    if(toWriteAddr > (1L << 36))
                    {
                        throw new System.Exception("wat");
                    }
                    _memory[toWriteAddr] = value;
                }
            }

            public long GetSumOfMemoryLocations()
            {
                return _memory.Values.Sum();
            }

            public IEnumerable<long> GetAddresses()
            {
                return _memory.Keys;
            }

            public static IEnumerable<long> GetFloatingValues(long baseValue, int[] ranks, int pos)
            {
                if(pos >= ranks.Length)
                    return new [] { baseValue };

                var left = GetFloatingValues(baseValue, ranks, pos + 1);

                var newBase = baseValue | (1L << ranks[pos]);

                var right = GetFloatingValues(newBase, ranks, pos + 1);

                return left.Concat(right);
            }
        }
    }
}