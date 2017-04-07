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

let readSymbol reader modulation = 
    readComplexNumber reader |> demodulateSymbol modulation

let readFrame fileType frametype modulation reader =
    match fileType with
    | IqFile ->
        [ 1 .. bitsPerFrame frametype ] 
        |> List.collect (fun _ -> readSymbol reader modulation)
    | BitFile ->
        [ 1 .. bitsPerFrame frametype ] 
        |> List.map (fun _ -> (reader.ReadByte(), 1.0))

let readTestFile fileType fileName frameType modulation =
    use stream = File.OpenRead(fileName)
    use reader = new BinaryReader(stream)
    readFrame fileType frameType modulation reader

[<EntryPoint>]
let main argv =
    let modcod = ModCodLookup.[testPls]
    let frame = readTestFile IqFile iqDataFileName Long modcod.Modulation |> Array.ofList
    let referenceFrame = readTestFile BitFile bitFileName Long modcod.Modulation |> Array.ofList
    let result = decode (1, 2) frame
    printfn "%A %d" frame frame.Length
    0 // return an integer exit code
