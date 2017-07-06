module Hamstr.Ldpc.Main

open System
open System.IO
open System.Numerics
open Hamstr.Ldpc.Math
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

let readFrame fileType frameType modcod reader =
    let frameLength = frameType |> FrameType.BitLength
    let codingTableEntry = findCodingTableEntry frameType modcod.LdpcRate
    let nDataBits = codingTableEntry.KLdpc
    let nParityBits = codingTableEntry.NLdpc - nDataBits

    let (data, parity) = 
        match fileType with
        | IqFile ->
            let bps  = bitsPerSymbol modcod.Modulation
            let data =
                [ 1 .. nDataBits / bps ] 
                |> Seq.collect (fun _ -> readSymbol reader modcod.Modulation)
            let parity = 
                [ 1 .. nParityBits / bps ] 
                |> Seq.collect (fun _ -> readSymbol reader modcod.Modulation)
            data, parity
        | BitFile ->
            let data =
                [ 1 .. nDataBits ] 
                |> Seq.map (fun _ -> FloatLLR.Create(reader.ReadByte()))
            let parity =
                [ 1 .. nParityBits ] 
                |> Seq.map (fun _ -> FloatLLR.Create(reader.ReadByte()))
            data, parity
    Some({ frameType = frameType; ldpcCode = modcod.LdpcRate; data = Array.ofSeq data; parity = Some(Array.ofSeq parity) })

let readTestFile fileType fileName frameType modcod =
    use stream = File.OpenRead(fileName)
    use reader = new BinaryReader(stream)
    readFrame fileType frameType modcod reader 

let checkForBitErrors referenceFrame frame =
    let comparer a b =
        // TODO: Convert to boolean
        printfn "reference: %A frame %A" a b
        if a = b then 0 else 1

    frame 
    |> Seq.compareWith comparer referenceFrame

[<EntryPoint>]
let main argv =
    let frameLength = Long |> FrameType.BitLength 
    let modcod = ModCodLookup.[testPls]
    let frame = readTestFile IqFile iqDataFileName Long modcod
    let referenceFrame = readTestFile BitFile bitFileName Long modcod
    let decodedFrame = 
        match frame with
        | Some(x) -> decode Rate_1_2 x
        | _ -> []
    let foo = checkForBitErrors referenceFrame.Value.data frame.Value.data
    printfn "Comparison result is %A" foo
    (*
    let a = makeParityTable () 
    [0 .. 5] |> List.iteri (fun x i -> printfn "a.[%d] = %A" i x)
    *)

    0
