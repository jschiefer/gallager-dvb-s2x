module Hamstr.Ldpc.Main

open System
open System.IO
open System.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod
open Hamstr.Ldpc.Decoder

type FileType = 
    | BitFile
    | IqFile

let testPls = 04uy
let iqDataFileName = "../Data/qpsk_testdata.out"
let bitFileName = "../Data/qpsk_testdata.bits"

let readComplexNumber (reader:BinaryReader) = 
    let real = reader.ReadSingle()
    let imaginary = reader.ReadSingle()
    Complex(float real, float imaginary)

// FIXME The demod should happen on a per-frame basis, not per-symbol
let readSymbol reader modulation = 
    let noiseVariance = 0.2
    readComplexNumber reader 
    |> demodulateSymbol noiseVariance modulation

let readFrame fileType frameLength modcod reader =
    let sequence = 
        match fileType with
        | IqFile ->
            let nSamplesToRead = frameLength / bitsPerSymbol modcod.Modulation
            [ 1 .. nSamplesToRead ] 
            |> Seq.collect (fun _ -> readSymbol reader modcod.Modulation)
        | BitFile ->
            [ 1 .. frameLength ] 
            |> Seq.map (fun _ -> (reader.ReadByte(), 0.0))
        |> List.ofSeq
    Some({ LdpcCode = modcod.LdpcRate; data = sequence })

let readTestFile fileType fileName frameType modcod =
    use stream = File.OpenRead(fileName)
    use reader = new BinaryReader(stream)
    readFrame fileType frameType modcod reader 

let checkForBitErrors referenceFrame frame =
    let comparer a b =
        if fst a = fst b then 0 else 1

    let foo = frame |> List.compareWith comparer referenceFrame
    ()

[<EntryPoint>]
let main argv =
    let frameLength = BitsPerFrame.Long |> int32
    let modcod = ModCodLookup.[testPls]
    let frame = readTestFile IqFile iqDataFileName frameLength modcod
    let referenceFrame = readTestFile BitFile bitFileName frameLength modcod
    let decodedFrame = 
        match frame with
        | Some(x) -> decode Rate_1_2 x
        | _ -> []
(*
    let a = makeParityTable () 
    [0 .. 5] |> List.iteri (fun x i -> printfn "a.[%d] = %A" i x)
*)

    0
