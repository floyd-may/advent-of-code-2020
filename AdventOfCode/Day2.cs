using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public sealed class Day2 : SolverBase
    {
        protected override object Part1Solution => LoadRecords()
            .Where(x => x.Part1Valid)
            .Count();

        protected override object Part2Solution => LoadRecords()
            .Where(x => x.Part2Valid)
            .Count();

        protected override int DayNumber => 2;

        private IEnumerable<PasswordRecord> LoadRecords() =>
            LoadRawData().Select(PasswordRecord.Parse);


        public sealed class PasswordRecord
        {
            public char RequiredChar { get; set; }
            public int Min { get; set; }
            public int Max { get; set; }
            public string Password { get; set; }

            public bool Part1Valid =>
                RequiredCharCount >= Min
                && RequiredCharCount <= Max;

            private int RequiredCharCount => Password
                .Where(x => x == RequiredChar)
                .Count();

            public bool Part2Valid => ComputePart2Valid();

            public static PasswordRecord Parse(string rawRecord)
            {
                var parts = rawRecord.Split(" ");

                var range = parts[0];

                var rangeParts = range.Split("-");

                return new PasswordRecord
                {
                    Min = int.Parse(rangeParts[0]),
                    Max = int.Parse(rangeParts[1]),
                    RequiredChar = parts[1][0],
                    Password = parts[2],
                };
            }

            public bool ComputePart2Valid()
            {
                var leftValid = Password[Min - 1] == RequiredChar;
                var rightValid = Password[Max - 1] == RequiredChar;

                return new[] { leftValid, rightValid }
                    .Where(x => x)
                    .Count() == 1;
            }
        }
    }
}