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

let rec parseInfix usePrecedence precedence left input =
    let input = eatWhitespace input

    match input with
    | [] -> (left, input)
    | '+' :: rest ->
        let (right, rest) = parsePrefix (doParse usePrecedence 1) rest
        parseInfix usePrecedence 1 (Add (left, right)) rest
    | '*' :: rest ->
        let (right, rest) = parsePrefix (doParse usePrecedence precedence) rest
        if usePrecedence && precedence = 1 then
            let (right, rest) = parseInfix usePrecedence 0 right rest
            (Mult (left, right), rest)
        else
            parseInfix usePrecedence 0 (Mult (left, right)) rest
    | _ -> (left, input)

and doParse usePrecedence precedence input =
    let (left, rest) = parsePrefix (doParse usePrecedence precedence) input

    parseInfix usePrecedence precedence left rest

let public parse str =
    let result = doParse false 0 (List.ofSeq str)

    result |> fst

let public parse2 str =
    let result = doParse true 1 (List.ofSeq str)

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


