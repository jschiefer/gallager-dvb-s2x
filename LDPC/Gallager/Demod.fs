module Hamstr.Demod

open System
open System.Numerics
open Hamstr.Ldpc.DvbS2

/// Determine the number of bits per symbol for a given modulation type
let bitsPerSymbol modulation = 
    let log2 x = Math.Log(x, 2.0)
    modulation |> getConstellation |> Array.length |> float |> log2 |> int32

/// Map the received signal to the nearest point in the constellation and compute LLR.
let demodulateSymbol (noiseVariance : float) (modulation : Modulation) (signal : Complex) = 

    /// Compute the square of the distance between z and z'
    let magSquared (z : Complex) (z' : Complex) = 
        let diff = z - z'
        diff.Real * diff.Real + diff.Imaginary * diff.Imaginary

    // Compute distances to all ideal constellation points
    let constellation = modulation |> getConstellation
    let errors = constellation |> Array.mapi (fun label point -> (label, magSquared point signal))
    
    // The one with the smallest error is our output symbol
    let symbol = errors |> Array.sortBy (fun (_, d) -> d) |> Array.head |> fst
    
    // Extract bit number n from x (starting at 0)
    let extractBit (x : int) (n : int32) = 
        let mask = 1 <<< n
        byte (x  &&& mask) >>> n

    let computeExactLlr n = 
            
        let accumulateError (s0, s1) (n, (constellationPoint, (error : float))) = 
            // Add, according to whether the bit at index n is in S0 or S1
            let constellationBit = extractBit constellationPoint n
            let contribution = exp(-1.0 * error / noiseVariance)
            match constellationBit with
            | 0uy -> (s0 + contribution, s1)
            | _ -> (s0, s1 + contribution)

        let bit = extractBit symbol n
        let (s0, s1) =
            errors 
            |> Seq.map (fun d -> (n, d))
            |> Seq.fold accumulateError  (0.0, 0.0)
        let llr = log(s0 / s1)
        (bit, llr)

    [ (bitsPerSymbol modulation - 1) .. -1 .. 0 ] 
    |> Seq.map computeExactLlr

