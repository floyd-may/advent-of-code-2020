module AdventOfCode.Fs.Day22Tests

open Xunit
open FsUnit.Xunit
open Day22
open Day22.Part1
open AdventOfCode.Queues

let toState l1 l2 =
    State.unitState (MapQueue.ofSeq l1) (MapQueue.ofSeq l2)

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

[<Fact>]
let ``part 2 solution exampl``() =
    let expected = 291

    let actual = getPart2Solution sampleInput

    actual |> should equal expected

[<Fact(Skip="perf issue")>]
let ``actual part 2 solution``() =
    let input1 = MapQueue.ofSeq [
        30;42;25;7;29;1;16;50;11;40;4;41;3;12;8;20;32;38;31;2;44;28;33;18;10
        ]
    let input2 = MapQueue.ofSeq [
        36;13;46;15;27;45;5;19;39;24;14;9;17;22;37;47;43;21;6;35;23;48;34;26;49
    ]

    let state = State.create input1 input2 Set.empty

    let finalState = Part2.playGame state

    let score = finalState

    score |> should not' (equal 33759)
