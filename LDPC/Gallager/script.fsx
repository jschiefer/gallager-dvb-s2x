#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Main

let iqDataFileName = "../Data/qpsk_testdata.out"
let message = readTestFile IqFile iqDataFileName Long ModCodLookup.[04uy]
let nBitnodes = 64800
let nChecknodes = 32400
let frameType = (Long, Rate_1_2)

let b = decode frameType message 