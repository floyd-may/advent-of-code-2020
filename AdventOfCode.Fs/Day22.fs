module AdventOfCode.Fs.Day22
open FSharpx.Collections

type State<'A> = {
    Player1: int Queue
    Player2: int Queue
    UserState: 'A
}

type Winner = Player1 | Player2

module State =
    let getWinner state =
        if state.Player1.IsEmpty
        then Some Player2
        else if state.Player2.IsEmpty
        then Some Player1
        else None

    let create p1 p2 st = {
        Player1 = p1;
        Player2 = p2;
        UserState = st;
    }

    let unitState p1 p2 =
        create p1 p2 ()

    let p1win state = {
        state with
            Player1 = state.Player1.Tail.Conj(state.Player1.Head).Conj(state.Player2.Head);
            Player2 = state.Player2.Tail;
    }
    let p2win state = {
        state with
            Player1 = state.Player1.Tail;
            Player2 = state.Player2.Tail.Conj(state.Player2.Head).Conj(state.Player1.Head);
    }

module Part1 =
    let playRound state =
        let winner = State.getWinner state
        match winner with
        | Some _ -> state
        | _ ->
        let p1Head = state.Player1.Head
        let p2Head = state.Player2.Head
        if p1Head > p2Head
        then State.p1win state
        else State.p2win state

    let rec playGameToWinner state =
        let winner = State.getWinner state
        match winner with
        | Some _ -> state
        | _ ->
        let newState = playRound state
        playGameToWinner newState

module Part2 =
    let shouldRecursePlayer (deck: int Queue) =
        let length = deck.Length
        if length = 0
        then false
        else
        let remainingCards = length - 1
        deck.Head <= remainingCards

    let shouldRecurse state =
        shouldRecursePlayer state.Player1
            &&
            shouldRecursePlayer state.Player2

    let serializePlayer p =
        Seq.map string p
        |> String.concat ","

    let serialize st =
        let p1 = serializePlayer st.Player1
        let p2 = serializePlayer st.Player2
        sprintf "%s %s" p1 p2

    let toPrevGame st =
        (st.Player1, st.Player2)

    let p1win state = {
        (State.p1win state) with
            UserState =
                Set.add (serialize state) state.UserState
    }
    let p2win state = {
        (State.p2win state) with
            UserState =
                Set.add (serialize state) state.UserState
    }

    let takeNtail n q =
        Seq.take n q
        |> Queue.ofSeq


    let printState st =
        let games = st.UserState |> Set.count
        if games % 8000 = 0 && ((games = 0) |> not)
        then printfn "(%i) %s" games (serialize st)
        else ()

    let rec playGame state =
        do printState state
        let winner = State.getWinner state
        if winner.IsSome then state else
        if state.UserState |> (Set.contains (serialize state))
        then p1win state |> playGame
        else
        let optSubgameResult = tryRecurse state
        if optSubgameResult |> Option.isSome
        then
            let subgameResult = optSubgameResult.Value
            match State.getWinner subgameResult with
            | Some Player1 -> p1win state |> playGame
            | Some Player2 -> p2win state |> playGame
            | _ -> failwith "subgame didn't get a winner"
        else if state.Player1.Head > state.Player2.Head
        then p1win state |> playGame
        else p2win state |> playGame

    and tryRecurse state =
        if shouldRecurse state
        then
            let p1sub = takeNtail state.Player1.Head state.Player1.Tail
            let p2sub = takeNtail state.Player2.Head state.Player2.Tail
            let substate = State.create p1sub p2sub Set.empty
            playGame substate |> Some
        else None

let scoreGame state =
    let winner =
        if state.Player1.IsEmpty
        then state.Player2
        else state.Player1
    winner.Rev()
    |> Seq.indexed
    |> Seq.map (fun (idx, card) -> (idx + 1) * card)
    |> Seq.sum

let parseGame input =
    let allInput = String.concat "\n" input

    let playerParts = allInput.Split("\n\n")

    let parsePlayer (player: string) =
        player.Split("\n")
        |> Seq.tail
        |> Seq.map int
        |> Queue.ofSeq

    {
        Player1 = parsePlayer playerParts.[0];
        Player2 = parsePlayer playerParts.[1];
        UserState = ()
    }

let getPart1Solution input =
    let state = parseGame input

    let finalState = Part1.playGameToWinner state

    scoreGame finalState

let getPart2Solution input =
    let state = parseGame input

    let state = State.create state.Player1 state.Player2 Set.empty

    let finalState = Part2.playGame state

    scoreGame finalState
    // not 33759