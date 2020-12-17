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

        public int ComputePart2(IEnumerable<string> input = null)
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

            System.Console.WriteLine($"{validTickets.Length} valid tickets...");
            System.Console.WriteLine($"{rules.Length} rules...");
            System.Console.WriteLine($"{ticketWidth} fields per tickets...");

            var rulesInOrder = GetRulesInOrder(0, ticketWidth, validTickets, rules);

            var departureRuleIndexes = rulesInOrder
                .Select((rule, ruleIdx) => new { rule, ruleIdx })
                .Where(x => x.rule.Name.StartsWith("departure"))
                .Select(x => x.ruleIdx)
                .ToArray();

            var departureValues = myTicket.Fields
                .Select((val, idx) => new { val, idx })
                .Where(x => departureRuleIndexes.Contains(x.idx))
                .Select(x => x.val)
                .ToArray();

            return departureValues.Aggregate(1, (p, v) => p * v);
        }

        private IEnumerable<Rule> GetRulesInOrder(int pos, int ticketWidth, IEnumerable<Ticket> tickets, IEnumerable<Rule> rules)
        {
            if(pos >= ticketWidth)
            {
                return new Rule[0];
            }
            var candidates = new List<Rule>();
            foreach(var rule in rules)
            {
                if(tickets.All(t => rule.Contains(t.Fields[pos])))
                {
                    candidates.Add(rule);
                }
            }

            if(pos == 0)
            {
                System.Console.WriteLine($"Evaluating {candidates.Count} candidates at position {pos}");
            }

            foreach(var candidate in candidates)
            {
                var nextRules = rules.Where(x => x.Name != candidate.Name).ToArray();

                var nextCandidates = GetRulesInOrder(pos + 1, ticketWidth, tickets, nextRules);

                if(nextCandidates != null)
                {
                    return new [] { candidate }.Concat(nextCandidates).ToArray();
                }
            }

            return null;
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
        }
    }
}