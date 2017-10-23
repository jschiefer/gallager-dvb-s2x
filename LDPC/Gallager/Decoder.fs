module Hamstr.Ldpc.Decoder

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2

// Go through the whole table and add the info bit to the relevant acumulators.
// Array index is the index of the parity bit, the contents are a list of 
// all the data bits that go into that parity bit.
let encode ldpcCode frame =
    let codingTableEntry = findCodingTableEntry ldpcCode
    let nParityBits = codingTableEntry.NLdpc - codingTableEntry.KLdpc
    let parityBits = Array.create nParityBits LLR.Zero
    
    codingTableEntry.AccTable
    // Iterate over the lines of the accumulator table
    |> Array.iteri (fun blockNo line ->
        // Apply each line in the accumulator table to 360 bits
        [0..359] 
        |> List.iter (fun blockOffset ->
            let dataOffset = blockNo * 360 + blockOffset
            let dataBit = frame.data.[dataOffset]
            line
            |> Array.iter (fun accAddress -> 
                // For each element in the accumulator line, modulo-2 add the data bit to the parity accumulator
                let parityIndex = (accAddress + (dataOffset % 360) * codingTableEntry.q ) % nParityBits
                parityBits.[parityIndex] <- parityBits.[parityIndex] + dataBit)))
    // Final step: Add the parity bits to each other
    [1 .. nParityBits - 1]
    |> List.iter (fun i -> parityBits.[i] <- parityBits.[i] + parityBits.[i - 1])

    parityBits
    
// Compile the connections between data nodes and check nodes
let makeDecodeTable ldpcCode =
    let codingTableEntry = findCodingTableEntry ldpcCode
    let nParityBits = codingTableEntry.NLdpc - codingTableEntry.KLdpc
    
    codingTableEntry.AccTable
    // Iterate over the lines of the accumulator table
    |> Array.mapi (fun blockNo line ->
        // Apply each line in the accumulator table to 360 bits
        [0..359] 
        |> List.map (fun blockOffset ->
            let dataOffset = blockNo * 360 + blockOffset
            line
            |> Array.map (fun accAddress -> 
                // For each element in the accumulator line, modulo-2 add the data bit to the parity accumulator
                let parityIndex = (accAddress + (dataOffset % 360) * codingTableEntry.q ) % nParityBits
                (dataOffset, parityIndex))))
        |> List.concat
    |> Array.concat


/// LDPC-decode the frame (which is an array of tuples of bit and LLR)
let decode ldpcCode frame =
    let decodeTable = makeDecodeTable ldpcCode
    

    [ 0uy; 0uy ]