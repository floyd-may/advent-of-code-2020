module AdventOfCode.Fs.Day18

module BaseTypes =
    type public Ast =
        | Parens of Ast
        | Add of Ast * Ast
        | Mult of Ast * Ast
        | Num of int64

    type ParseResult = {
        Ast: Ast;
        Rest: char list;
    }
    let toResult left rest = { Ast = left; Rest = rest; }

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

        toResult (Num num) rest

    let parseParen input parse =
        let result = parse (List.tail input)

        toResult (Parens result.Ast) (List.tail result.Rest)

    let parsePrefix parse input =
        let input = eatWhitespace input

        match input with
            | d :: rest when isDigit d -> parseNum input
            | '(' :: rest -> parseParen input parse
            | [] -> failwith "unexpected eof"
            | x::xs -> failwithf "unexpected char %s" (x |> string)

open BaseTypes

module RecursiveDescent =

    let rec parseInfix left input =
        let input = eatWhitespace input

        match input with
        | [] -> toResult left input
        | '+' :: rest ->
            let result = parsePrefix doParse rest
            parseInfix (Add (left, result.Ast)) result.Rest
        | '*' :: rest ->
            let result = parsePrefix doParse rest
            parseInfix (Mult (left, result.Ast)) result.Rest
        | _ -> toResult left input

    and doParse input : ParseResult =
        let result = parsePrefix doParse input

        parseInfix result.Ast result.Rest

let isEqual val1 val2 =
    val1 = val2

let rec reduceParens tokens =
    let leftParenPos =
        tokens
        |> List.tryFindIndexBack (isEqual "(")

    match leftParenPos with
    | None -> tokens
    | Some leftIdx ->

    let rightIdx =
        (
            tokens
            |> List.skip leftIdx
            |> List.findIndex (isEqual ")")
        ) + leftIdx

    let left = tokens |> List.take leftIdx
    let right =
        tokens
        |> List.skip (rightIdx + 1)

    let contentWidth = rightIdx - leftIdx - 1

    let content =
        tokens
        |> List.skip (leftIdx + 1)
        |> List.take contentWidth

    reduceParens (left @ (reduce2 content @ right))

and reducePlus tokens =
    let plusPosOpt =
        tokens
        |> List.tryFindIndex (isEqual "+")

    match plusPosOpt with
    | None -> tokens
    | Some idx ->

    let left = List.take (idx - 1) tokens
    let right = List.skip (idx + 2) tokens

    let leftOperand =
        tokens
        |> List.item (idx - 1)
        |> decimal
    let rightOperand =
        tokens
        |> List.item (idx + 1)
        |> decimal

    let sum = leftOperand + rightOperand |> string

    reducePlus (left @ (sum :: right))

and reduceMult tokens =
    let plusPosOpt =
        tokens
        |> List.tryFindIndex (isEqual "*")

    match plusPosOpt with
    | None -> tokens
    | Some idx ->

    let left = List.take (idx - 1) tokens
    let right = List.skip (idx + 2) tokens

    let leftOperand =
        tokens
        |> List.item (idx - 1)
        |> decimal
    let rightOperand =
        tokens
        |> List.item (idx + 1)
        |> decimal

    let product = leftOperand * rightOperand |> string

    reduceMult (left @ (product :: right))

and reduce2 input =
    input
    |> reduceParens
    |> reducePlus
    |> reduceMult

let public parse2 str =
    let tokens =
        List.ofSeq str
        |> List.filter (isWhitespace >> not)
        |> List.map string

    let result = reduce2 tokens

    result |> List.head |> decimal

let rec computeAst ast =
    match ast with
    | Num x -> x
    | Parens x -> computeAst x
    | Add (x, y) -> computeAst x + computeAst y
    | Mult (x, y) -> computeAst x * computeAst y

let public parse str =
    let result = RecursiveDescent.doParse (List.ofSeq str)

    result.Ast

let public computePart1 str =
    let parsed = parse str

    computeAst parsed

let public computePart2 str =
    parse2 str

