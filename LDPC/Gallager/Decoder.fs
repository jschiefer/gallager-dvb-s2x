module Hamstr.Ldpc.Decoder

open System
open System.IO
open System.Numerics
open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod

// Go through the whole table and add the info bit to the relevant acumulators.
// Array index is the index of the parity bit, the contents are a list of 
// all the data bits that go into that parity bit.
let encode rate frame =
    let codingTableEntry = findLongCodingTableEntry rate
    let nParityBits = codingTableEntry.NLdpc - codingTableEntry.KLdpc
    let parityTable = Array.create nParityBits Unchecked.defaultof<FloatLLR>
    
    codingTableEntry.AccTable
    |> List.iter (fun line ->       // line in the parity table
        line 
        |> List.iter (fun bitAddress ->      // Line item
            [0..359] 
            |> List.iter (fun m ->
                let parityIndex = (bitAddress + m * codingTableEntry.q) % nParityBits
                let dataIndex = 0
                parityTable.[parityIndex] <- parityTable.[parityIndex])))

    let data = Array.ofSeq frame.data 



/// LDPC-decode the frame (which is an array of tuples of bit and LLR)
let decode rate frame =
    let codingTableEntry = findLongCodingTableEntry rate

    [ 0uy; 0uy ]