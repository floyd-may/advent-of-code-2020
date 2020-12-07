using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    public sealed class Day7 : SolverBase
    {
        private readonly RuleModel _model;
        public Day7()
        {
            _model = RuleModel.Parse(LoadRawData());
        }
        protected override object Part1Solution => _model.PathsToShinyGoldBag;

        protected override object Part2Solution => _model.RequiredBagsInsideShinyGold;

        protected override int DayNumber => 7;

        public sealed class RuleModel
        {
            public sealed class GraphEdge
            {
                public GraphNode Target { get; }
                public int Count { get; }

                public GraphEdge(GraphNode target, int count)
                {
                    Count = count;
                    Target = target;
                }
            }

            public sealed class GraphNode
            {
                public string Label { get; set; }
                public bool TopLevel => Label != "shiny gold" && Successors.Any();

                public IEnumerable<GraphNode> Successors => Edges.Select(x => x.Target);

                public List<GraphEdge> Edges { get; } = new List<GraphEdge>();

                public int InDegree { get; set; }

                public bool IsPathToShinyGoldBag =>
                    Successors.Any(x => x.Label == "shiny gold" || x.IsPathToShinyGoldBag);

                public string Print()
                {
                    return Print(0);
                }

                private string Print(int indentlevel)
                {
                    var space = "".PadLeft(indentlevel, ' ');
                    return string.Join($"\n{space}", new[] { Label }.Concat(
                        Successors.Select(x => x.Print(indentlevel + 4))
                    ));

                }

                public int RequiredBagCount =>
                    Edges
                        .Select(e => e.Count + e.Count * e.Target.RequiredBagCount)
                        .Sum();
            }

            public readonly Dictionary<string, GraphNode> _nodes = new Dictionary<string, GraphNode>();

            private static readonly Regex RuleParser = new Regex(@"^([\w ]+) bags contain (.*).$", RegexOptions.Compiled);
            private static readonly Regex NullRuleParser = new Regex(@"^([\w ]+) bags contain no other bags.$", RegexOptions.Compiled);
            private static readonly Regex ToRuleParser = new Regex(@"^([0-9]+) ([\w ]+) bags?$", RegexOptions.Compiled);

            public static RuleModel Parse(IEnumerable<string> input)
            {
                var model = new RuleModel();

                foreach(var line in input)
                {
                    var nullMatch = NullRuleParser.Match(line);
                    if(nullMatch.Success)
                    {
                        model.AddNodeIfNotExists(nullMatch.Groups[1].Value);
                        continue;
                    }

                    var match = RuleParser.Match(line);

                    var from = match.Groups[1].Value.Trim();

                    var toChunk = match.Groups[2].Value;

                    foreach(var part in toChunk.Split(", "))
                    {
                        var chunkMatch = ToRuleParser.Match(part);

                        var to = chunkMatch.Groups[2].Value.Trim();

                        var count = int.Parse(chunkMatch.Groups[1].Value);

                        model.AddEdge(from, to, count);
                    }
                }

                return model;
            }

            public int PathsToShinyGoldBag =>
                _nodes.Values
                    .Where(x => x.TopLevel)
                    .Where(x => x.IsPathToShinyGoldBag)
                    .Count();

            public int RequiredBagsInsideShinyGold => _nodes["shiny gold"].RequiredBagCount;

            private void AddEdge(string from, string to, int count)
            {
                var fromNode = GetNode(from);

                var toNode = GetNode(to);

                fromNode.Edges.Add(new GraphEdge(toNode, count));
                toNode.InDegree++;
            }

            public void AddNodeIfNotExists(string label)
            {
                GetNode(label);
            }

            private GraphNode GetNode(string label)
            {
                if(!_nodes.TryGetValue(label, out var node))
                {
                    node = new GraphNode { Label = label };
                    _nodes[label] = node;
                }

                return node;
            }
        }
    }
}