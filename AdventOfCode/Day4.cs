using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    public sealed class Day4 : SolverBase
    {
        public IEnumerable<string> RequiredFieldHeaders => new[] {
            "byr",
            "iyr",
            "eyr",
            "hgt",
            "hcl",
            "ecl",
            "pid",
            //"cid",
        };

        private readonly Regex _splitter = new Regex("([^\\s]+)\\s", RegexOptions.Compiled);

        public IEnumerable<IEnumerable<string>> Records => string.Join("\n", LoadRawData())
                .Split("\n\n")
                .Select(x => x + " ")
                .Select(x => _splitter.Matches(x).Select(m => m.Value.Trim()));

        public IEnumerable<IEnumerable<string>> RecordsWithRequiredFields => Records
            .Where(x => !RequiredFieldHeaders.Except(ExtractHeaders(x)).Any());

        protected override object Part1Solution => RecordsWithRequiredFields.Count();

        protected override object Part2Solution => RecordsWithRequiredFields
            .Select(x => x.All(f => ValidateField(f)))
            .Where(x => x)
            .Count();

        protected override int DayNumber => 4;

        public bool ValidateField(string field)
        {
            var parts = field.Split(":");
            if(parts.Length != 2)
            {
                return false;
            }

            var header = parts[0];
            var value = parts[1];

            switch(header)
            {
                case "byr":
                    return ValidateYear(value, min: 1920, max: 2002);
                case "iyr":
                    return ValidateYear(value, min: 2010, max: 2020);
                case "eyr":
                    return ValidateYear(value, min: 2020, max: 2030);
                case "hgt":
                    return ValidateHeight(value);
                case "hcl":
                    return ValidateHairColor(value);
                case "ecl":
                    return ValidateEyeColor(value);
                case "pid":
                    return ValidatePassportId(value);
            }

            return true;
        }

        public bool ValidateYear(string val, int min, int max)
        {
            return val.Length == 4
                && ValidateNumber(val, min: min, max: max);
        }

        public bool ValidateNumber(string val, int min, int max)
        {
            return int.TryParse(val, out var year)
                && year >= min
                && year <= max;
        }

        public bool ValidateHeight(string val)
        {
            if(val.EndsWith("cm"))
            {
                return ValidateNumber(val.Substring(0, val.Length - 2), 150, 193);
            }
            if(val.EndsWith("in"))
            {
                return ValidateNumber(val.Substring(0, val.Length - 2), 59, 76);
            }

            return false;
        }

        public bool ValidateHairColor(string val)
        {
            return Regex.IsMatch(val, "^#[0-9a-f]{6}$");
        }

        public bool ValidateEyeColor(string val)
        {
            switch(val)
            {
                case "amb":
                case "blu":
                case "brn":
                case "gry":
                case "grn":
                case "hzl":
                case "oth":
                    return true;
            }

            return false;
        }

        public bool ValidatePassportId(string val)
        {
            return Regex.IsMatch(val, "^[0-9]{9}$");
        }

        /*
        pid (Passport ID) - a nine-digit number, including leading zeroes.

        byr (Birth Year) - four digits; at least 1920 and at most 2002.
        iyr (Issue Year) - four digits; at least 2010 and at most 2020.
        eyr (Expiration Year) - four digits; at least 2020 and at most 2030.
        cid (Country ID) - ignored, missing or not.
        hgt (Height) - a number followed by either cm or in:
            If cm, the number must be at least 150 and at most 193.
            If in, the number must be at least 59 and at most 76.
        hcl (Hair Color) - a # followed by exactly six characters 0-9 or a-f.
        ecl (Eye Color) - exactly one of: amb blu brn gry grn hzl oth.
        */

        private IEnumerable<string> ExtractHeaders(IEnumerable<string> fields)
        {
            return fields.Select(x => x.Split(":")[0]);
        }
    }
}