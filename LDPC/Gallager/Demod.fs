module Hamstr.Demod

open System
open System.Numerics
open Hamstr.Ldpc.Math
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
    
    /// "Exact" LLR as opposed to approximate LLR
    let computeExactLlr n = 
            
        // Extract bit number n from x (starting at 0)
        let extractBit (x : int) (n : int32) = 
            let mask = 1 <<< n
            byte (x  &&& mask) >>> n

        // Add the gaussian of the error contributation, depending on whether bit n is in S0 or S1
        let accumulateError (s0, s1) (n, (constellationPoint, (error : float))) = 
            let contribution = exp(-1.0 * error / noiseVariance)
            let constellationBit = extractBit constellationPoint n
            match constellationBit with
            | 0uy -> (s0 + contribution, s1)
            | _ -> (s0, s1 + contribution)

        let outputBit = extractBit symbol n
        let llr = 
            let (s0, s1) =
                errors 
                |> Seq.map (fun d -> (n, d))
                |> Seq.fold accumulateError  (0.0, 0.0)
            // This is the "L" in "LLR". Wait, what?
            log(s0 / s1)

        // Our work here is done.
        FloatLLR.Create(llr)

    [ (bitsPerSymbol modulation - 1) .. -1 .. 0 ] |> Seq.map computeExactLlr

