#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Main


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
        |> List.reduce (+) 

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
            |> List.reduce (+)
        { b with value = hardDecision } )

// Check parity equations: The sum of all the bitnodes adjacent to a 
// checknode must be 0.
let checkParityEquations (bitnodes : BitNode []) (checknodes : CheckNode []) =
    let nonzeros = 
        checknodes
        |> Array.map (fun c -> 
            c.contributions
            |> List.map (fun bi -> bitnodes.[bi.peerIndex].value.ToBool)
            |> List.reduce (<>) ) 
        |> Array.filter not
        |> Array.length
    nonzeros = 0

let printBitNodes (bitnodes : BitNode[]) =
    bitnodes
    |> Array.take 50
    |> Array.iteri (fun i b -> printfn "%A %A " i b.value)


let iqDataFileName = "../Data/qpsk_testdata.out"
let frame = readTestFile IqFile iqDataFileName Long ModCodLookup.[04uy]
let typeAndCode = (Long, Rate_1_2)
let (blankBitnodes, checkNodes) = makeDecodeTables typeAndCode
let bitnodes = initializeBitNodes frame blankBitnodes
let newChecknodes = updateCheckNodes bitnodes checkNodes 
let newBitnodes = updateBitnodes bitnodes newChecknodes
let hd = computeHardDecision newBitnodes

printBitNodes  newBitnodes

blankBitnodes.[64799]

let b = newBitnodes.[0]
b.contributions
|> List.map (fun cont -> cont.llr)
|> List.reduce (+) 

newBitnodes |> Array.filter (fun b -> b.contributions.IsEmpty) |> Array.length

// Bitnodes 32400 and 64799 do not have any contributions (they do have values, though)
// The offsets between the index of the Bitnodes and the checkNodes seem off:
// bit node 32400 -> no check node
// bit node 32401 -> check node 0
// bit node 32402 -> check node 1
// ....
// bit node 64798 -> check node 32397
// bit node 64799 -> no check node
// This looks like there are two off-by-one errors.
