module Hamstr.Ldpc.Util

open System.IO
open System.Numerics
open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod

type FileType = 
    | BytePerBit of FrameType
    | IqFile of FrameType * Modulation

let readFrame fileType (reader : BinaryReader) =
    let bitsPerFrame frameType =
        let codingTableEntry = findCodingTableEntry frameType
        codingTableEntry.NLdpc

    match fileType with
    | BytePerBit(frameType) ->
        let byteCount = bitsPerFrame frameType
        let data =
            [ 1 .. byteCount ] 
            |> Seq.map (fun _ -> LLR.Create(reader.ReadByte()))
        { frameType = frameType; bits = Array.ofSeq data }

    | IqFile(frameType, modulation) ->
        // FIXME The demod should happen on a per-frame basis, not per-symbol
        let readSymbol reader modulation = 
            let noiseVariance = 0.2

            let readComplexNumber (reader : BinaryReader) = 
                let real = reader.ReadSingle()
                let imaginary = reader.ReadSingle()
                Complex(float real, float imaginary)

            readComplexNumber reader 
            |> demodulateSymbol noiseVariance modulation

        let symbolCount = bitsPerFrame frameType / bitsPerSymbol modulation
        let bits =
            [ 1 .. symbolCount ] 
            |> Seq.collect (fun _ -> readSymbol reader modulation)
        { frameType = frameType; bits = Array.ofSeq bits }

let readTestFile fileType fileName =
    use stream = File.OpenRead(fileName)
    use reader = new BinaryReader(stream)
    readFrame fileType reader 

let checkForBitErrors refSeq otherSeq =
    let comparer (a:LLR) (b:LLR) =
        if a.ToBool = b.ToBool then 0 else 1
    otherSeq 
    |> Seq.compareWith comparer refSeq

let compareArrays (refArray: LLR array) (otherArray : LLR array) = 
    Array.zip refArray otherArray
    |> Array.iteri (fun i (a, b) -> 
        if a.ToBool = b.ToBool then () else printfn "Difference in element %A" i)

