module AdventOfCode.Fs.Day20

type Tile = {
    Id: int64;
    Pixels: string[,]
}

type TileEdges = {
    Top: string;
    Bottom: string;
    Left: string;
    Right: string;
}

type TileOrientation = {
    Edges: TileEdges;
    Pixels: string[,]
}

type EdgeConstraint =
    | Edge
    | Neighbor of string
    | NonEdge

type PlacementConstraint = {
    Top: EdgeConstraint;
    Left: EdgeConstraint;
    Right: EdgeConstraint;
    Bottom: EdgeConstraint;
} with static member None = {
        Top = NonEdge;
        Bottom = NonEdge;
        Left = NonEdge;
        Right = NonEdge;
    }

type PartialResult = {
    Tiles: Tile list
    Size: int;
    PlacedTiles: Tile list list;
    UniqueEdges: string Set;
}

module Parsing =
    open FParsec

    let getParseResult parser input =
        match run parser input with
        | Success (x, _, _) -> x
        | _ -> failwith "bad tile ID line format"

    let split (sep:string) (s:string) = s.Split(sep)

    let parseTileId line =
        let parser = skipString "Tile " >>. pint32 .>> skipChar ':' .>> eof

        getParseResult parser line |> int64


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

module Matrix =
    let getTop matrix =
        let sz = Array2D.length1 matrix
        Seq.init sz (fun x -> matrix.[x, 0])
        |> String.concat ""
    let getBottom matrix =
        let sz = Array2D.length1 matrix
        Seq.init sz (fun x -> matrix.[x, sz - 1])
        |> String.concat ""
    let getLeft matrix =
        let sz = Array2D.length1 matrix
        Seq.init sz (fun y -> matrix.[0, y])
        |> String.concat ""
    let getRight matrix =
        let sz = Array2D.length1 matrix
        Seq.init sz (fun y -> matrix.[sz - 1, y])
        |> String.concat ""

    let getEdges matrix : TileEdges =
        {
            Top = getTop matrix;
            Bottom = getBottom matrix;
            Left = getLeft matrix;
            Right = getRight matrix;
        }

    let rotate90 matrix =
        let sx = Array2D.length1 matrix
        let sy = Array2D.length2 matrix

        Array2D.init sy sx (fun x y -> matrix.[y, sx - x - 1])

    let flipVert matrix =
        let sx = Array2D.length1 matrix
        let sy = Array2D.length2 matrix

        Array2D.init sx sy (fun x y -> matrix.[x, sy - y - 1])

    let getCoords matrix =
        let width = Array2D.length1 matrix
        let height = Array2D.length2 matrix

        Seq.init width id
        |> Seq.collect (fun x -> Seq.init height (fun y -> (x, y)))

    let getOrientations matrix =
        let r90 = rotate90;
        let flip = flipVert;
        let r180 = r90 >> r90;
        let r270 = r180 >> r90;
        let transforms = [
            id;
            r90;
            r180;
            r270;
            flip;
            flip >> r90;
            flip >> r180;
            flip >> r270;
        ]

        transforms
        |> List.map (fun t -> t matrix)

module Tile =
    let revStr input =
        input
        |> Seq.rev
        |> Seq.map string
        |> String.concat ""

    let getEdges (tile:Tile) =
        let origEdges = Matrix.getEdges tile.Pixels

        Set.ofList [
            origEdges.Top;
            origEdges.Bottom;
            origEdges.Left;
            origEdges.Right;
            origEdges.Top |> revStr;
            origEdges.Bottom |> revStr;
            origEdges.Left |> revStr;
            origEdges.Right |> revStr;
        ]

    let getOrientations (tile:Tile) =
        Matrix.getOrientations tile.Pixels
        |> List.map (fun m -> { Pixels = m; Edges = Matrix.getEdges m })

