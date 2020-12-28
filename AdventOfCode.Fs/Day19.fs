module AdventOfCode.Fs.Day19

open FParsec

type RuleId = int

type RuleSpec =
    | Sequence of RuleId list
    | Choice of RuleSpec list
    | Terminal of string

type Rule = {
    Id: RuleId
    Spec: RuleSpec
}

let parseSequence (input:string) =
    let input = input.Trim()
    let sequence =
        input.Split " "
        |> List.ofSeq
        |> List.map (fun x -> x.Trim() |> int)

    Sequence sequence

let parseChoice (input:string) =
    let choices =
        input.Split "|"
        |> List.ofSeq
        |> List.map parseSequence

    if List.length choices = 1
    then List.head choices
    else Choice choices

let parseRuleSpec (input:string) =
    let input = input.Trim()
    if input.StartsWith "\""
    then Terminal (input.Substring(1,1))
    else parseChoice input

let public parseRule (input:string) =
    let parts = input.Split(":")

    match parts with
    | [| idstr; specstr |] ->
        let ruleId = idstr.Trim() |> int
        let ruleSpec = parseRuleSpec (specstr.Trim())

        { Id = ruleId; Spec = ruleSpec }

    | _ -> failwithf "invalid input: %s" input

type ParserState = {
    LeftBalance: int;
    SeenRight: bool;
} with static member Default = { LeftBalance = 0; SeenRight = false }

let rec parserFromRule isPart2 (rules: Map<RuleId, Rule>) rule =
    // 8: 42 | 42 8
    // 11: 42 31 | 42 11 31
    if isPart2 && List.contains rule.Id [8;11]
    then

        if rule.Id = 8 then preturn ""
        else

        let parser42 = parserFromRule true rules (rules.[42])
        let parser31 = parserFromRule true rules (rules.[31])

        let addLeft = updateUserState (fun x -> { x with LeftBalance = x.LeftBalance + 1 })
        let addRight = updateUserState (fun x -> { x with LeftBalance = x.LeftBalance - 1; SeenRight = true })
        let isBalanced = userStateSatisfies (fun x -> x.LeftBalance > 0)
        let seenRight = userStateSatisfies (fun x -> x.SeenRight)
        let canParseLeft = userStateSatisfies (fun x -> not x.SeenRight)

        let pLeft = canParseLeft >>. parser42 .>> addLeft
        let pRight = parser31 .>> addRight

        let pAttempt = (attempt pRight) <|> pLeft .>> isBalanced

        pLeft >>. pLeft >>. (many pAttempt >>% "") .>> seenRight
    else
        let mutable isTerminal = false
        let toReturn =
            match rule.Spec with
            | Terminal x ->
                do isTerminal <- true
                pstring x
            | Sequence x ->
                x
                |> List.map (fun ruleId -> rules.[ruleId])
                |> List.map (parserFromRule isPart2 rules)
                |> List.reduce ( >>. )
            | Choice c ->
                c
                |> List.map (fun x -> { Id = rule.Id; Spec = x })
                |> List.map (parserFromRule isPart2 rules)
                |> List.reduce (fun l r -> (attempt l) <|> r)

        if isTerminal
        then toReturn
        else toReturn <?> sprintf "rule %i" rule.Id

let public parseRulesRaw input =
    input
    |> List.map parseRule
    |> List.map (fun x -> (x.Id, x))
    |> Map.ofList

let public parseRules isPart2 input =
    let map = parseRulesRaw input

    (parserFromRule isPart2 map map.[0]) .>> eof

let public evaluate rules input =
    let result = runParserOnString rules ParserState.Default "" input

    match result with
    | Success _ -> true
    | _ -> false

let public solvePart1 input =
    let parts =
        (String.concat "\n" input).Split("\n\n")
        |> List.ofArray

    match parts with
    | [ruleStr; inputStr] ->
        let rules = parseRules false (ruleStr.Split("\n") |> List.ofArray)

        (inputStr.Split("\n"))
        |> List.ofArray
        |> List.map (evaluate rules)
        |> List.filter id
        |> List.length
    | _ -> failwith "whoops - bad input"

let public solvePart2 input =
    let parts =
        (String.concat "\n" input).Split("\n\n")
        |> List.ofArray

    match parts with
    | [ruleStr; inputStr] ->
        let rules = parseRules true (ruleStr.Split("\n") |> List.ofArray)

        (inputStr.Split("\n"))
        |> List.ofArray
        |> List.map (evaluate rules)
        |> List.filter id
        |> List.length
    | _ -> failwith "whoops - bad input"