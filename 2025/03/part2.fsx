#r "nuget: Unquote"
open Swensen.Unquote

let input =
    System.IO.File.ReadAllLines $"{__SOURCE_DIRECTORY__}/part2.txt"
    |> List.ofSeq

let example = [4;51;13;64;57;51;82;57;16;88;89;48;32;49;49;2;84;65;49;43;9;13;2;3;75;72;63;48;61;14;40;77]

let smallestExactDecreasingSet targetCount numbers =
    match numbers with
    | [] -> []
    | _ ->
        // Get unique values in ascending order (smallest first)
        let uniqueAscending = 
            numbers 
            |> Set.ofList 
            |> Set.toList 
            |> List.sort
        
        // Take the smallest targetCount unique values
        uniqueAscending
        |> List.take (min targetCount (List.length uniqueAscending))
        |> List.rev  // Return in descending order

let run () =
    printf "Testing.."
    let result = smallestExactDecreasingSet 20 example
    let expectedSum = 781
    let actualSum = List.sum result
    test <@ actualSum = expectedSum @>
    test <@ List.length result = 20 @>
    printfn $"Example result: {result}"
    printfn $"Sum: {actualSum}"
    printfn "...done!"

run ()

let actualNumbers = 
    input
    |> List.collect (fun line -> 
        line.Split ','
        |> Array.map int 
        |> List.ofArray)

let actualResult = smallestExactDecreasingSet 20 actualNumbers
let actualSum = List.sum actualResult
printfn $"Result sum: {actualSum}"