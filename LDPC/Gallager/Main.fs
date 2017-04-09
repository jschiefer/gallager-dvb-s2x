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
    printfn "read %A %A" real imaginary
    Complex(float real, float imaginary)

let readSymbol reader modulation = 
    let noiseVariance = 0.2
    readComplexNumber reader |> demodulateSymbol noiseVariance modulation

let readFrame fileType frametype modulation reader =
    match fileType with
    | IqFile ->
        [ 1 .. bitsPerFrame frametype ] 
        |> Seq.collect (fun _ -> readSymbol reader modulation)
    | BitFile ->
        [ 1 .. bitsPerFrame frametype ] 
        |> Seq.map (fun _ -> (reader.ReadByte(), 0.0))

let readTestFile fileType fileName frameType modulation =
    use stream = File.OpenRead(fileName)
    use reader = new BinaryReader(stream)
    readFrame fileType frameType modulation reader 
    |> List.ofSeq

[<EntryPoint>]
let main argv =
    let modcod = ModCodLookup.[testPls]
    let frame = readTestFile IqFile iqDataFileName Long modcod.Modulation 
    let referenceFrame = readTestFile BitFile bitFileName Long modcod.Modulation 

    let foo = frame |> List.compareWith (fun a b -> if fst a = fst b then 0 else 1) referenceFrame

    // printfn "Foo is %d" foo
    // let result = decode (1, 2) frame
    printfn "%A" frame
    printfn "%A" referenceFrame
    0 // return an integer exit code
