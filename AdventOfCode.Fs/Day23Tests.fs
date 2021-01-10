module AdventOfCode.Fs.Day23Tests

open Xunit
open FsUnit.Xunit
open Day23

[<Fact>]
let ``sample part 1 solution``() =
    let actual = getPart1Solution "389125467"

    actual |> should equal 67384529