module Hamstr.Ldpc.Decoder

open System
open System.IO
open System.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod

// Modulo2-addition operator 
let inline (<+>) a b = (a + b) &&& 1

// Go through the whole table and add the info bit to the relevant acumulators.
// Array index is the index of the parity bit, the contents are a list of 
// all the data bits that go into that parity bit.
let makeParityTable () = 
    let codeParam = findLongCodingTableEntry Rate_1_2
    let nParityBits = codeParam.NLdpc - codeParam.KLdpc
    let parityTable = Array.create nParityBits List.empty<int>
    codeParam.AccTable
    |> List.iter (fun line ->
        line 
        |> List.iter (fun x -> 
            [0..359] 
            |> List.iter (fun m ->
                let index = (x + m * codeParam.q) % nParityBits
                parityTable.[index] <- m::parityTable.[index])))
    (*
    [0 .. (nParityBits - 1)]
    |> List.pairwise
    |> List.iter (fun (p, q) -> parityTable.[q] <- List.append parityTable.[p] parityTable.[q])
    *)

    parityTable
    
let encode rate frame =
    let codingTableEntry = findLongCodingTableEntry rate
    let nParityBits = codingTableEntry.NLdpc - codingTableEntry.KLdpc
    let parityTable = Array.create nParityBits 0
    let data = Array.ofSeq frame.data 
    codingTableEntry.AccTable
    |> List.iter (fun line ->
        line 
        |> List.iter (fun x -> 
            [0..359] 
            |> List.iter (fun m ->
                let parityIndex = (x + m * codingTableEntry.q) % nParityBits
                let dataIndex = 0
                parityTable.[parityIndex] <- parityTable.[parityIndex] <+> data.[dataIndex])))

    ()

/// LDPC-decode the frame (which is an array of tuples of bit and LLR)
let decode rate frame =
    let codingTableEntry = findLongCodingTableEntry rate

    [ 0uy; 0uy ]