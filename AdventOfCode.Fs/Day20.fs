module AdventOfCode.Fs.Day20

open FParsec

let getParseResult parser input =
    match run parser input with
    | Success (x, _, _) -> x
    | _ -> failwith "bad tile ID line format"

let split (sep:string) (s:string) = s.Split(sep)

let parseTileId line =
    let parser = skipString "Tile " >>. pint32 .>> skipChar ':' .>> eof

    getParseResult parser line |> int64


type Tile = {
    Id: int64;
    Pixels: string[,]
}

let parseTile (input: string seq) =
    let pixels =
        input
        |> Seq.tail
        |> Seq.map (List.ofSeq >> (List.map string))
        |> array2D

    {
        Id = parseTileId (Seq.head input)
        Pixels = pixels;
    }

let parseTiles input =
    input
    |> String.concat "\n"
    |> split "\n\n"
    |> Seq.map ((split "\n") >> parseTile)
    |> List.ofSeq

let revStr input =
    input
    |> Seq.rev
    |> Seq.map string
    |> String.concat ""

let getEdges tile =
    let width = tile.Pixels.GetLength(1)
    let height = tile.Pixels.GetLength(0)

    let top =
        Seq.init width (fun x -> tile.Pixels.[x, 0])
        |> String.concat ""
    let bottom =
        Seq.init width (fun x -> tile.Pixels.[x, height - 1])
        |> String.concat ""
    let left =
        Seq.init height (fun y -> tile.Pixels.[0, y])
        |> String.concat ""
    let right =
        Seq.init height (fun y -> tile.Pixels.[width - 1, y])
        |> String.concat ""

    Set.ofList [
        top;
        bottom;
        left;
        right;
        revStr top;
        revStr bottom;
        revStr left;
        revStr right;
    ]

let getPart1Solution input =
    let tiles = parseTiles input

    let edgeCountsByEdge =
        tiles
        |> Seq.collect getEdges
        |> Seq.groupBy id
        |> Seq.map (fun (k, vals) -> (k, Seq.length vals))
        |> Map.ofSeq

    let isCorner tile =
        tile
        |> getEdges
        |> Seq.map (fun f -> edgeCountsByEdge.[f])
        |> Seq.filter (fun c -> c = 1)
        |> Seq.length
        |> (fun x -> x = 4)

    let corners =
        tiles
        |> Seq.filter isCorner
        |> List.ofSeq

    corners |> List.fold (fun agg t -> agg * t.Id) 1L
