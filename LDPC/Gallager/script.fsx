#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open System
open System.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod

/// Create a list of check nodes for bit node at index b
let checkNodes b = ()

/// Create a list of bit nodes for the check node at index c
let bitNodes c = ()

let (pt, q) = findLongParityTable Rate_1_2

