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

let upperleft = parse example
let width = 1_000
let sampleSize = 100
let diff = width / sampleSize

let sampleGrid =
    seq {
        for diff_imag in 0 .. diff .. width do
        for diff_real in 0 .. diff .. width do
            Complex.add upperleft (Complex.c diff_real diff_imag)
    }
    
let mandelbrot (c: Complex) =
    let rec loop n checkResult =
        // printfn $"%d{n}: %A{checkResult}"
        if n = 100 then
            true
        else
            let acc' = Complex.add (Complex.divide (Complex.multiply checkResult checkResult) (Complex.c 100_000 100_000)) c
            if acc'.Real < -1_000_000 then false
            elif acc'.Real > 1_000_000 then false
            elif acc'.Imaginary < -1_000_000 then false
            elif acc'.Imaginary > 1_000_000 then false
            else loop (n + 1) acc'
    loop 1 (Complex.c 0 0)

mandelbrot (Complex.c 35460 -64910)

let set =
    sampleGrid
    |> Seq.map (fun loc -> (loc, mandelbrot loc))
    |> Seq.toList
    
let render (grid : ('a * bool) list) = 
    [for row in 0 .. sampleSize do
         [for col in 0 .. sampleSize do
             let idx = row * (sampleSize + 1) + col
             if snd grid.[idx] then "X" else "."] |> String.concat ""
    ] |> String.concat "\n"
    |> printfn "%s"
render set

let run () =
    printf "Testing.."
    test <@ Complex.add (Complex.c 1 1) (Complex.c 2 2) = Complex.c 3 3 @>
    
    test <@ Complex.multiply (Complex.c 1 1) (Complex.c 2 2) = Complex.c 0 4 @>
    
    test <@ Complex.divide (Complex.c 10 12) (Complex.c 2 2) = Complex.c 5 6 @>
    test <@ Complex.divide (Complex.c 11 12) (Complex.c 3 5) = Complex.c 3 2 @>
    test <@ Complex.divide (Complex.c -10 -12) (Complex.c 2 2) = Complex.c -5 -6 @>
    test <@ Complex.divide (Complex.c -11 -12) (Complex.c 3 5) = Complex.c -3 -2 @>
    printfn "...done!"

run ()

let c = input |> parse
$"[{c.Real},{c.Imaginary}]"