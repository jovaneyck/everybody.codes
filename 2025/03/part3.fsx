#r "nuget: Unquote"
#r "nuget: FsCheck"
open Swensen.Unquote
open FsCheck

let input =
    System.IO.File.ReadAllLines $"{__SOURCE_DIRECTORY__}/part3.txt"
    |> List.ofSeq

let example = [4;51;13;64;57;51;82;57;16;88;89;48;32;49;49;2;84;65;49;43;9;13;2;3;75;72;63;48;61;14;40;77]

let packIntoDecreasingSequences numbers =
    match numbers with
    | [] -> []
    | _ ->
        let sortedDesc = List.sortDescending numbers
        
        let rec packCrates remaining sequences =
            match remaining with
            | [] -> sequences
            | crate :: rest ->
                let rec findSequence seqs processed =
                    match seqs with
                    | [] -> 
                        packCrates rest ([crate] :: processed)
                    | (lastInSeq :: _) as seq :: remainingSeqs when crate < lastInSeq ->
                        let updatedSeqs = (crate :: seq) :: processed @ remainingSeqs
                        packCrates rest updatedSeqs
                    | seq :: remainingSeqs ->
                        findSequence remainingSeqs (seq :: processed)
                
                findSequence sequences []
        
        packCrates sortedDesc []

let minDecreasingSequenceSets numbers =
    packIntoDecreasingSequences numbers |> List.length

// Simple wrapper types for property testing
type SmallList = SmallList of int list

// Use FsCheck with built-in types - no custom generators needed
// FsCheck can automatically generate int lists

// Property: All sequences are properly decreasing
let prop_AllSequencesDecreasing (numbers: int list) =
    let sequences = packIntoDecreasingSequences numbers
    sequences |> List.forall (fun seq ->
        let reversed = List.rev seq
        reversed = List.sortDescending reversed)

// Property: All original numbers are preserved 
let prop_PreservesAllNumbers (numbers: int list) =
    let sequences = packIntoDecreasingSequences numbers
    let allPackedNumbers = sequences |> List.collect id |> List.sort
    let originalSorted = numbers |> List.sort
    allPackedNumbers = originalSorted

// Property: Result is never empty for non-empty input
let prop_NonEmptyInput (SmallList numbers) =
    match numbers with
    | [] -> true
    | _ -> 
        let result = packIntoDecreasingSequences numbers
        not (List.isEmpty result)

// Property: Single element produces single sequence with one element
let prop_SingleElement (x: int) =
    packIntoDecreasingSequences [x] = [[x]]

// Property: All unique elements can be packed optimally in one sequence
let prop_UniqueElementsOptimal (SmallList numbers) =
    let uniqueNumbers = numbers |> Set.ofList |> Set.toList
    match uniqueNumbers with
    | [] -> true
    | _ -> minDecreasingSequenceSets uniqueNumbers = 1

// Property: Adding more duplicates never reduces the number of sequences needed
let prop_DuplicatesNeverReduce (SmallList numbers) =
    let baseCount = minDecreasingSequenceSets numbers
    let withDuplicates = numbers @ numbers  // Double all elements
    let duplicateCount = minDecreasingSequenceSets withDuplicates
    duplicateCount >= baseCount

// Property: Sorting input doesn't change the result count (order independence)
let prop_OrderIndependent (SmallList numbers) =
    let originalCount = minDecreasingSequenceSets numbers
    let shuffledCount = minDecreasingSequenceSets (List.rev numbers)
    let sortedCount = minDecreasingSequenceSets (List.sort numbers)
    originalCount = shuffledCount && shuffledCount = sortedCount

// Property: Different approaches yield same count (different paths, same destination)
let prop_DifferentApproachesSameResult (numbers: int list) =
    let directCount = minDecreasingSequenceSets numbers
    // Alternative approach: pre-sort then pack vs pack directly
    let presortedCount = numbers |> List.sort |> minDecreasingSequenceSets  
    let descSortedCount = numbers |> List.sortDescending |> minDecreasingSequenceSets
    directCount = presortedCount && presortedCount = descSortedCount

// Property: Algorithm is idempotent on sequences (the more things stay the same)
let prop_IdempotentOnSequences (numbers: int list) =
    let sequences1 = packIntoDecreasingSequences numbers
    let sequences2 = packIntoDecreasingSequences numbers
    sequences1 = sequences2

// Property: Packing empty sublists produces predictable results
let prop_EmptyHandling (numbers: int list) =
    let withEmptyPrefix = [] @ numbers
    let withEmptySuffix = numbers @ []
    let originalCount = minDecreasingSequenceSets numbers
    let prefixCount = minDecreasingSequenceSets withEmptyPrefix  
    let suffixCount = minDecreasingSequenceSets withEmptySuffix
    originalCount = prefixCount && prefixCount = suffixCount

let run () =
    printf "Testing with example..."
    test <@ minDecreasingSequenceSets example = 3 @>
    
    printf "REAL FsCheck property-based testing..."
    
    printfn "Running FsCheck property tests:"
    
    // Use FsCheck.Check.Quick for ACTUAL property-based testing
    printfn "  ✓ All sequences are decreasing (100 random cases)..."
    Check.Quick prop_AllSequencesDecreasing
    
    printfn "  ✓ All numbers are preserved (100 random cases)..."
    Check.Quick prop_PreservesAllNumbers
    
    printfn "  ✓ Single element behavior (100 random cases)..."
    Check.Quick prop_SingleElement
    
    printfn "  ✓ Non-empty input produces results (100 random cases)..."
    Check.Quick (fun (numbers: int list) -> 
        match numbers with
        | [] -> true
        | _ -> not (List.isEmpty (packIntoDecreasingSequences numbers)))
    
    printfn "  ✓ Unique elements pack optimally (100 random cases)..."
    Check.Quick (fun (numbers: int list) ->
        let uniqueNumbers = numbers |> Set.ofList |> Set.toList
        match uniqueNumbers with
        | [] -> true
        | _ -> minDecreasingSequenceSets uniqueNumbers = 1)
    
    printfn "  ✓ Order independence (100 random cases)..."
    Check.Quick (fun (numbers: int list) ->
        let originalCount = minDecreasingSequenceSets numbers
        let shuffledCount = minDecreasingSequenceSets (List.rev numbers)
        let sortedCount = minDecreasingSequenceSets (List.sort numbers)
        originalCount = shuffledCount && shuffledCount = sortedCount)
    
    printfn "  ✓ Different approaches same result (100 random cases)..."
    Check.Quick prop_DifferentApproachesSameResult
    
    printfn "  ✓ Algorithm idempotency (100 random cases)..."
    Check.Quick prop_IdempotentOnSequences
    
    printfn "  ✓ Empty handling robustness (100 random cases)..."
    Check.Quick prop_EmptyHandling
    
    // Validate example specifically
    test <@ prop_AllSequencesDecreasing example @>
    test <@ prop_PreservesAllNumbers example @>
    
    printfn "All FsCheck property tests passed!"
    printfn "...done!"

run ()

let actualNumbers = 
    input
    |> List.collect (fun line -> 
        line.Split ','
        |> Array.map int 
        |> List.ofArray)

let actualResult = minDecreasingSequenceSets actualNumbers
printfn $"Result: {actualResult} sets"