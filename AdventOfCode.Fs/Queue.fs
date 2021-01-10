module AdventOfCode.Queues

open System.Collections
open System.Collections.Generic

type MapQueue<'T> (map: Map<int, 'T>, min: int) =
    let mutable hashCode = None

    (*
    let isCorrupt =
        Seq.init map.Count (fun x -> x + min)
        |> Seq.forall (fun x -> Map.containsKey x map)
        |> not

    do
        if isCorrupt
        then failwithf "Corrupt queue %A with min %i" map min
    *)

    member _.RawMap = map
    member private _.OptHashCode = hashCode

    member private this.InsertIndex =
        if this.Length = 0
        then 0
        else this.Length + min

    member _.Length = map.Count
    member this.IsEmpty = this.Length = 0

    member this.Head =
        if this.IsEmpty
        then failwith "MapQueue is empty"
        else map.[min]

    member this.Pop =
        let newMin = if this.Length = 1 then 0 else min + 1
        let v = map.[min]

        (v, MapQueue(Map.remove min map, newMin))

    member this.Push(v) =
        let newMap = map.Add(this.InsertIndex, v)
        MapQueue(newMap, if this.IsEmpty then 0 else min)

    member this.PushMany(vs) =
        let m =
            Seq.indexed vs
            |> Seq.fold (fun m (idx, v) -> Map.add (idx + this.InsertIndex) v m) map
        MapQueue(m, min)

    member this.Tail =
        if this.IsEmpty
        then failwith "MapQueue is empty"
        else this.Pop |> snd

    override this.GetHashCode() =
        match hashCode with
        | None ->
            let mutable hash = 1
            for x in this do
                hash <- 31 * hash + Unchecked.hash x
            hashCode <- Some hash
            hash
        | Some hash -> hash

    member internal x.CompareHashCodes (y: MapQueue<'T>) =
        match (x.OptHashCode, y.OptHashCode) with
        | (None, None) -> true
        | _ -> Unchecked.equals (x.GetHashCode()) (y.GetHashCode())

    override this.Equals(other) =
        (this :> IStructuralEquatable).Equals(other, StructuralComparisons.StructuralEqualityComparer)

    interface IEnumerable<'T> with
        override this.GetEnumerator() : System.Collections.Generic.IEnumerator<'T> =
            let e =
                Seq.init this.Length (fun x -> x + min)
                |> Seq.map (fun x -> map.[x])
            e.GetEnumerator()

    interface IEnumerable with
        override this.GetEnumerator() =
            (this :> System.Collections.Generic.IEnumerable<'T>).GetEnumerator()
            :> System.Collections.IEnumerator

    interface IReadOnlyCollection<'T> with
        member this.Count = this.Length

    interface System.Collections.IStructuralComparable with
        member x.CompareTo (yobj, comparer) =
            match yobj with
            | :? MapQueue<'T> as y ->
                let lenDiff = x.Length - y.Length
                let xPadding = if lenDiff > 0 then Seq.empty else Seq.replicate -lenDiff None
                let yPadding = if lenDiff < 0 then Seq.empty else Seq.replicate lenDiff None
                let toOptSeq s = Seq.map Some s
                let xSeq = Seq.concat [toOptSeq x; xPadding]
                let ySeq = Seq.concat [toOptSeq y; yPadding]
                Seq.zip xSeq ySeq
                |> Seq.map (fun (l, r) -> comparer.Compare(l, r))
                |> Seq.skipWhile (Unchecked.equals 0)
                |> Seq.tryHead
                |> Option.defaultValue 0
            | _ -> 1

    interface IStructuralEquatable with
        member this.Equals(yobj, _) =
            match yobj with
            | :? MapQueue<'T> as y ->
                if this.Length <> y.Length then false
                else
                    if not (this.CompareHashCodes(y)) then false
                    else Seq.forall2 (Unchecked.equals) this y
            | _ -> false
        member this.GetHashCode(_) =
            this.GetHashCode()

[<RequireQualifiedAccess>]
module MapQueue =
    let empty<'T> = MapQueue(Map.empty, 0)

    let ofSeq s =
        let m =
            Seq.indexed s
            |> Map.ofSeq
        MapQueue(m, 0)

    let pop (q: MapQueue<'T>) = q.Pop

    let push v (q: MapQueue<'T>) =
        q.Push(v)

    let pushMany s (q: MapQueue<'T>) =
        q.PushMany(s)

    let tail (q:MapQueue<'T>) = q.Tail
    let head (q:MapQueue<'T>) = q.Head