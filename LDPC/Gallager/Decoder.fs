module Hamstr.Decoder

open System
open System.IO
open Hamstr.Ldpc.DvbS2

let testDataFileName = "../Data/qpsk_testdata.out"
let bitFileName = "../Data/qpsk_testdata.bits"

let readFrame frametype modulation stream =
    let nBits = bitsPerFrame frametype
    let bps = bitsPerSymbol modulation
    ()

let readLongQpskFrame stream = readFrame Long M_QPSK stream

let readTestFile fileName =
    use stream = File.OpenRead(fileName)
    use reader = new StreamReader(stream)
    let frame = readLongQpskFrame reader
    frame

[<EntryPoint>]
let main argv =
    let frame = readTestFile testDataFileName
    0 // return an integer exit code
