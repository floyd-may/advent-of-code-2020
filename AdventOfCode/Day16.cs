using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day16 : SolverBase
    {
        protected override object Part1Solution => ComputePart1();

        protected override object Part2Solution => ComputePart2();

        public int ComputePart1(IEnumerable<string> input = null)
        {
            input = input ?? LoadRawData();
            var parts = string.Join("\n", input).Split("\n\n");

            var rules = parts[0].Split("\n").Select(x => Rule.Parse(x));

            var otherTickets = parts[2].Split("\n")
                .Skip(1)
                .Select(x => Ticket.Parse(x))
                .ToArray();

            var invalidValues = otherTickets
                .SelectMany(ticket => ticket.GetInvalidValues(rules));

            return invalidValues.Sum();
        }

        public long ComputePart2(IEnumerable<string> input = null)
        {
            input = input ?? LoadRawData();
            var parts = string.Join("\n", input).Split("\n\n");

            var rules = parts[0].Split("\n").Select(x => Rule.Parse(x)).ToArray();

            var myTicket = Ticket.Parse(parts[1].Split("\n")[1]);

            var otherTickets = parts[2].Split("\n")
                .Skip(1)
                .Select(x => Ticket.Parse(x))
                .ToArray();

            var validTickets = otherTickets
                .Where(x => x.GetIsValid(rules))
                .ToArray();

            var ticketWidth = validTickets[0].Fields.Count();

            var allPositions = (1 << ticketWidth) - 1;

            var powersOfTwo = Enumerable.Range(0, ticketWidth)
                .Select(x => 1 << x)
                .ToHashSet();

            var ruleOptions = Enumerable
                .Repeat(allPositions, ticketWidth)
                .ToArray();

            var found = 0;
            foreach(var ticket in validTickets)
            {
                foreach(var (rule, ruleNo) in rules.Select((r, idx) => (r, idx)))
                {
                    var opts = ruleOptions[ruleNo];
                    if(!powersOfTwo.Contains(opts))
                    {
                        opts = opts & ticket.MapRule(rule);
                    }
                    ruleOptions[ruleNo] = opts;

                    if(powersOfTwo.Contains(opts))
                    {
                        found = found | opts;
                    }
                }

                found = ReduceOptions(ruleOptions, found, powersOfTwo);
            }

            ReduceOptions(ruleOptions, found, powersOfTwo);

            if(ruleOptions.Except(powersOfTwo).Any())
            {
                throw new System.Exception("didn't filter them all!");
            }

            var ruleIndexes = ruleOptions
                .Select(x => (int)System.Math.Log2(x))
                .ToArray();

            var rulesInOrder = ruleIndexes
                .Zip(rules, (idx, rule) => new { idx, rule })
                .OrderBy(x => x.idx)
                .ToArray();

            foreach(var (ticket, ticketIdx) in validTickets.Select((x, idx) => (x, idx)))
            {
                foreach(var (rule, val) in rulesInOrder.Zip(ticket.Fields, (r, v) => (r, v)))
                {
                    if(!rule.rule.Contains(val))
                    {
                        throw new System.Exception($"rule {rule.rule.Name} at position {rule.idx} does not match value {val} on ticket {ticketIdx + 1}");
                    }
                }
            }

            var departureRuleIndexes = rulesInOrder
                .Where(x => x.rule.Name.StartsWith("departure"))
                .Select(x => x.idx)
                .ToArray();

            var departureValues = myTicket.Fields
                .Select((val, idx) => new { val, idx })
                .Where(x => departureRuleIndexes.Contains(x.idx))
                .Select(x => x.val)
                .ToArray();

            return departureValues.Aggregate(1L, (p, v) => p * v);
        }

        private int ReduceOptions(int[] ruleOptions, int found, HashSet<int> powersOfTwo)
        {
            if(found == 0)
            {
                return 0;
            }
            var changed = true;
            while(changed)
            {
                changed = false;
                foreach(var (opts, idx) in ruleOptions.Select((x, idx) => (x, idx)))
                {
                    if(powersOfTwo.Contains(opts))
                    {
                        found |= opts;
                        continue;
                    }

                    var newOpts = opts & ~found;
                    changed = changed || opts != newOpts;
                    ruleOptions[idx] = newOpts;

                    if(powersOfTwo.Contains(newOpts))
                    {
                        found |= opts;
                    }
                }
            }

            return found;
        }

        public sealed class Range
        {
            public int Min { get; }
            public int Max { get; }

            public Range(int min, int max)
            {
                Min = min;
                Max = max;
            }

            public bool Contains(int value)
            {
                return value >= Min
                    && value <= Max;
            }
        }

        public sealed class Rule
        {
            public string Name { get; }
            public IEnumerable<Range> Ranges { get; }

            public Rule(string name, IEnumerable<Range> ranges)
            {
                Name = name;
                Ranges = ranges;
            }

            public static Rule Parse(string input)
            {
                var parts = input.Split(":");
                var name = input.Split(":")[0];

                var ranges = parts[1].Trim().Split(" or ")
                    .Select(x => x.Split("-"))
                    .Select(x => new Range(int.Parse(x[0]), int.Parse(x[1])))
                    .ToArray()
                    ;

                return new Rule(name, ranges);
            }

            public bool Contains(int value)
            {
                return Ranges.Any(x => x.Contains(value));
            }
        }

        public sealed class Ticket
        {
            public IList<int> Fields { get; }

            public static Ticket Parse(string input)
            {
                var fields = input.Split(",").Select(int.Parse).ToArray();

                return new Ticket(fields);
            }

            public Ticket(IList<int> fields)
            {
                Fields = fields;
            }

            public IEnumerable<int> GetInvalidValues(IEnumerable<Rule> rules)
            {
                return Fields.Where(val => !rules.Any(r => r.Contains(val)));
            }

            public bool GetIsValid(IEnumerable<Rule> rules)
            {
                return !GetInvalidValues(rules).Any();
            }

            public int MapRule(Rule rule)
            {
                var agg = 0;
                foreach(var (field, idx) in Fields.Select((f, i) => (f, i)))
                {
                    if(rule.Contains(field))
                    {
                        agg |= (1 << idx);
                    }
                }

                return agg;
            }
        }
    }
}