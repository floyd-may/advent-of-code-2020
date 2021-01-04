module AdventOfCode.Fs.Day21Tests

open Xunit
open FsUnit.Xunit
open Day21

let sampleInput = [
    "mxmxvkd kfcds sqjhc nhms (contains dairy, fish)";
    "trh fvjkl sbzzf mxmxvkd (contains dairy)";
    "sqjhc fvjkl (contains soy)";
    "sqjhc mxmxvkd sbzzf (contains fish)";
]

[<Fact>]
let ``parse line``() =
    let actual = Parsing.parseInputLine sampleInput.Head

    let expected = {
        Ingredients = ["mxmxvkd";"kfcds";"sqjhc";"nhms"]
        Allergens = ["dairy"; "fish"]
    }

    actual |> should equal expected

[<Fact>]
let ``computes sample 1 solution (raw)``() =
    let finalState = getSolutionRaw sampleInput |> snd
    let result =
        finalState.Foods
        |> List.collect (fun x -> x.Ingredients)
        |> Set.ofList

    result |> should equal (Set.ofList ["kfcds"; "nhms"; "sbzzf"; "trh"])

[<Fact>]
let ``computes sample 1 solution``() =
    let result = getPart1Solution sampleInput

    result |> should equal 5

