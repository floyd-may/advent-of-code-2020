module AdventOfCode.Fs.Day18

type public Ast =
    | Parens of Ast
    | Add of Ast * Ast
    | Mult of Ast * Ast
    | Num of int64

let isDigit c = System.Char.IsDigit c
let isWhitespace c = System.Char.IsWhiteSpace c
let eatWhitespace input = List.skipWhile isWhitespace input

let parseNum input =
    let digits =
        List.takeWhile isDigit input
        |> Seq.map string
        |> String.concat ""

    let rest = List.skip digits.Length input
    let num = digits |> int64

    (Num num, rest)

let parseParen input parse =
    let (contents, rest) = parse (List.tail input)

    (Parens contents, List.tail rest)

let parsePrefix parse input =
    let input = eatWhitespace input

    match input with
        | d :: rest when isDigit d -> parseNum input
        | '(' :: rest -> parseParen input parse
        | [] -> failwith "unexpected eof"
        | x::xs -> failwithf "unexpected char %s" (x |> string)

let rec parseInfix left input =
    let input = eatWhitespace input

    match input with
    | '+' :: rest ->
        let (right, rest) = eatWhitespace rest |> parsePrefix beginParse
        let rest = eatWhitespace rest
        continueParse (Add (left, right)) rest
    | '*' :: rest ->
        let (right, rest) = eatWhitespace rest |> parsePrefix beginParse
        let rest = eatWhitespace rest
        continueParse (Mult (left, right)) rest
    | _ -> (left, input)

and continueParse left input =
    match input with
    | [] -> (left, [])
    | _ ->
        parseInfix left input

and beginParse input =
    let (left, rest) = parsePrefix beginParse input

    continueParse left rest

let public parse str =
    let result = beginParse (List.ofSeq str)

    result |> fst

let public parse2 str =
    let result = beginParse (List.ofSeq str)

    result |> fst

let rec computeAst ast =
    match ast with
    | Num x -> x
    | Parens x -> computeAst x
    | Add (x, y) -> computeAst x + computeAst y
    | Mult (x, y) -> computeAst x * computeAst y

let public computePart1 str =
    let parsed = parse str

    computeAst parsed


