open Hamstr.Ldpc.Util
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open System.IO

let testPls = 04uy
let iqDataFileName = "Data/qpsk_testdata.out"
let bitFileName = "Data/qpsk_testdata.bits"

[<EntryPoint>]
let main _ =
    let dir = Directory.GetCurrentDirectory()
    printfn "Starting program in %s" dir

    let modcod = ModCodLookup.[testPls]
    let frame = readTestFile IqFile iqDataFileName Long modcod
    let referenceFrame = readTestFile BitFile bitFileName Long modcod

    printParity referenceFrame.bits frame.bits

    let foo = decode (Long, modcod.LdpcRate) frame
    0
