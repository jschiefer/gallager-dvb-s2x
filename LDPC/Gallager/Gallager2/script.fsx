// #r "bin/Debug/netcoreapp1.1/Gallager.dll"

#r "bin/Debug/Gallager2.exe"

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Main

let iqDataFileName = "c:/Users/jas/Develop/Github/p4g-toys/LDPC/Data/qpsk_testdata.out"
let message = readTestFile IqFile iqDataFileName Long ModCodLookup.[04uy]

let (b, c) = makeDecodeTables (Long, Rate_1_2)

