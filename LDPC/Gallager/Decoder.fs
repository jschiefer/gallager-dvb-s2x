module Hamstr.Ldpc.Decoder

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod

// Go through the whole table and add the info bit to the relevant acumulators.
// Array index is the index of the parity bit, the contents are a list of 
// all the data bits that go into that parity bit.
let encode rate frame =
    let codingTableEntry = findLongCodingTableEntry rate
    let nParityBits = codingTableEntry.NLdpc - codingTableEntry.KLdpc
    let parityTable = Array.create nParityBits FloatLLR.Zero
    
    codingTableEntry.AccTable
    |> List.iteri (fun i line ->       // line in the parity table
        line 
        |> List.iter (fun x ->      // Line item
            [0..359] 
            |> List.iter (fun m ->
                let parityIndex = (x + m * codingTableEntry.q) % nParityBits
                let dataIndex = i * 360 + m
                printfn "parityIndex = %A, dataIndex = %A" parityIndex dataIndex
                parityTable.[parityIndex] <- parityTable.[parityIndex] <+> frame.data.[dataIndex])))
    // TODO: Iterate over the table and do the concatenation

    parityTable

/// LDPC-decode the frame (which is an array of tuples of bit and LLR)
let decode rate frame =
    let codingTableEntry = findLongCodingTableEntry rate

    [ 0uy; 0uy ]