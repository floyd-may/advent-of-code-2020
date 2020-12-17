using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public sealed class Day13 : SolverBase
    {
        protected override object Part1Solution => ComputePart1();

        protected override object Part2Solution => ComputePart2();

        public int ComputePart1(IEnumerable<string> input = null)
        {
            input = input ?? LoadRawData();

            var earliestDeparture = int.Parse(input.First());

            var busses = input.Last().Split(',')
                .Where(x => x != "x")
                .Select(x => int.Parse(x))
                .ToArray();

            var departuresByBus = busses
                .Select(bus => new {
                    bus,
                    departure = ((earliestDeparture % bus) == 0)
                        ? earliestDeparture
                        : ((earliestDeparture / bus) + 1) * bus
                })
                .OrderBy(x => x.departure);

            var departure = departuresByBus.First();

            return departure.bus * (departure.departure - earliestDeparture);
        }

        public long ComputePart2(IEnumerable<string> input = null)
        {
            input = input ?? LoadRawData();

            var busses = input.Last().Split(',')
                .Select(x => int.TryParse(x, out var busNo) ? (int?)busNo : null)
                .ToArray();

            var workingSet = busses
                .Select((bus, busIdx) => new { bus, busIdx })
                .Where(x => x.bus.HasValue)
                .Select(x => new Bus(x.bus.Value, x.busIdx))
                .OrderByDescending(x => x.BusNumber)
                .ToArray();

            var solution = workingSet
                .Aggregate((a, b) => a.FindCommonSolution(b));

            return solution.Solution;
        }

        public sealed class Bus
        {
            public Bus(long busNumber, long idx)
            {
                BusNumber = busNumber;
                Idx = -idx % busNumber;
            }

            private Bus(long busNumber, long a, bool isPrivate)
            {
                BusNumber = busNumber;
                Idx = a;
            }

            public long Idx { get; }
            public long BusNumber { get; }
            public long Solution => Idx.NormalizeToDivisor(N);
            public long N => BusNumber;
            public long A => Idx;

            public Bus FindCommonSolution(Bus other)
            {
                if(BusNumber < other.BusNumber)
                {
                    return other.FindCommonSolution(this);
                }

                var (m1, m2) = GetBezoutCoefficients(other);

                decimal n1 = this.N;
                decimal n2 = other.N;

                decimal a1 = A;
                decimal a2 = other.A;

                var n12 = n1 * n2;

                var a12 = (a1 * m2 * n2) + (a2 * m1 * n1);

                var ret = new Bus((long)n12, (long)(a12 % n12), true);

                if(ret.Solution % n1 != a1.NormalizeToDivisor(n1))
                {
                    throw new Exception($"whoops! merging busses {n1} and {n2} failed! {n1} not solved");
                }
                if(ret.Solution % n2 != a2.NormalizeToDivisor(n2))
                {
                    throw new Exception($"whoops! merging busses {n1} and {n2} failed! {n2} not solved");
                }

                return ret;
            }

            private (long, long) GetBezoutCoefficients(Bus other)
            {
                var old_r = BusNumber;
                var r = other.BusNumber;
                var old_s = 1L;
                var s = 0L;
                var old_t = 0L;
                var t = 1L;

                while(r != 0)
                {
                    var quotient = old_r / r;

                    var tmpR = r;
                    r = old_r - quotient * r;
                    old_r = tmpR;

                    var tmpS = s;
                    s = old_s - quotient * s;
                    old_s = tmpS;

                    var tmpT = t;
                    t = old_t - quotient * t;
                    old_t = tmpT;
                }

                return (old_s, old_t);
            }
        }
    }

    static class Extensions
    {
        public static long NormalizeToDivisor(this long a, long divisor)
        {
            return ((a % divisor) + divisor) % divisor;
        }

        public static decimal NormalizeToDivisor(this decimal a, decimal divisor)
        {
            return ((a % divisor) + divisor) % divisor;
        }
    }
}