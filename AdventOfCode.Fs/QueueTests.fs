module AdventOfCode.Q.Tests

open Xunit
open FsUnit.Xunit

open AdventOfCode.Queues

[<Fact>]
let ``from seq``() =
    let actual =
        MapQueue.ofSeq [3;4;5;6]
        :> int seq
        |> List.ofSeq

    let expected = [3;4;5;6]

    actual |> should equal expected

[<Fact>]
let ``pop``() =
    let q = MapQueue.ofSeq [3;4;5;6]

    let actual = q.Pop

    let expected = 3, MapQueue.ofSeq [4;5;6]

    actual |> should equal expected

[<Fact>]
let ``pop all elements``() =
    let mutable q = MapQueue.ofSeq [3;4;5;6]

    while not q.IsEmpty
        do q <- q.Tail

    q.IsEmpty |> should equal true

[<Fact>]
let ``pop all then push``() =
    let mutable q = MapQueue.ofSeq [3;4;5;6]

    while not q.IsEmpty
        do q <- q.Tail

    q <- MapQueue.push 7 q

    q |> List.ofSeq |> should equal [7]

[<Fact>]
let ``pop then push``() =
    let q =
        MapQueue.ofSeq [3;4;5;6]
        |> MapQueue.tail
        |> MapQueue.push 7

    q |> List.ofSeq |> should equal [4;5;6;7]

[<Fact>]
let ``two pops then tail``() =
    let q =
        MapQueue.ofSeq [3;4;5;6]
        |> MapQueue.tail
        |> MapQueue.tail

    q |> List.ofSeq |> should equal [5;6]

[<Fact>]
let ``special case 1``() =
    let data = [
        42;25;7;29;1;16;50;11;40;4;41;3;12;8;20;32;38;31;2;44;28;33;18;10
    ]
    let q = MapQueue.ofSeq data

    let result =
        q
        |> MapQueue.tail
        |> MapQueue.pushMany [42; 13]
        |> MapQueue.tail
        |> MapQueue.tail
        |> Seq.take 7
        |> List.ofSeq

    result |> should equal [
        29;1;16;50;11;40;4;
    ]
