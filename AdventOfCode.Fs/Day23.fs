module AdventOfCode.Fs.Day23

type Cup = int

module CupCircle =
    type T = {
        CupToPos: Map<Cup, int>;
        PosToCup: Map<int, Cup>;
        Max: int;
        Offset: int;
    }

    let create (startValues: Cup list) max = {
        Max = max;
        PosToCup =
            startValues
            |> List.indexed
            |> Map.ofList;
        CupToPos =
            startValues
            |> List.indexed
            |> List.map (fun (a,b) -> (b, a))
            |> Map.ofList;
        Offset = 0;
    }

    let getCupIndex d cup =
        let optVal = Map.tryFind cup d.CupToPos
        match optVal with
        | None -> (cup - 1 - d.Offset) |> int
        | Some x -> x - d.Offset

    let private getPosition d idx =
        (idx % d.Max) + d.Offset

    let getCupAt d pos : Cup =
        let pos = getPosition d pos
        let optVal = Map.tryFind pos d.PosToCup
        match optVal with
        | None -> pos + 1
        | Some x -> x

    let selectCupsClockwiseFrom cup d =
        let idx = getCupIndex d cup
        [
            idx + 1;
            idx + 2;
            idx + 3;
        ]
        |> List.map (getCupAt d)

    let moveMid cups srcIdx dstIdx d =
        let moveCount = (srcIdx - dstIdx) |> abs
        let otherSrcIdxes =
            Seq.init moveCount (fun f -> f + dstIdx)
            |> Seq.map (fun x -> (x, x + 3))
        let mutable newCupToPos = d.CupToPos
        let mutable newPosToCup = d.PosToCup

        for (toMoveSrcIdx, toMoveDstIdx) in otherSrcIdxes do
            let cup = d.PosToCup.[toMoveSrcIdx]
            newPosToCup <- Map.add toMoveDstIdx cup newPosToCup
            newCupToPos <- Map.add cup toMoveDstIdx newCupToPos

        let idxes = [dstIdx; dstIdx + 1; dstIdx + 2]

        let cupToPosKvps = List.zip cups idxes
        for (cup, pos) in cupToPosKvps do
            newPosToCup <- Map.add pos cup newPosToCup
            newCupToPos <- Map.add cup pos newCupToPos

        {
            d with
                CupToPos = newCupToPos;
                PosToCup = newPosToCup;
        }

    let moveSelectedCups cups srcIdx dstIdx d =
        moveMid cups srcIdx dstIdx d

        // legend:
        // l: left
        // s: selected
        // m: mid
        // r: right
        // cases:
        // lllsssmmmrrr
        // lllmmmsssrrr


type CupCircle = CupCircle.T


type Game = {
    Max: int;
    Cups: Cup list;
    Current: Cup;
}

module Game =
    let create cups max = {
        Max = max;
        Cups = cups;
        Current = cups.Head
    }
    let rec private getDestination current max selected =
        let destination =
            if current = 1
            then max
            else current - 1

        if List.contains destination selected
        then getDestination destination max selected
        else destination

    let singleRound state =
        let current = state.Current
        let selected = state.Cups.Tail |> List.take 3
        let remaining = state.Cups |> List.skip 4
        let destination = getDestination current state.Max selected

        let destinationPos = List.findIndex (fun x -> x = destination) remaining

        let right = remaining |> List.skip (destinationPos + 1)

        let left = remaining |> List.take (destinationPos + 1)

        let nextCups = left @ selected @ right @ [current]

        { state with Cups = nextCups; Current = nextCups.Head }

let getPart1Solution input =
    let numbers =
        input
        :> char seq
        |> Seq.map (string >> int)
        |> List.ofSeq

    let max = List.max numbers
    let start = Game.create numbers max

    let iterations = Seq.replicate 100 ()

    let final =
        Seq.fold (fun s _ -> Game.singleRound s) start iterations

    let finalCups = final.Cups

    let idxOfOne = finalCups |> List.findIndex (fun x -> x = 1)

    let left = finalCups |> List.skip (idxOfOne + 1)
    let right = finalCups |> List.take (idxOfOne)

    left @ right
    |> List.map string
    |> String.concat ""
    |> int

let getPart2Solution input =
    0