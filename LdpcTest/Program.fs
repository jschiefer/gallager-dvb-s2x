open Hamstr.Ldpc.Util
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open System.IO
open System

let testPls = 04uy
let dataDir = "../../../../"
let iqDataFileName = dataDir + "Data/qpsk_testdata.out"
let bitFileName = dataDir + "Data/qpsk_testdata.bits"

[<EntryPoint>]
let main _ =
    let dir = Directory.GetCurrentDirectory()
    printfn "Starting program in %s" dir

    let modcod = ModCodLookup.[testPls]
    let frame = readTestFile IqFile iqDataFileName Long modcod

    // let referenceFrame = readTestFile BitFile bitFileName Long modcod

    // compareArrays referenceFrame.bits frame.bits

    let f frame = decode (Long, modcod.LdpcRate) frame 1 |> ignore

    [0 .. 25] |> List.iter (fun _ -> f frame)

    // printfn "Press any key to terminate" 
    // Console.ReadKey() |> ignore

    0
