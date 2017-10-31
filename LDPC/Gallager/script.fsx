// #r "bin/Debug/netcoreapp1.1/Gallager.dll"

#r "bin/Debug/Gallager2.exe"

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Main

let iqDataFileName = "c:/Users/jas/Develop/Github/p4g-toys/LDPC/Data/qpsk_testdata.out"
let message = readTestFile IqFile iqDataFileName Long ModCodLookup.[04uy]
let nBitnodes = 64800
let nChecknodes = 32400
let (bi, ci) = makeDecodeTables (Long, Rate_1_2)

// Initialize bitnodes from message bits
let initializeBitNodes bitnodes = 
    bitnodes 
        |> Array.map (fun b -> 
            { b with value = message.bits.[b.index] })

// Update checknodes by adding contributions from bit nodes
let updateCheckNodes (checknodes : CheckNode[]) (bitnodes : BitNode[]) = 
    checknodes
    |> Array.map (fun c ->
        let contris = 
            c.bitNodes 
            |> List.map (fun b -> 
                { peerIndex = b; llr = bitnodes.[b].value })
        { c with contributions = contris } )

// Update bitnodes by assembling contributions from checknodes
let updateBitnodes (bitnodes : BitNode[]) (checknodes : CheckNode[]) =
    let summarizeChecknode c exclude =
        c.contributions
        |> List.filter (fun cont -> cont.peerIndex <> exclude) 
        |> List.map (fun cont -> cont.llr)
        |> List.fold (+) LLR.Undecided

    bitnodes
    |> Array.map (fun b ->
        let contris = 
            b.checkNodes
            |> List.map (fun i -> checknodes.[i]) 
            |> List.map (fun c -> { peerIndex = c.index; llr = summarizeChecknode c b.index } )
        { b with contributions = contris } )

// Compute hard decision
let computeHardDecision (b : BitNode) =
    b.contributions
    |> List.map (fun cont -> cont.llr)
    |> List.fold (+) LLR.Undecided

// Check parity equations

