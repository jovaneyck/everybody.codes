#r "nuget: Unquote"
open Swensen.Unquote

let input =
    System.IO.File.ReadAllLines $"{__SOURCE_DIRECTORY__}/part1.txt"
    |> List.ofSeq

let example =
    """Vyrdax,Drakzyph,Fyrryn,Elarzris

R3,L2,R3,L1""".Split("\n")
    |> Array.map (fun s -> s.Trim())
    |> List.ofSeq

let parse (input: string list) =
    let names =
        input.[0].Split(',')
        |> List.ofSeq

    let moves =
        input.[2].Split(',')
        |> Array.map (fun s -> s.Trim())
        |> Array.map (fun x -> if x.StartsWith('L') then -1 * (int (x.Substring(1))) else (int (x.Substring(1))))
        |> List.ofSeq

    names, moves

let solve input =
    let n,m = parse input
    let bounds = Seq.length n
    let finalPosition =
        m
        |> Seq.fold (fun pos move ->
            let diff = pos + move
            if diff < 0 then 0
            elif diff >= bounds then bounds - 1
            else diff) 0
    let name = n[finalPosition]
    name
    
let run () =
    printf "Testing.."
    test <@ solve example = "Fyrryn" @>
    printfn "...done!"

run ()

solve input