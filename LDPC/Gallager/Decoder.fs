module Hamstr.Ldpc.Decoder

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod

// Go through the whole table and add the info bit to the relevant acumulators.
// Array index is the index of the parity bit, the contents are a list of 
// all the data bits that go into that parity bit.
let encode ldpcCode frame =
    let codingTableEntry = findLongCodingTableEntry ldpcCode
    let nParityBits = codingTableEntry.NLdpc - codingTableEntry.KLdpc
    let parityBits = Array.create nParityBits FloatLLR.Zero
    
    codingTableEntry.AccTable
    |> List.iteri (fun blockNo line ->
        // Apply each line in the accumulator table to 360 bits
        [0..359] 
        |> List.iter (fun blockOffset ->
            let dataOffset = blockNo * 360 + blockOffset
            let dataBit = frame.data.[dataOffset]
            line
            |> List.iter (fun accAddress -> 
                let parityIndex = (accAddress + (dataOffset % 360) * codingTableEntry.q ) % nParityBits
                parityBits.[parityIndex] <- parityBits.[parityIndex] <+> dataBit)))

    [1 .. nParityBits - 1]
    |> List.iter (fun i -> parityBits.[i] <- parityBits.[i] <+> parityBits.[i - 1])

    parityBits
    

/// LDPC-decode the frame (which is an array of tuples of bit and LLR)
let decode rate frame =
    let codingTableEntry = findLongCodingTableEntry rate

    [ 0uy; 0uy ]