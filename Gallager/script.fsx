#I __SOURCE_DIRECTORY__
#r "bin/Debug/net461/Gallager.dll"
#r "Kludge/netstandard.dll"

open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Util

let decodeHalfRate = decode (Long, Rate_1_2) 1

let bitFileName = @"C:\Users\jas\Develop\p4g\gallager-dvb-s2x\Data\qpsk_testdata.bits"

// Read the file
let frame = readTestFile BitFile bitFileName Long ModCodLookup.[04uy]
let foo = decodeHalfRate frame |> ignore

// Create empty table structure that fits the modcod
let (blankBitnodes, checkNodes) = makeDecodeTables (Long, Rate_1_2)

// Fill in the bits from the message
let bitnodes = initializeBitNodes frame blankBitnodes

checkParityEquations bitnodes checkNodes 
bitnodes |> Array.length

blankBitnodes.[0]
checkNodes.[0]
