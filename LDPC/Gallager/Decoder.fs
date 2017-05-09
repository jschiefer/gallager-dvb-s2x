module Hamstr.Ldpc.Decoder

open System
open System.IO
open System.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod

// Go through the whole table and add the info bit to the relevant accumulators.
// Array index is the index of the parity bit, the contents are a list of 
// all the data bits that go into that parity bit.
let makeParityTable () = 
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
    [0 .. (nParityBits - 1)]
    |> List.pairwise
    |> List.iter (fun (p, q) -> parityTable.[q] <- List.append parityTable.[p] parityTable.[q])

    parityTable
    
// let encode 

/// LDPC-decode the frame (which is an array of tuples of bit and LLR)
let decode rate frame =
    let (parityTable, q) = findLdpcParameters rate frame

    [ 0uy; 0uy ]