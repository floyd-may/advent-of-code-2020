module AdventOfCode.Fs.Day23

type Game = {
    Max: int;
    Cups: int list;
}


let rec getNextCurrent last max selected =
    let nextCurrent =
        if last = 1
        then max
        else last - 1

    if List.contains nextCurrent selected
    then getNextCurrent nextCurrent max selected
    else nextCurrent


let singleRound state =
    let current = state.Cups.Head
    let selected = [
        state.Cups.Tail.Head;
        state.Cups.Tail.Tail.Head;
        state.Cups.Tail.Tail.Tail.Head;
    ]
    let remaining = state.Cups.Tail.Tail.Tail.Tail;
    let nextCurrent = getNextCurrent current state.Max selected

    let right =
        remaining
        |> Seq.takeWhile (fun x -> x <> nextCurrent)

    let left =
        remaining
        |> Seq.skipWhile (fun x -> x <> nextCurrent)
        |> Seq.tail

    Seq.concat [
        left;
        selected |> Seq.ofList;
        right;
        seq { current }
    ]

let getPart1Solution input =
    let numbers =
        input
        :> char seq
        |> Seq.map (string >> int)
        |> List.ofSeq

    0

let getPart2Solution input =
    0