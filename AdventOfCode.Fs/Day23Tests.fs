module AdventOfCode.Fs.Day23Tests

open Xunit
open FsUnit.Xunit
open Day23

[<Fact>]
let ``single round``() =
    let start = Game.create [3; 8; 9; 1; 2; 5; 4; 6; 7; ] 9
    let actual = Game.singleRound start

    let expected = Game.create [2; 8; 9; 1; 5; 4; 6; 7; 3;] 9

    actual |> should equal expected

[<Fact>]
let ``sample part 1 solution``() =
    let actual = getPart1Solution "389125467"

    actual |> should equal 67384529