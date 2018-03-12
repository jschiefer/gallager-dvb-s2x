#I __SOURCE_DIRECTORY__
#r "bin/Debug/net46/Gallagher.exe"

open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Main

let printBitNodes (bitnodes : BitNode[]) =
    bitnodes
    |> Array.take 50
    |> Array.iteri (fun i b -> printfn "%A %A " i b.value)


let dec typeAndCode frame =
    let (blankBitnodes, checkNodes) = makeDecodeTables typeAndCode
    let bitnodes = initializeBitNodes frame blankBitnodes
    let newChecknodes = updateCheckNodes bitnodes checkNodes 
    let newBitnodes = updateBitnodes bitnodes newChecknodes
    let hd = computeHardDecision newBitnodes
    checkParityEquations hd newChecknodes

let dec2 = dec (Long, Rate_1_2)

let iqDataFileName = @"C:\Users\jas\Develop\Github\p4g-toys\LDPC\Data\qpsk_testdata.out"
let frame = readTestFile IqFile iqDataFileName Long ModCodLookup.[04uy]

let b = frame |> dec2 

let nParityBits = 8
let nDataBits = 8
let origXorLinks = 
    seq { for i in 1 .. nParityBits - 2 do yield (i + nDataBits, i - 1) }  |> Array.ofSeq

let betterXorLinks =
    seq { for i in 0 .. nParityBits - 1 do yield (i + nDataBits, i) }  |> Array.ofSeq