module Placement =
    let isBottom uniqueEdges orientation =
        Set.contains orientation.Edges.Bottom uniqueEdges
    let isTop uniqueEdges orientation =
        Set.contains orientation.Edges.Top uniqueEdges
    let isLeft uniqueEdges orientation =
        Set.contains orientation.Edges.Left uniqueEdges
    let isRight uniqueEdges orientation =
        Set.contains orientation.Edges.Right uniqueEdges

    let private validateEdge uniqueEdges edgeConstraint edge =
        match edgeConstraint with
        | NonEdge -> Set.contains edge uniqueEdges |> not
        | Edge -> Set.contains edge uniqueEdges
        | Neighbor str -> edge = str

    let private validateOrientation uniqueEdges constraints orientation =
        let ve = validateEdge uniqueEdges
        let isValid =
            [
                ve constraints.Top orientation.Edges.Top;
                ve constraints.Bottom orientation.Edges.Bottom;
                ve constraints.Left orientation.Edges.Left;
                ve constraints.Right orientation.Edges.Right;
            ]
            |> List.reduce (&&)
        if isValid then Some orientation else None

    let tryOrientTile uniqueEdges constraints (tile:Tile) =
        Tile.getOrientations tile
        |> Seq.ofList
        |> Seq.map (validateOrientation uniqueEdges constraints)
        |> Seq.map (Option.map (fun o -> { tile with Pixels = o.Pixels }))
        |> Seq.choose id
        |> Seq.tryHead

    let brConstraint = {
        PlacementConstraint.None with
            Bottom = Edge;
            Right = Edge;
    }

    let getBottomConstraint state =
        match state.PlacedTiles with
        | []
        | [ _ ] -> Edge
        | [ curRow; nextRow ]
        | curRow :: (nextRow :: _) ->
        let downTile =
            nextRow
            |> List.item (state.Size - (List.length curRow) - 1)
        Neighbor (Matrix.getTop downTile.Pixels)

    let getRightConstraint state =
        let row = state.PlacedTiles.Head
        match row with
        | [] -> Edge
        | x :: _ -> Neighbor (Matrix.getLeft x.Pixels)

    let getLeftConstraint state =
        let row = state.PlacedTiles.Head
        let rowSize = List.length row
        if rowSize + 1 = state.Size
        then Edge
        else NonEdge

    let getTopConstraint state =
        let rowCount = List.length state.PlacedTiles
        if rowCount = state.Size
        then Edge
        else NonEdge

    let getConstraint state =
        {
            Top = getTopConstraint state;
            Bottom = getBottomConstraint state;
            Left = getLeftConstraint state;
            Right = getRightConstraint state;
        }

module Monster =
    let pixels =
        [
            "                  # ";
            "#    ##    ##    ###";
            " #  #  #  #  #  #   ";
        ]
        |> List.map (Seq.map string)
        |> array2D

    let coords =
        Matrix.getCoords pixels
        |> Seq.filter (fun (x, y) -> pixels.[x, y] = "#")
        |> List.ofSeq
    let coordsAt x y =
        coords
        |> List.map (fun (mx, my) -> (x + mx, y + my))
    let width = Array2D.length1 pixels
    let height = Array2D.length2 pixels

    let existsAt (image: string[,]) x y =
        let isMatch monsterX monsterY =
            let imagePixel = image.[x + monsterX, y + monsterY]

            imagePixel = "#"

        coords
        |> Seq.map (fun (monsterX, monsterY) -> isMatch monsterX monsterY)
        |> Seq.forall id

    let tryGetPositions image =
        let hasMonsterAt coord = existsAt image (fst coord) (snd coord)

        let imageWidth = Array2D.length1 image
        let imageHeight = Array2D.length2 image

        let attemptCoords =
            Seq.init (imageWidth - width) id
            |> Seq.collect (fun x -> Seq.init (imageHeight - height) (fun y -> (x, y)))

        let monsterPositions =
            attemptCoords
            |> Seq.filter hasMonsterAt
            |> List.ofSeq

        if monsterPositions.IsEmpty
        then None
        else Some monsterPositions


