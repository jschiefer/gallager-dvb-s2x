#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open System
open System.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod

/// One meager test symbol
let sym = 
    [ 
        Complex(-0.640049576759338, 0.511995136737823); 
        Complex(-0.56793886423111, -0.883610665798187) 
    ]

let distances = 
    constellation M_QPSK
    |> Array.mapi (fun i s -> (i, distanceSquare s sym.[0]))
let symbol = distances |> Array.sortBy (fun (_, d) -> d) |> Array.head |> fst

/// Accumulate the distances into the numerator and denominator
let accumulateDistance (numerator, denominator) (mask, (constellationPoint, distance)) = 
    let transform x = x
    // Add, according to whether the bit is in S0 or S1
    match constellationPoint &&& mask with
    | 0 -> (numerator + transform distance, denominator)
    | _ -> (numerator, denominator + transform distance)

let dd = 
    [ 1 .. bitsPerSymbol M_QPSK]
    |> Seq.map (fun bitNo -> 
        let mask = 1 <<< (bitNo - 1)
        let bit = if mask &&& symbol <> 0 then 0uy else 1uy
        ( bit , 
            distances 
            |> Seq.map (fun d -> (mask, d))
            |> Seq.fold accumulateDistance  (0.0, 0.0)))
    |> Seq.map (fun (b, x) -> (b, log(fst x / snd x)))

dd