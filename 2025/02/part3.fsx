#r "nuget: Unquote"
open Swensen.Unquote

type Complex = { Real: int64; Imaginary: int64 }
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
    System.IO.File.ReadAllText $"{__SOURCE_DIRECTORY__}/part3.txt"

let example = """A=[35300,-64910]"""

let parse (input : string) =
    let parts = input.Trim().Split('=')
    let coords =
        parts.[1].Trim('[', ']').Split(',')
        |> Array.map int
    Complex.c coords.[0] coords.[1]
    
let mandelbrot (c: Complex) =
    let rec loop n result =
        // printfn $"%d{n}: %A{acc}"
        if n > 100 then
            true
        else
            let squared = Complex.multiply result result
            let divident = Complex.c 100_000L 100_000L
            let divided = Complex.divide squared divident
            let acc' = Complex.add divided c
            if acc'.Real < -1_000_000L then false
            elif acc'.Real > 1_000_000L then false
            elif acc'.Imaginary < -1_000_000L then false
            elif acc'.Imaginary > 1_000_000L then false
            else 
                
                loop (n + 1) acc'
    
    let result = (Complex.c 0L 0L)
    loop 1 result

let solve sampleSize input =
    let upperleft = parse input
    let width = 1_000
    let diff = width / sampleSize

    let sampleGrid =
        seq {
            for diff_imag in 0 .. diff .. width do
            for diff_real in 0 .. diff .. width do
                Complex.add upperleft (Complex.c diff_real diff_imag)
        }
        
    let set =
        sampleGrid
        |> Seq.map (fun loc -> (loc, mandelbrot loc))
        |> Seq.toList
        
    set
    
let render sampleSize (grid : ('a * bool) list) = 
    [for row in 0 .. sampleSize do
         [for col in 0 .. sampleSize do
             let idx = row * (sampleSize + 1) + col
             if snd grid.[idx] then "X" else "."] |> String.concat ""
    ] |> String.concat "\n"
    |> printfn "%s"

let sampleSize = 1_000

let set = solve sampleSize input
// render sampleSize set
set |> Seq.filter snd |> Seq.length

let run () =
    printf "Testing.."
    test <@ Complex.add (Complex.c 1 1) (Complex.c 2 2) = Complex.c 3 3 @>
    test <@ Complex.add (Complex.c 2 5) (Complex.c 3 7) = Complex.c 5 12 @>
    test <@ Complex.add (Complex.c -2 5) (Complex.c 10 -1) = Complex.c 8 4 @>
    test <@ Complex.add (Complex.c -1 -2) (Complex.c -3 -4) = Complex.c -4 -6 @>
    
    test <@ Complex.multiply (Complex.c 1 1) (Complex.c 2 2) = Complex.c 0 4 @>
    test <@ Complex.multiply (Complex.c 2 5) (Complex.c 3 7) = Complex.c -29 29 @>
    test <@ Complex.multiply (Complex.c -2 5) (Complex.c 10 -1) = Complex.c -15 52 @>
    test <@ Complex.multiply (Complex.c -1 -2) (Complex.c -3 -4) = Complex.c -5 10 @>
    
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