let placeTile state tile =
    let mutable prevRows = state.PlacedTiles.Tail
    let row = state.PlacedTiles.Head
    let mutable newRow = tile :: row

    let newRowSize = List.length newRow
    let rowCount = List.length state.PlacedTiles

    if newRowSize = state.Size && rowCount < state.Size
    then
        do prevRows <- newRow :: prevRows
        do newRow <- []

    {
        state with
            PlacedTiles = newRow :: prevRows;
            Tiles = List.filter (fun x -> x.Id = tile.Id |> not) state.Tiles
    }

let rec private solveBoard state =
    match state.Tiles with
    | [] -> Some state
    | _ ->
    let currentConstraint = Placement.getConstraint state
    let tryOrient = Placement.tryOrientTile state.UniqueEdges currentConstraint
    let potentialTiles =
        state.Tiles
        |> List.map tryOrient
        |> List.choose id
    potentialTiles
    |> Seq.ofList
    |> Seq.map (placeTile state)
    |> Seq.map solveBoard
    |> Seq.choose id
    |> Seq.tryHead

let constructImage (placedTiles: Tile list list) =
    let tileSize =
        placedTiles.Head.Head.Pixels
        |> Array2D.length1
        |> (fun x -> x - 2)
    let gridSize = placedTiles.Head |> List.length

    let indexRow row =
        row
        |> List.indexed
        |> Map.ofList

    let indexedTiles =
        placedTiles
        |> List.map indexRow
        |> List.indexed
        |> Map.ofList

    let getPixel x y =
        let tileX = x / tileSize
        let tileY = y / tileSize
        let tilePixelX = (x % tileSize) + 1
        let tilePixelY = (y % tileSize) + 1
        let row = indexedTiles.[tileY]
        let tile = row.[tileX]

        tile.Pixels.[tilePixelX, tilePixelY]

    let imageSize = tileSize * gridSize

    Array2D.init imageSize imageSize getPixel

let private parseAndSolveBoard input =
    let tiles = Parsing.parseTiles input

    let uniqueEdges =
        tiles
        |> Seq.collect Tile.getEdges
        |> Seq.groupBy id
        |> Seq.map (fun (k, vals) -> (k, Seq.length vals))
        |> Seq.filter (fun f -> snd f = 1)
        |> Seq.map fst
        |> Set.ofSeq

    let state = {
        Size = System.Math.Sqrt(List.length tiles |> float) |> int;
        PlacedTiles = [[]];
        UniqueEdges = uniqueEdges;
        Tiles = tiles
    }

    solveBoard state |> Option.get

let getPart1Solution input =
    let solvedState = parseAndSolveBoard input

    let topRow = solvedState.PlacedTiles.Head
    let bottomRow = solvedState.PlacedTiles |> List.last
    let corners = [
        topRow.Head; // top left
        topRow |> List.last; // top right
        bottomRow.Head;
        bottomRow |> List.last;
    ]

    corners |> List.fold (fun agg t -> agg * t.Id) 1L

let getPart2Solution input =
    let state = parseAndSolveBoard input

    let image = constructImage state.PlacedTiles

    let orientations = Matrix.getOrientations image

    let orientation, positions =
        orientations
        |> Seq.ofList
        |> Seq.map (fun o -> (o, Monster.tryGetPositions o))
        |> Seq.filter (snd >> Option.isSome)
        |> Seq.map (fun (o, optPos) -> (o, optPos.Value))
        |> Seq.head

    let allMonsterCoords =
        positions
        |> List.collect (fun (x, y) -> Monster.coordsAt x y)

    let setCoord coord =
        let (x, y) = coord

        do orientation.[x, y] <- "0"

    do List.iter setCoord allMonsterCoords

    let roughness =
        Matrix.getCoords orientation
        |> Seq.map (fun coord -> Array2D.get orientation (fst coord) (snd coord))
        |> Seq.filter (fun c -> c = "#")
        |> Seq.length

    roughness


