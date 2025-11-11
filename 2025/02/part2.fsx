#r "nuget: Unquote"
open Swensen.Unquote

type Complex = { Real: int; Imaginary: int }
module Complex =
    let c real imaginary = { Real = real; Imaginary = imaginary }

    let add c1 c2 = c (c1.Real + c2.Real) (c1.Imaginary + c2.Imaginary)
    let multiply c1 c2 =
        c
            (c1.Real * c2.Real - c1.Imaginary * c2.Imaginary)
            (c1.Real * c2.Imaginary + c1.Imaginary * c2.Real)
    let divide c1 c2 =
        c (c1.Real / c2.Real) (c1.Imaginary / c2.Imaginary)
        
let input =
    System.IO.File.ReadAllText $"{__SOURCE_DIRECTORY__}/part2.txt"

let example = """A=[35300,-64910]"""

let parse (input : string) =
    let parts = input.Trim().Split('=')
    let coords =
        parts.[1].Trim('[', ']').Split(',')
        |> Array.map int
    Complex.c coords.[0] coords.[1]

let n = parse example

let solve a =
    let rec loop count result =
        if count = 0 then
            result
        else
            let result = Complex.multiply result result
            let result = Complex.divide result (Complex.c 10 10)
            let result = Complex.add result a
            loop (count - 1) result
    loop 3 (Complex.c 0 0)

let run () =
    printf "Testing.."
    test <@ Complex.add (Complex.c 1 1) (Complex.c 2 2) = Complex.c 3 3 @>
    
    test <@ Complex.multiply (Complex.c 1 1) (Complex.c 2 2) = Complex.c 0 4 @>
    
    test <@ Complex.divide (Complex.c 10 12) (Complex.c 2 2) = Complex.c 5 6 @>
    test <@ Complex.divide (Complex.c 11 12) (Complex.c 3 5) = Complex.c 3 2 @>
    test <@ Complex.divide (Complex.c -10 -12) (Complex.c 2 2) = Complex.c -5 -6 @>
    test <@ Complex.divide (Complex.c -11 -12) (Complex.c 3 5) = Complex.c -3 -2 @>
    test <@ example |> parse |> solve = Complex.c 357 862 @>
    printfn "...done!"

run ()

let c = input |> parse |> solve
$"[{c.Real},{c.Imaginary}]"