#r "nuget: Unquote"
open Swensen.Unquote

let input =
    System.IO.File.ReadAllLines $"{__SOURCE_DIRECTORY__}/part1.txt"
    |> List.ofSeq

let example = [10; 5; 1; 10; 3; 8; 5; 2; 2]

let maxDecreasingSubsequenceSum numbers =
    match numbers with
    | [] -> 0
    | _ ->
        // Get unique values in descending order
        let uniqueDescending = 
            numbers 
            |> Set.ofList 
            |> Set.toList 
            |> List.sortDescending
        
        // Greedily pick the longest decreasing chain
        let rec buildChain remaining lastPicked =
            match remaining with
            | [] -> []
            | current :: rest ->
                match lastPicked with
                | None -> current :: buildChain rest (Some current)
                | Some last when current < last -> current :: buildChain rest (Some current)
                | Some _ -> buildChain rest lastPicked
        
        let chain = buildChain uniqueDescending None
        List.sum chain

let run () =
    printf "Testing.."
    test <@ maxDecreasingSubsequenceSum example = 29 @>
    printfn "...done!"

run ()

let actualNumbers = 
        input
        |> List.collect (fun line -> 
            line.Split ','
            |> Array.map int 
            |> List.ofArray)
let actualResult = maxDecreasingSubsequenceSum actualNumbers
printfn $"Result: {actualResult}"