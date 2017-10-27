module Hamstr.Ldpc.Decoder

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2


// Compile the connections between data nodes and check nodes
let makeDecodeTables typeAndCode =
    let codingTableEntry = findCodingTableEntry typeAndCode
    let nDataBits = codingTableEntry.KLdpc
    let nBitNodes = codingTableEntry.NLdpc
    let nParityBits = nBitNodes - nDataBits
    
    let accumulatorLinks = 
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
                    let parityIndex = 
                        (accAddress + (dataOffset % 360) * codingTableEntry.q ) % nParityBits
                    (dataOffset, parityIndex))))
            |> List.concat
        |> Array.concat

    // Treat parity bits as data. Kudos to g4guo for the insight!
    let parityLinks = 
        seq { for i in 0 .. nParityBits - 1 do yield (i + nDataBits, i)} 
        |> Array.ofSeq

    // This handles the final XOR during encoding
    let xorLinks = 
        seq { for i in 0 .. nParityBits - 2 do yield (i, i + 1)} 
        |> Array.ofSeq

    // Split up the linkage between bitNodes and checkNodes
    let bitNodes = Array.create nBitNodes ([] : int list) 
    let checkNodes = Array.create nParityBits ([] : int list)

    [| accumulatorLinks; parityLinks; xorLinks |] 
    |> Array.concat 
    |> Array.iter (fun (b, c) -> 
        bitNodes.[b] <- c :: bitNodes.[b]
        checkNodes.[c] <- b :: checkNodes.[c])
    (bitNodes, checkNodes)

/// LDPC-decode the frame (which is an array of tuples of bit and LLR)
type BitNode = {
    /// List of indices of the associated checkNodes
    checkNodes : int list
    /// Current sum of the modulo-2 additions
    sum : LLR
}

type CheckNode = {
    /// List of indices of the associated bitNodes
    bitNodes : int list
    /// Current sum of the modulo-2 additions
    sum : LLR
}

type Foo = {
    /// Index to BitNode or CheckNode
    nodeId : int
    llrContribution : LLR
}

let decode typeAndCode frame =
    let processBitnodes frame bitnodes = 

        ()
    let processCheckNodes frame = 
        ()
    let checkParityEquations frame = 
        ()

    let (bitNodes, checkNodes) = makeDecodeTables typeAndCode


    [ 0uy; 0uy ]