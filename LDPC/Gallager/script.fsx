#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open System
open System.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod

/// Create a list of check nodes for bit node at index b
let checkNodes b = ()

/// Create a list of bit nodes for the check node at index c
let bitNodes c = ()

let newFrame = Array.create 32400 List.empty<int>

// Go through the whole table and add the info bit to the relevant accumulators
let bar = 
    let (accumulatorTable, codeParam) = findLongLdpcParameters Rate_1_2
    accumulatorTable
    |> List.iter (fun line ->
        line 
        |> List.iter (fun x -> 
            [0..359] 
            |> List.iter (fun m ->
                let index = (x + (m % 360) * codeParam.q) % (codeParam.NLdpc - codeParam.KLdpc)
                newFrame.[index] <- m::newFrame.[index])))

