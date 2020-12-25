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
let ``sample rules, input 0``() =
    let rules = parseRules rulePortion

    let actual = evaluate rules "ababbb"

    actual |> should equal true

[<Fact>]
let ``sample input``() =
    let actual = solvePart1 sampleInput

    actual |> should equal 2