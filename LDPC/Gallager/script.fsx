#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Main

let message = readTestFile IqFile iqDataFileName Long ModCodLookup.[04uy]


// Each mapping should be captured in an array of lists
// checknodes has a list of bitnode indeces for each checknode
// bitnodes has a list of checknode indeces for each bitnode, including the parity bits

let makeTables (ft, rate) =
    let codingTableEntry = findCodingTableEntry (ft, rate)
    let nBitNodes = codingTableEntry.NLdpc
    let bitNodes = Array.create nBitNodes ([] : int list) 
    let nCheckNodes = codingTableEntry.NLdpc - codingTableEntry.KLdpc
    let checkNodes = Array.create nCheckNodes ([] : int list)
    makeDecodeTable (ft, rate)
    |> Array.iter (fun (b, c) -> 
        bitNodes.[b] <- c :: bitNodes.[b]
        checkNodes.[c] <- b :: checkNodes.[c])
    (bitNodes, checkNodes)

let (b, c) = makeTables (Long, Rate_1_2)

b |> Array.map (fun l -> l |> List.length) |> Array.groupBy id |> Array.map (fun (x, y) -> (x, y |> Array.length))

seq { for i in 1 .. 3 do yield i }
c.[1]
