module Hamstr.Ldpc.Decoder

open FSharp.Numerics
open DvbS2

type Contribution = {
    /// Who contributed this
    peerIndex : int
    llr : LLR
}

type BitNode = {
    /// My bitnode index
    index : int
    /// List of indices of the associated checkNodes
    checkNodeIds : int array
    /// Current sum of the modulo-2 additions
    value : LLR
    // List of contributions from the peers
    contributions : Contribution array
}

type CheckNode = {
    // My checknode index
    index : int
    // List of indices of the associated bitNodes
    bitNodes : int array
    // List of contributions from the peers
    contributions : Contribution array
}

let findSize codingTableEntry = 
    let nBitNodes = codingTableEntry.NLdpc
    let nDataBits = codingTableEntry.KLdpc
    let nParityBits = nBitNodes - nDataBits
    (nDataBits, nParityBits) 

/// Compile the connections between data nodes and check nodes
let makeDecodeTables frameType =
    let codingTableEntry = findCodingTableEntry frameType
    let (nDataBits, nParityBits) = findSize codingTableEntry
    
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
                    // Each element in the line is a parity accumulator that this bit goes into 
                    let parityIndex = 
                        (accAddress + (dataOffset % 360) * codingTableEntry.q) % nParityBits
                    (dataOffset, parityIndex))))
            |> List.concat
        |> Array.concat

    // Connect the parity bits to the corresponding checknodes
    let parityLinks =
        seq { for i in 1 .. nParityBits - 1 do yield (i + nDataBits, i) }  
        |> Array.ofSeq

    // Handle the final XOR in the encoding. Thank you to g4guo for the discussions!
    let xorLinks = 
        seq { for i in 1 .. nParityBits - 1 do yield (i + nDataBits - 1, i) }  
        |> Array.ofSeq

    // Create separate index lists for bitnodes and checknodes
    let nBitNodes = nDataBits + nParityBits
    let bitNodePeerLists = Array.create nBitNodes ([] : int list) 
    let checkNodePeerLists = Array.create nParityBits ([] : int list)

    let allLinks = [| accumulatorLinks; parityLinks; xorLinks |] 
    
    allLinks
    |> Array.concat 
    |> Array.iter (fun (b, c) -> 
        bitNodePeerLists.[b] <- c :: bitNodePeerLists.[b]
        checkNodePeerLists.[c] <- b :: checkNodePeerLists.[c])

    let bitNodes = 
        bitNodePeerLists 
        |> Array.mapi (fun i c -> 
            { index = i; checkNodeIds = List.toArray c; value = LLR.Undecided; contributions = Array.empty })

    let checkNodes = 
        checkNodePeerLists 
        |> Array.mapi (fun i b -> 
            { index = i; bitNodes = List.toArray b; contributions = Array.empty })
    
    (bitNodes, checkNodes)

/// Create the equivalent of Table I. in the Eroz paper. This is the number of bit nodes of various degrees, 
/// and hopefully useful for troubleshooting.
let createDegreeTable() =
    let degreeTable (b : BitNode[]) =
        b |> List.ofArray |> List.groupBy (fun b -> b.checkNodeIds.Length) |> List.map (fun (x, bs) -> (x, bs.Length))

    [ Rate_1_4; Rate_1_3; Rate_1_2; Rate_3_5; Rate_2_3; Rate_3_4; Rate_4_5; Rate_5_6; Rate_8_9; Rate_9_10 ]
    |> List.map (fun r -> { frameSize = Long; codeRate = r})
    |> List.map (fun x -> (x.codeRate, makeDecodeTables x |> fst))
    |> List.map (fun (rate, bitnodes) -> (rate, degreeTable bitnodes))

/// Initialize bitnodes from message bits
let initializeBitNodes (frame : FECFRAME) (bitnodes : BitNode []) = 
    bitnodes 
        |> Array.map (fun b -> 
            { b with value = frame.bits.[b.index] })

/// Update checknodes by adding contributions from all the connected bit nodes
let updateCheckNodes (bitnodes : BitNode[]) (checknodes : CheckNode[]) = 
    checknodes
    |> Array.map (fun c ->
        let contris = 
            c.bitNodes 
            |> Array.map (fun b -> 
                { peerIndex = b; llr = bitnodes.[b].value })
        { c with contributions = contris } )

/// Update bitnodes by assembling contributions from connected checknodes
let updateBitnodes (bitnodes : BitNode[]) (checknodes : CheckNode[]) =
    let summarizeChecknode c exclude =
        c.contributions
        |> Array.filter (fun cont -> cont.peerIndex <> exclude) 
        |> Array.map (fun cont -> cont.llr)
        |> Array.reduce (<+>) 

    bitnodes
    |> Array.iteri (fun i b ->
        let contris = 
            b.checkNodeIds
            |> Array.map (fun cnid -> 
                { peerIndex = checknodes.[cnid].index; llr = summarizeChecknode checknodes.[cnid] b.index } )
        bitnodes.[i] <- { b with contributions = contris } )

/// Compute hard decision
let computeHardDecision (bitnodes : BitNode []) =
    bitnodes 
    |> Array.map (fun b -> 
        let hardDecision = 
            b.contributions
            |> Array.map (fun cont -> cont.llr)
            |> Array.reduce (<+>)
        { b with value = hardDecision } )

/// Check parity equations: The sum of all the bitnodes adjacent to a 
/// checknode must be 0.
let checkParityEquations2 (bitnodes : BitNode []) (checknodes : CheckNode []) =
    let nonzeros = 
        checknodes
        |> Array.map (fun c -> 
            c.contributions
            |> Array.map (fun bi -> bitnodes.[bi.peerIndex].value.ToBool)
            |> Array.reduce (<>) ) 
        |> Array.filter not
        |> Array.length
    0 = nonzeros

let checkParityBool (frameType : FrameType) (bits : bool array) =
    let codingTableEntry = findCodingTableEntry frameType
    let (nDataBits, nParityBits) = findSize codingTableEntry
    ()

let checkParity (frame : FECFRAME) =
    frame.bits
    |> Array.map (fun b -> b.ToBool)
    |> checkParityBool frame.frameType

/// The main decoder
let decode (frame : FECFRAME) iterations =
    let (blankBitnodes, checkNodes) = makeDecodeTables frame.frameType
    let bitnodes = initializeBitNodes frame blankBitnodes
    let newChecknodes = updateCheckNodes bitnodes checkNodes 
    updateBitnodes bitnodes newChecknodes
    let hd = computeHardDecision bitnodes
    let result = checkParityEquations2 hd newChecknodes
    printfn "Parity check returned %A" result

    bitnodes
    