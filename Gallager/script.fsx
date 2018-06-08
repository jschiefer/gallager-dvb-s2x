#I __SOURCE_DIRECTORY__
#r "bin/Debug/net461/Gallager.dll"
#r "Kludge/netstandard.dll"

open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Util

let printBitNodes (bitnodes : BitNode[]) =
    bitnodes
    |> Array.take 50
    |> Array.iteri (fun i b -> printfn "%A %A " i b.value)


let dec typeAndCode frame =
    let (blankBitnodes, checkNodes) = makeDecodeTables typeAndCode
    let bitnodes = initializeBitNodes frame blankBitnodes
    let newChecknodes = updateCheckNodes bitnodes checkNodes 
    updateBitnodes bitnodes newChecknodes
    let hd = computeHardDecision bitnodes
    checkParityEquations hd newChecknodes

let decodeHalfRate = dec (Long, Rate_1_2)

let bitFileName = @"C:\Users\jas\Develop\p4g\gallager-dvb-s2x\Data\qpsk_testdata.bits"
let iqFrame = readTestFile BitFile bitFileName Long ModCodLookup.[04uy]
let foo = decodeHalfRate iqFrame

let checkParityEquations2 (bitnodes : BitNode []) (nDatabits : int) =
    let parityEquation i = 
        let paritybits =
            bitnodes.[i].checkNodeIds
            |> List.map (fun ci -> bitnodes.[ci + nDatabits] )

        bitnodes.[i] :: paritybits
        |> List.map (fun b -> b.value.ToBool)
        |> List.reduce (<>) 

    [0..(nDatabits - 1)]
    |> List.map parityEquation

// Read the file
let frame = readTestFile BitFile bitFileName Long ModCodLookup.[04uy]
// Create the empty table structure that fits the modcod
let (blankBitnodes, checkNodes) = makeDecodeTables (Long, Rate_1_2)
// Fill in the bits from the message
let bitnodes = initializeBitNodes frame blankBitnodes
checkParityEquations2 bitnodes 32400 
bitnodes |> Array.length
