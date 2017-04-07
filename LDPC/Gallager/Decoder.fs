module Hamstr.Ldpc.Decoder

open System
open System.IO
open System.Numerics
open Hamstr.Ldpc.DvbS2

let bitsPerSymbol modulation = 
    let log2 x = Math.Log(x, 2.0)
    modulation |> constellation |> Array.length |> float |> log2 |> int32

let demodulateSymbol modulation (samples:seq<Complex>) = 
    let c = constellation modulation
    printfn "%A" samples
    [ 0uy ]

let decode (rate : int * int) (frame : byte[]) =
    let parityTable = findParityTable rate
    ()