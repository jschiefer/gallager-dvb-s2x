module Hamstr.Ldpc.Decoder

open FSharp.Numerics
open Hamstr.Ldpc.DvbS2

type Contribution = {
    /// Who contributed this
    peerIndex : int
    llr : LLR
}

/// LDPC-decode the frame (which is an array of tuples of bit and LLR)
type BitNode = {
    /// My bitnode index
    index : int
    /// List of indices of the associated checkNodes
    checkNodes : int list
    /// Current sum of the modulo-2 additions
    value : LLR
    // List of contributions from the peers
    contributions : Contribution list
}

type CheckNode = {
    /// My checknode index
    index : int
    /// List of indices of the associated bitNodes
    bitNodes : int list
    // List of contributions from the peers
    contributions : Contribution list
}

// Compile the connections between data nodes and check nodes
let makeDecodeTables typeAndCode =
    let codingTableEntry = findCodingTableEntry typeAndCode
    let nDataBits = codingTableEntry.KLdpc
    let nBitNodes = codingTableEntry.NLdpc
    let nParityBits = nBitNodes - nDataBits
    
    let accumulatorLinks = 
        codingTableEntry.AccTable
        // Iterate over the lines of the accumulator table
        |> Array.mapi (fun blockNo line ->
            // Apply each line in the accumulator table to 360 bits
            [0..359] 
            |> List.map (fun blockOffset ->
                let dataOffset = blockNo * 360 + blockOffset
                line
                |> Array.map (fun accAddress -> 
                    // Each element in the line, is a parity accumulator that this bit goes into 
                    let parityIndex = 
                        (accAddress + (dataOffset % 360) * codingTableEntry.q) % nParityBits
                    (dataOffset, parityIndex))))
            |> List.concat
        |> Array.concat

    // Handle the final XOR in the encoding. Thank you to g4guo for the insight!
    let xorLinks = 
        seq { for i in 1 .. nParityBits - 2 do yield (i + nDataBits, i - 1) }  
        |> Array.ofSeq

    // Create separate index lists for bitnodes and checknodes
    let bitNodePeerLists = Array.create nBitNodes ([] : int list) 
    let checkNodePeerLists = Array.create nParityBits ([] : int list)

    [| accumulatorLinks; xorLinks |] 
    |> Array.concat 
    |> Array.iter (fun (b, c) -> 
        bitNodePeerLists.[b] <- c :: bitNodePeerLists.[b]
        checkNodePeerLists.[c] <- b :: checkNodePeerLists.[c])

    let bitNodes = bitNodePeerLists |> Array.mapi (fun i c -> 
        { index = i; checkNodes = c; value = LLR.Undecided; contributions = [] })
    let checkNodes = checkNodePeerLists |> Array.mapi (fun i b -> 
        { index = i; bitNodes = b; contributions = [] })
    
    (bitNodes, checkNodes)

let decode typeAndCode frame =

    // Initialize bitnodes from message bits
    let initializeBitNodes (frame : FECFRAME) (bitnodes : BitNode []) = 
        bitnodes 
            |> Array.map (fun b -> 
                { b with value = frame.bits.[b.index] })

    // Update checknodes by adding contributions from all the connected bit nodes
    let updateCheckNodes (checknodes : CheckNode[]) (bitnodes : BitNode[]) = 
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
    let checkParityEquations (checknodes : CheckNode []) (bitnodes : BitNode []) =
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

    ()
