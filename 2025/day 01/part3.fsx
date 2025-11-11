#r "nuget: Unquote"
open Swensen.Unquote

let input =
    System.IO.File.ReadAllLines $"{__SOURCE_DIRECTORY__}/part3.txt"
    |> List.ofSeq

let example =
    """Vyrdax,Drakzyph,Fyrryn,Elarzris

R3,L2,R3,L3""".Split("\n")
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
        
let swap bounds (names : string list) move =
    let off = offset bounds 0 move
    let updated = names |> List.mapi (fun i n -> if i = 0 then names[off] elif i = off then names[0] else n)
    updated

let solve input =
    let n,m = parse input
    let bounds = Seq.length n
    let finalNames =
        m
        |> Seq.fold (swap bounds) n
    let name = finalNames[0]
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
    
    test <@ solve example = "Drakzyph" @>
    printfn "...done!"

run ()

solve input
