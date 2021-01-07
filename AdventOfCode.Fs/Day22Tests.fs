module AdventOfCode.Fs.Day22Tests

open Xunit
open FsUnit.Xunit
open Day22
open Day22.Part1

open FSharpx.Collections

let q l =
    l
    //|> Seq.rev
    |> Queue.ofSeq

let toState l1 l2 =
    State.unitState (q l1) (q l2)

[<Fact>]
let ``war with p1 winner``() =
    let state = toState [7;4;3;] [3;6;8;]

    let actual = playRound state

    let expected = toState [4;3;7;3] [6;8;]

    actual |> should equal expected

[<Fact>]
let ``war with p2 winner``() =
    let state = toState [7;4;10;] [9;6;8;]

    let actual = playRound state

    let expected = toState [4;10;] [6;8;9;7]

    actual |> should equal expected

[<Fact>]
let ``score game p1 winner``() =
    let state = toState [7;4;3;] []

    let actual = scoreGame state

    actual |> should equal 32

[<Fact>]
let ``score game p2 winner``() =
    let state = toState [] [9;6;8;]

    let actual = scoreGame state

    actual |> should equal 47

let sampleInput = [
    "Player 1:";
    "9";
    "2";
    "6";
    "3";
    "1";
    "";
    "Player 2:";
    "5";
    "8";
    "4";
    "7";
    "10";
]

[<Fact>]
let ``parse sample input``() =
    let expected = toState [9;2;6;3;1] [5;8;4;7;10]

    let actual = parseGame sampleInput

    actual |> should equal expected

let ``part 2 solution``() =
    let expected = 291

    let actual = getPart2Solution sampleInput

    actual |> should equal expected