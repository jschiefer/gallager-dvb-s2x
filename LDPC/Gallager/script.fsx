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
let message = readTestFile IqFile iqDataFileName Long ModCodLookup.[04uy]

// Compile the connections between data nodes and check nodes
codingTableEntry.AccTable

// Every soft bit in the message is a bit node.

// How are the check nodes determined? Somehow from the accumulator table


