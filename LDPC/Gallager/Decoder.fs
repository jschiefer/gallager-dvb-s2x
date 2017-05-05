module Hamstr.Ldpc.Decoder

open System
open System.IO
open System.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod



// Go through the whole table and add the info bit to the relevant accumulators
let makeTable = 
    let (accumulatorTable, codeParam) = findLongLdpcParameters Rate_1_2
    let nParityBits = codeParam.NLdpc - codeParam.KLdpc
    let parityTable = Array.create nParityBits List.empty<int>
    accumulatorTable
    |> List.iter (fun line ->
        line 
        |> List.iter (fun x -> 
            [0..359] 
            |> List.iter (fun m ->
                let index = (x + m * codeParam.q) % nParityBits
                parityTable.[index] <- m::parityTable.[index])))
    [0..nParityBits - 1]
    |> Seq.pairwise
    |> Seq.iter (fun (p, q) -> parityTable.[p] <- List.append parityTable.[p] parityTable.[q])
    parityTable
    

/// LDPC-decode the frame (which is an array of tuples of bit and LLR)
let decode rate frame =
    let (parityTable, q) = findLdpcParameters rate frame

    [ 0uy; 0uy ]