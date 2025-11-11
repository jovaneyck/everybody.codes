#r "nuget: Unquote"
open Swensen.Unquote

let input =
    System.IO.File.ReadAllLines $"{__SOURCE_DIRECTORY__}/part2.txt"
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

let offset bounds pos move =
    let diff = (pos + move) % bounds
    if diff < 0 then
        bounds + diff
    else
        diff

let solve input =
    let n,m = parse input
    let bounds = Seq.length n
    let finalPosition =
        m
        |> Seq.fold (offset bounds) 0
    let name = n[finalPosition]
    name
    
let run () =
    printf "Testing.."
    test <@ offset 100 0 1 = 1 @>
    test <@ offset 100 0 2 = 2 @>
    test <@ offset 100 0 100 = 0 @>
    test <@ offset 100 50 -1 = 49 @>
    test <@ offset 100 50 -2 = 48 @>
    test <@ offset 100 0 -1 = 99 @>
    test <@ offset 100 0 -2 = 98 @>
    
    test <@ solve example = "Elarzris" @>
    printfn "...done!"

run ()

solve input
