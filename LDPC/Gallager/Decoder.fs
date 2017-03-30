module Hamstr.Decoder

open System
open System.IO
open Hamstr.Ldpc.DvbS2

let testDataFileName = "/home/jan/Docker/qpsk_testdata.out"

let readFrame frametype modulation stream =
    let nBits = bitsPerFrame frametype
    let bps = bitsPerSymbol modulation
    ()

let readLongQpskFrame stream = readFrame Long M_QPSK stream

[<EntryPoint>]
let main argv =
    use stream = File.OpenRead(testDataFileName)
    use reader = new StreamReader(stream)
    let frame = readLongQpskFrame stream
    0 // return an integer exit code
