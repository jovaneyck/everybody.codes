#r "nuget: Unquote"
open Swensen.Unquote

type Complex = { Real: int64; Imaginary: int64 }
module Complex =
    let c real imaginary = { Real = int64 real; Imaginary = int64 imaginary }

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

let upperleft = parse input
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
    let rec loop n acc =
        // printfn $"%d{n}: %A{acc}"
        if n > 100 then
            true
        else
            let squared = Complex.multiply acc acc
            let divided = Complex.divide squared (Complex.c 100_000L 100_000L)
            if divided.Real < -1_000_000L then false
            elif divided.Real > 1_000_000L then false
            elif divided.Imaginary < -1_000_000L then false
            elif divided.Imaginary > 1_000_000L then false
            else 
                let acc' = Complex.add divided c
                loop (n + 1) acc'
    loop 1 (Complex.c 0L 0L)

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

set |> Seq.filter snd |> Seq.length

let run () =
    printf "Testing.."
    test <@ Complex.add (Complex.c 1 1) (Complex.c 2 2) = Complex.c 3 3 @>
    
    test <@ Complex.multiply (Complex.c 1 1) (Complex.c 2 2) = Complex.c 0 4 @>
    
    test <@ Complex.divide (Complex.c 10 12) (Complex.c 2 2) = Complex.c 5 6 @>
    test <@ Complex.divide (Complex.c 11 12) (Complex.c 3 5) = Complex.c 3 2 @>
    test <@ Complex.divide (Complex.c -10 -12) (Complex.c 2 2) = Complex.c -5 -6 @>
    test <@ Complex.divide (Complex.c -11 -12) (Complex.c 3 5) = Complex.c -3 -2 @>
    
    // Mandelbrot tests - points that should be engraved
    test <@ mandelbrot (Complex.c 35630 -64880) = true @>
    test <@ mandelbrot (Complex.c 35630 -64870) = true @>
    test <@ mandelbrot (Complex.c 35640 -64860) = true @>
    test <@ mandelbrot (Complex.c 36230 -64270) = true @>
    test <@ mandelbrot (Complex.c 36250 -64270) = true @>
    
    // Mandelbrot tests - points that should NOT be engraved
    test <@ mandelbrot (Complex.c 35460 -64910) = false @>
    test <@ mandelbrot (Complex.c 35470 -64910) = false @>
    test <@ mandelbrot (Complex.c 35480 -64910) = false @>
    test <@ mandelbrot (Complex.c 35680 -64850) = false @>
    test <@ mandelbrot (Complex.c 35630 -64830) = false @>
    printfn "...done!"

run ()

let c = input |> parse
$"[{c.Real},{c.Imaginary}]"