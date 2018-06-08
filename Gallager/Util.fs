module Hamstr.Ldpc.Util

open System.IO
open System.Numerics
open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod

type FileType = 
    | BitFile
    | IqFile

let readComplexNumber (reader:BinaryReader) = 
    let real = reader.ReadSingle()
    let imaginary = reader.ReadSingle()
    Complex(float real, float imaginary)

// FIXME The demod should happen on a per-frame basis, not per-symbol
let readSymbol reader modulation = 
    let noiseVariance = 0.2
    readComplexNumber reader 
    |> demodulateSymbol noiseVariance modulation

let readFrame fileType frameType modcod reader =
    let codingTableEntry = findCodingTableEntry (frameType, modcod.LdpcRate)
    let nBits = codingTableEntry.NLdpc

    let data = 
        match fileType with
        | IqFile ->
            let bps  = bitsPerSymbol modcod.Modulation
            let data =
                [ 1 .. nBits / bps ] 
                |> Seq.collect (fun _ -> readSymbol reader modcod.Modulation)
            data
        | BitFile ->
            let data =
                [ 1 .. nBits ] 
                |> Seq.map (fun _ -> LLR.Create(reader.ReadByte()))
            data
    { frameType = frameType; ldpcCode = modcod.LdpcRate; bits = Array.ofSeq data }

let readTestFile fileType fileName frameType modcod =
    use stream = File.OpenRead(fileName)
    use reader = new BinaryReader(stream)
    readFrame fileType frameType modcod reader 

let checkForBitErrors refSeq otherSeq =
    let comparer (a:LLR) (b:LLR) =
        if a.ToBool = b.ToBool then 0 else 1
    otherSeq 
    |> Seq.compareWith comparer refSeq

let compareArrays (refArray: LLR array) (otherArray : LLR array) = 
    Array.zip refArray otherArray
    |> Array.iteri (fun i (a, b) -> 
        if a.ToBool = b.ToBool then () else printfn "Difference in element %A" i)
