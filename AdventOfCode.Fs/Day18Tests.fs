module AdventOfCode.Fs.Day18Tests

open Xunit
open FsUnit.Xunit
open Day18

[<Fact>]
let ``Parse simple int`` () =
    let parsed = parse "23"

    let expected = Num 23L

    parsed |> should equal expected

[<Fact>]
let ``Parse plus expression`` () =
    let parsed = parse "23 + 6"

    let expected = Add (Num 23L, Num 6L)

    parsed |> should equal expected

[<Fact>]
let ``Parse two plus expressions left associative`` () =
    let parsed = parse "23 + 6 + 3"

    let expected = Add (Add (Num 23L, Num 6L), Num 3L)

    parsed |> should equal expected

[<Fact>]
let ``simple parens`` () =
    let parsed = parse "(6 + 3)"

    let expected = Parens (Add ( Num 6L, Num 3L))

    parsed |> should equal expected

[<Fact>]
let ``Parse two plus expressions right associative with parens`` () =
    let parsed = parse "23 + (6 + 3)"

    let expected = Add (Num 23L, Parens (Add ( Num 6L, Num 3L)))

    parsed |> should equal expected

[<Fact>]
let ``Parse with precedence on plus`` () =
    let parsed = parse2 "23 * 6 + 3"

    let expected = Mult (Num 23L, Add (Num 6L, Num 3L))

    parsed |> should equal expected