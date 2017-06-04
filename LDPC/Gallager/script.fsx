#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open System
open System.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.DvbS2Tables
open Hamstr.Demod
open Hamstr.Ldpc.Decoder

let a = makeParityTable()
printfn "%A" a.[0]
