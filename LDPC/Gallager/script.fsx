#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open System
open System.Numerics
open Hamstr.Ldpc.Decoder

/// Create a list of check nodes for bit node at index b
let checkNodes b = ()

/// Create a list of bit nodes for the check node at index c
let bitNodes c = ()

makeTable
|> Array.map (fun xs -> printfn "%A" xs.Length)
