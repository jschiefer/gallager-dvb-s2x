#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open System
open System.Numerics
open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.DvbS2Tables
open Hamstr.Demod
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Main

let codingTableEntry = findLongCodingTableEntry Rate_1_2

// Compile the connections between data nodes and check nodes
// The data nodes are all the bits of the message
// We need a message
let message = readTestFile IqFile iqDataFileName Long ModCodLookup.[04uy]

// How are the check nodes determined? Somehow from the accumulator table

