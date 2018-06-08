module Hamstr.Ldpc.Encoder

open FSharp.Numerics
open DvbS2

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
            let dataBit = frame.bits.[dataOffset]
            line
            |> Array.iter (fun accAddress -> 
                // For each element in the accumulator line, modulo-2 add the data bit to the parity accumulator
                let parityIndex = (accAddress + (dataOffset % 360) * codingTableEntry.q ) % nParityBits
                parityBits.[parityIndex] <- parityBits.[parityIndex] <+> dataBit)))
    // Final step: Add the parity bits to each other
    [1 .. nParityBits - 1]
    |> List.iter (fun i -> parityBits.[i] <- parityBits.[i] <+> parityBits.[i - 1])

    parityBits
    