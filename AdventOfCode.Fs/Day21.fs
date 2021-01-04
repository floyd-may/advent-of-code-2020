module AdventOfCode.Fs.Day21

open FParsec

type Food = {
    Ingredients: string list;
    Allergens: string list;
}

type State = {
    Foods: Food list;
    Allergens: string list;
    IngredientsWithAllergens: Map<string, string>;
}

module Parsing =
    let getParseResult parser input =
        match run parser input with
        | Success (x, _, _) -> x
        | Failure (err, _, _) -> failwithf "bad line format: %s" err

    let private charListToString chlist =
        chlist |> List.map string |> String.concat ""

    let private pSpace<'a> = pstring " "

    let private pWord<'a> = manyChars asciiLetter

    let private pIngredients<'a> = many (pWord .>> pSpace)

    let private pAllergenSeparator<'a> = pchar ',' >>. pSpace

    let private pAllergenInnerList<'a> = sepBy1 pWord pAllergenSeparator

    let private pAllergens<'a> =
        pstring "contains " >>. pAllergenInnerList
        |> between (pstring "(") (pstring ")")

    let private pFood<'a> =
        pIngredients .>>. pAllergens
        >>= (fun res -> preturn {
            Ingredients = fst res;
            Allergens = snd res;
        })

    let parseInputLine line =
        getParseResult pFood line

module Food =
    let withoutIngredient ingredient food =
        {
            food with
                Ingredients =
                    food.Ingredients
                    |> List.except [ingredient]
        }
    let withoutAllergen allergen (food:Food) =
        {
            food with
                Allergens =
                    food.Allergens
                    |> List.except [allergen]
        }

module State =
    let private withoutIngredient ingredient state =
        {
            state with
                Foods =
                    state.Foods
                    |> List.map (Food.withoutIngredient ingredient)
        }
    let private withoutAllergen allergen state =
        {
            state with
                Foods =
                    state.Foods
                    |> List.map (Food.withoutAllergen allergen);
                Allergens = List.except [allergen] state.Allergens;
        }
    let withIngredientToAllergen ingredient allergen state =
        let newState =
            state
            |> withoutIngredient ingredient
            |> withoutAllergen allergen
        {
            newState with
                IngredientsWithAllergens =
                    state.IngredientsWithAllergens
                    |> Map.add ingredient allergen
        }

let private allIngredients foods =
    foods
    |> Seq.collect (fun x -> x.Ingredients)
    |> Set.ofSeq

let private commonIngredients foods =
    foods
    |> Seq.map (fun x -> x.Ingredients |> Set.ofList)
    |> Seq.reduce Set.intersect

let private tryReduceAllergen state allergen =
    let hasAllergen (food:Food) = List.contains allergen food.Allergens
    let foodsWith = List.filter hasAllergen state.Foods
    let ingredientsWith = commonIngredients foodsWith |> List.ofSeq

    match ingredientsWith with
    | [ x ] -> State.withIngredientToAllergen x allergen state |> Some
    | _ -> None

let rec private reducePart1 state =
    match state.Allergens with
    | [] -> state
    | _ ->
    let nextState =
        state.Allergens
        |> Seq.ofList
        |> Seq.map (tryReduceAllergen state)
        |> Seq.choose id
        |> Seq.head

    reducePart1 nextState


let getSolutionRaw input =
    let foods =
        Seq.map Parsing.parseInputLine input
        |> List.ofSeq

    let allergens =
        foods
        |> Seq.ofList
        |> Seq.collect (fun f -> f.Allergens)
        |> Seq.distinct
        |> List.ofSeq

    let state = {
        Foods = foods;
        Allergens = allergens;
        IngredientsWithAllergens = Map.empty
    }

    let finalState = reducePart1 state

    (state, finalState)


let getPart1Solution input =
    let state, finalState = getSolutionRaw input

    finalState.Foods
    |> Seq.collect (fun x -> x.Ingredients)
    |> Seq.length

let getPart2Solution input =
    let _, finalState = getSolutionRaw input

    finalState.IngredientsWithAllergens
    |> Map.toSeq
    |> Seq.sortBy snd
    |> Seq.map fst
    |> String.concat ","

