#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Main

let message = readTestFile IqFile iqDataFileName Long ModCodLookup.[04uy]
message.bits |> Array.length

let (b, c) = makeDecodeTables (Long, Rate_1_2)

c 
|> Array.map (fun l -> l |> List.length) 
|> Array.groupBy id 
|> Array.map (fun (x, y) -> (x, y |> Array.length))



