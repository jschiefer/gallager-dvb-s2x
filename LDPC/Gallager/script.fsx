#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Main

let iqDataFileName = "../Data/qpsk_testdata.out"
let frame = readTestFile IqFile iqDataFileName Long ModCodLookup.[04uy]
let typeAndCode = (Long, Rate_1_2)

// Initialize bitnodes from message bits
let initializeBitNodes (frame : FECFRAME) (bitnodes : BitNode []) = 
    bitnodes 
        |> Array.map (fun b -> 
            { b with value = frame.bits.[b.index] })

// Update checknodes by adding contributions from all the connected bit nodes
let updateCheckNodes (bitnodes : BitNode[]) (checknodes : CheckNode[]) = 
    checknodes
    |> Array.map (fun c ->
        let contris = 
            c.bitNodes 
            |> List.map (fun b -> 
                { peerIndex = b; llr = bitnodes.[b].value })
        { c with contributions = contris } )

// Update bitnodes by assembling contributions from connected checknodes
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
let computeHardDecision (bitnodes : BitNode []) =
    bitnodes 
    |> Array.map (fun b -> 
        let hardDecision = 
            b.contributions
            |> List.map (fun cont -> cont.llr)
            |> List.fold (+) LLR.Undecided
        { b with value = hardDecision } )

// Check parity equations: The sum of all the bitnodes adjacent to a 
// checknode must be 0.
let checkParityEquations (bitnodes : BitNode []) (checknodes : CheckNode []) =
    let nonzeros = 
        checknodes
        |> Array.map (fun c -> 
            c.contributions
            |> List.map (fun bi -> bitnodes.[bi.peerIndex].value.ToBool)
            |> List.fold (<>) false) 
        |> Array.filter not
        |> Array.length
    nonzeros = 0

let (blankBitnodes, checkNodes) = makeDecodeTables typeAndCode
let bitnodes = initializeBitNodes frame blankBitnodes
let newChecknodes = updateCheckNodes bitnodes checkNodes 
let newBitnodes = updateBitnodes bitnodes newChecknodes

newBitnodes.[0]
