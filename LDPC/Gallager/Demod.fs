module Hamstr.Demod

open System
open System.Numerics
open Hamstr.Ldpc.DvbS2

/// Determine the number of bits per symbol for a given modulation type
let bitsPerSymbol modulation = 
    let log2 x = Math.Log(x, 2.0)
    modulation |> constellation |> Array.length |> float |> log2 |> int32

/// Compute the square of the distance between z and z'
let distanceSquare (z : Complex) (z' : Complex) = 
    let diff = z - z'
    Complex (diff.Real * diff.Real, diff.Imaginary * diff.Imaginary)

/// Does x have a bit set to 1 at position n (starting from 0)?
let hasOneBitAtPosition n x = x &&& (1 <<< n) <> 0

/// Map the received symbol to the closest point in the constellation. Also compute LLR.
let demodulateSymbol modulation (signal : Complex) = 
    let distances = 
        constellation modulation
        |> Array.map (fun s -> (distanceSquare signal))
    

    [ (0uy, 1.0) ]
