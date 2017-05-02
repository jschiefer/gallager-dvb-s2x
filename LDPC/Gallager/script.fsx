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

let newFrame = Array.create 32400 List.empty<int>
let cp = longCodingTable.[Rate_1_2]
let accumulatorTable = findLongParityTable Rate_1_2 |> fst

// Some of the magic we need for the 360 replication
let foo =
    accumulatorTable 
    |> List.map (fun l ->
        [ 0..359 ] |> List.collect (fun e -> l)
    )

// try this out with the first line
let line0 = accumulatorTable |> List.head
line0 |> List.iter (fun index -> newFrame.[index] <- 0::newFrame.[index])

// Go through the whole table and add the info bit to the relevant accumulators
accumulatorTable 
|> List.iter (fun line ->
    line 
    |> List.iter (fun index -> 
        newFrame.[index] <- 0::newFrame.[index]))

newFrame
