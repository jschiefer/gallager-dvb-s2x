open Hamstr.Ldpc.Util
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open System.IO
open System

let dataDir = "../../../../"
// let dataDir ="./"
let iqDataFileName = dataDir + "Data/qpsk_testdata.out"
let bitFileName = dataDir + "Data/qpsk_testdata.bits"

[<EntryPoint>]
let main _ =
    let dir = Directory.GetCurrentDirectory()
    printfn "Starting program in %s" dir

    let frameType = { frameSize = Long; codeRate = Rate_1_2 }
    let modulation = M_QPSK

    let iqFileType = IqFile(frameType, modulation)
    let frame = readTestFile iqFileType iqDataFileName

    let referenceFileType = BytePerBit(frameType)
    let referenceFrame = readTestFile referenceFileType bitFileName 

    compareArrays referenceFrame.bits frame.bits

    // let f frame = decode frame 1 |> ignore
    let a = checkParity frame
    printfn "checkParityEquations returned %A" a

    // [0 .. 0] |> List.iter (fun _ -> f frame)

    printfn "Press any key to terminate" 
    Console.ReadKey() |> ignore

    0
