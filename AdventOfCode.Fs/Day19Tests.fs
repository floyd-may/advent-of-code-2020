module AdventOfCode.Fs.Day19Tests

open Xunit
open FsUnit.Xunit
open Day19

let sampleInput = [
    "0: 4 1 5";
    "1: 2 3 | 3 2";
    "2: 4 4 | 5 5";
    "3: 4 5 | 5 4";
    "4: \"a\"";
    "5: \"b\"";
    "";
    "ababbb";
    "bababa";
    "abbbab";
    "aaabbb";
    "aaaabbb";
]

let rulePortion = List.take 6 sampleInput

[<Fact>]
let ``sample rules by input``() =
    let rules = parseRules false rulePortion

    let inputs = List.skip 7 sampleInput

    let actual = inputs |> List.map (evaluate rules)

    actual |> should equal [true;false;true;false;false]

[<Fact>]
let ``sample input``() =
    let actual = solvePart1 sampleInput

    actual |> should equal 2

open FParsec
let sampleInput2 = [
    "42: 9 14 | 10 1";
    "9: 14 27 | 1 26";
    "10: 23 14 | 28 1";
    "1: \"a\"";
    "11: 42 31";
    "5: 1 14 | 15 1";
    "19: 14 1 | 14 14";
    "12: 24 14 | 19 1";
    "16: 15 1 | 14 14";
    "31: 14 17 | 1 13";
    "6: 14 14 | 1 14";
    "2: 1 24 | 14 4";
    "0: 8 11";
    "13: 14 3 | 1 12";
    "15: 1 | 14";
    "17: 14 2 | 1 7";
    "23: 25 1 | 22 14";
    "28: 16 1";
    "4: 1 1";
    "20: 14 14 | 1 15";
    "3: 5 14 | 16 1";
    "27: 1 6 | 14 18";
    "14: \"b\"";
    "21: 14 1 | 1 14";
    "25: 1 1 | 1 14";
    "22: 14 14";
    "8: 42";
    "26: 14 22 | 1 20";
    "18: 15 15";
    "7: 14 5 | 1 21";
    "24: 14 1";
    "";
    "abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa"; // bad
    "bbabbbbaabaabba";
    "babbbbaabbbbbabbbbbbaabaaabaaa";
    "aaabbbbbbaaaabaababaabababbabaaabbababababaaa";
    "bbbbbbbaaaabbbbaaabbabaaa";
    "bbbababbbbaaaaaaaabbababaaababaabab";
    "ababaaaaaabaaab";
    "ababaaaaabbbaba";
    "baabbaaaabbaaaababbaababb";
    "abbbbabbbbaaaababbbbbbaaaababb";
    "aaaaabbaabaaaaababaa";
    "aaaabbaaaabbaaa"; // bad
    "aaaabbaabbaaaaaaabbbabbbaaabbaabaaa";
    "babaaabbbaaabaababbaabababaaab"; // bad
    "aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba";
]

let rulePortion2 = List.take 31 sampleInput2

let getParseResult rules input = runParserOnString rules ParserState.Default "" input

[<Fact>]
let ``sample input 2 part 2``() =
    let actual = solvePart2 sampleInput2

    actual |> should equal 12