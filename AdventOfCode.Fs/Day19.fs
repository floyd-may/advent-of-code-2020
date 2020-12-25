module AdventOfCode.Fs.Day19

open System.Text.RegularExpressions
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
    let sequence =
        input.Split " "
        |> List.ofSeq
        |> List.map (fun x -> x.Trim())
        |> List.map int

    Sequence sequence

let parseChoice (input:string) =
    let choices =
        input.Split "|"
        |> List.ofSeq
        |> List.map (fun x -> x.Trim())
        |> List.map parseSequence

    Choice choices

let parseRuleSpec (input:string) =
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

let rec toRegex (rules:Map<RuleId, RuleSpec>) rule =
    match rule with
    | Terminal x -> x
    | Sequence x ->
        x
        |> List.map (fun ruleId -> rules.[ruleId])
        |> List.map (toRegex rules)
        |> String.concat ""
    | Choice c ->
        c
        |> List.map (toRegex rules)
        |> String.concat "|"
        |> sprintf "(%s)"

let rec toParser (rules:Map<RuleId, RuleSpec>) rule =
    match rule with
    | Terminal x -> pstring x
    | Sequence x ->
        x
        |> List.map (fun ruleId -> rules.[ruleId])
        |> List.map (toParser rules)
        |> List.reduce ( >>. )
    | Choice c ->
        c
        |> List.map (toParser rules)
        |> choice

let public parseRules input =
    let map =
        input
        |> List.map parseRule
        |> List.map (fun x -> (x.Id, x.Spec))
        |> Map.ofList

    sprintf "^%s$" (toRegex map map.[0])

let public evaluate rules input =
    Regex.IsMatch (input, rules)

let public solvePart1 input =
    let parts =
        (String.concat "\n" input).Split("\n\n")
        |> List.ofArray

    match parts with
    | [ruleStr; inputStr] ->
        let regex = parseRules (ruleStr.Split("\n") |> List.ofArray)

        (inputStr.Split("\n"))
        |> List.ofArray
        |> List.map (evaluate regex)
        |> List.filter id
        |> List.length
    | _ -> failwith "whoops - bad input"