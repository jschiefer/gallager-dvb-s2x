module Hamstr.Ldpc.DvbS2

open System
open System.Numerics
open FSharp.Numerics
open Hamstr.Ldpc.DvbS2Tables

type LdpcCode = 
    | Rate_1_4 | Rate_1_3 | Rate_2_5 | Rate_1_2 | Rate_3_5 | Rate_2_3 
    | Rate_3_4 | Rate_4_5 | Rate_5_6 | Rate_8_9 | Rate_9_10

type FrameType =
    | Short | Medium | Long 
    static member BitLength = function
        | Short -> 16200
        | Medium -> 32400
        | Long -> 64800

type FECFRAME = {
    frameType : FrameType
    ldpcCode : LdpcCode
    data : array<LLR>
    parity : array<LLR>
}
let (|LongFrame|MediumFrame|ShortFrame|Invalid|) (frame : FECFRAME) = 
    match frame.data |> Seq.length with
    | 16200 -> ShortFrame
    | 32400 -> MediumFrame
    | 64800 -> LongFrame
    | _ -> Invalid

/// Coding parameters, as per secion 5.3, tables 5
type CodingTableEntry = {
    KBch : int      // BCH Uncoded block kBch
    KLdpc : int     // BCH coded block nBch, LDPC Uncoded Block kLdpc
    BchTError : int // BCH t-error correction
    NLdpc : int     // LDPC Coded Block nLdpc
    AccTable : int list list
    q : int         
}
let longCodingTable = 
    [
        // ( Rate_1_4, { KBch = 16008; KLdpc = 16200; BchTError = 12; NLdpc = 64800; q = 135; AccTable = ldpc_1_4N } )
        // ( Rate_1_3, { KBch = 21408; KLdpc = 21600; BchTError = 12; NLdpc = 64800; q = 120; AccTable = ldpc_1_3N } )
        // ( Rate_2_5, { KBch = 25728; KLdpc = 25920; BchTError = 12; NLdpc = 64800; q = 108; AccTable = ldpc_2_5N } )
        ( Rate_1_2, { KBch = 32208; KLdpc = 32400; BchTError = 12; NLdpc = 64800; q = 90; AccTable = ldpc_1_2N } )
        // ( Rate_3_5, { KBch = 38688; KLdpc = 38880; BchTError = 12; NLdpc = 64800; q = 72; AccTable = ldpc_3_5N } )
        // ( Rate_2_3, { KBch = 43040; KLdpc = 43200; BchTError = 10; NLdpc = 64800; q = 60; AccTable = ldpc_2_3N } )
        // ( Rate_3_4, { KBch = 48408; KLdpc = 48600; BchTError = 12; NLdpc = 64800; q = 45; AccTable = ldpc_3_4N } )
        // ( Rate_4_5, { KBch = 51648; KLdpc = 51840; BchTError = 12; NLdpc = 64800; q = 36; AccTable = ldpc_4_5N } )
        // ( Rate_5_6, { KBch = 53840; KLdpc = 54000; BchTError = 10; NLdpc = 64800; q = 30; AccTable = ldpc_5_6N } )
        // ( Rate_8_9, { KBch = 57472; KLdpc = 57600; BchTError = 8; NLdpc = 64800; q = 20 ; AccTable = ldpc_8_9N } )
        // ( Rate_9_10, { KBch = 58192; KLdpc = 58320; BchTError = 8; NLdpc = 64800; q = 18; AccTable = ldpc_9_10N } )
    ] |> Map.ofList

(*
let shortCodingTable = 
    [
        ( Rate_1_4, { KBch = 3072; KLdpc = 3240; BchTError = 12; NLdpc = 16200; q = 36 } )
        ( Rate_1_3, { KBch = 5232; KLdpc = 5400; BchTError = 12; NLdpc = 16200; q = 30 } )
        ( Rate_2_5, { KBch = 6312; KLdpc = 6480; BchTError = 12; NLdpc = 16200; q = 27 } )
        ( Rate_1_2, { KBch = 7032; KLdpc = 7200; BchTError = 12; NLdpc = 16200; q = 25 } )
        ( Rate_3_5, { KBch = 9552; KLdpc = 9720; BchTError = 12; NLdpc = 16200; q = 18 } )
        ( Rate_2_3, { KBch = 10632; KLdpc = 10800; BchTError = 12; NLdpc = 16200; q = 15 } )
        ( Rate_3_4, { KBch = 11712; KLdpc = 11880; BchTError = 12; NLdpc = 16200; q = 12 } )
        ( Rate_4_5, { KBch = 12432; KLdpc = 12600; BchTError = 12; NLdpc = 16200; q = 10 } )
        ( Rate_5_6, { KBch = 13152; KLdpc = 13320; BchTError = 12; NLdpc = 16200; q = 8 } )
        ( Rate_8_9, { KBch = 14232; KLdpc = 14400; BchTError = 12; NLdpc = 16200; q = 5 } )
    ] |> Map.ofList
*)

/// Modulation types allowed for DVB-S2. DVB-S2X has a bunch more (not yet implemented).
type Modulation = 
    /// Defined in the DVB-S2 Specification, section 5.4.1
    | M_QPSK                
    /// Defined in the DVB-S2 Specification, section 5.4.2
    | M_8PSK                
    /// Defined in the DVB-S2 Specification, section 5.4.3
    | M_16APSK_4_12         
    /// Defined in the DVB-S2 Specification, section 5.4.4
    | M_32APSK_4_12_16      

/// Constellation mapping for each type of modulation.
/// They are in order, i.e. the index into the array is the value of the symbol.
let getConstellation = function
     | M_QPSK -> 
        let s = 1.0 / sqrt(2.0) 
        [| Complex(s, s); Complex(s, -s); Complex(-s, s); Complex(-s, -s) |]

/// A MODCOD is the combination of the modulation type and the coding. 
/// There are still some bits missing here.
type Modcod = {
    Modulation : Modulation
    LdpcRate : LdpcCode
}

/// The PLS code defines the MODCOD used in a frame. 
let ModCodLookup = 
    [
        ( 1uy, { Modulation = M_QPSK; LdpcRate = Rate_1_4 } );
        ( 2uy, { Modulation = M_QPSK; LdpcRate = Rate_1_3 } );
        ( 3uy, { Modulation = M_QPSK; LdpcRate = Rate_2_5 } );
        ( 4uy, { Modulation = M_QPSK; LdpcRate = Rate_1_2 } );
        ( 5uy, { Modulation = M_QPSK; LdpcRate = Rate_3_5 } );
        ( 6uy, { Modulation = M_QPSK; LdpcRate = Rate_2_3 } );
        ( 7uy, { Modulation = M_QPSK; LdpcRate = Rate_3_4 } );
        ( 8uy, { Modulation = M_QPSK; LdpcRate = Rate_4_5 } );
        ( 9uy, { Modulation = M_QPSK; LdpcRate = Rate_5_6 } );
        ( 10uy, { Modulation = M_QPSK; LdpcRate = Rate_8_9 } );
        ( 11uy, { Modulation = M_QPSK; LdpcRate = Rate_9_10 } );
        ( 12uy, { Modulation = M_8PSK; LdpcRate = Rate_3_5 } );
        ( 13uy, { Modulation = M_8PSK; LdpcRate = Rate_2_3 } );
        ( 14uy, { Modulation = M_8PSK; LdpcRate = Rate_3_4 } );
        ( 15uy, { Modulation = M_8PSK; LdpcRate = Rate_5_6 } );
        ( 16uy, { Modulation = M_8PSK; LdpcRate = Rate_8_9 } );
        ( 17uy, { Modulation = M_8PSK; LdpcRate = Rate_9_10 } );
        ( 18uy, { Modulation = M_16APSK_4_12; LdpcRate = Rate_2_3 } );
        ( 19uy, { Modulation = M_16APSK_4_12; LdpcRate = Rate_3_4 } );
        ( 20uy, { Modulation = M_16APSK_4_12; LdpcRate = Rate_4_5 } );
        ( 21uy, { Modulation = M_16APSK_4_12; LdpcRate = Rate_5_6 } );
        ( 22uy, { Modulation = M_16APSK_4_12; LdpcRate = Rate_8_9 } );
        ( 23uy, { Modulation = M_16APSK_4_12; LdpcRate = Rate_9_10 } );
        ( 24uy, { Modulation = M_32APSK_4_12_16; LdpcRate = Rate_3_4 } );
        ( 25uy, { Modulation = M_32APSK_4_12_16; LdpcRate = Rate_4_5 } );
        ( 26uy, { Modulation = M_32APSK_4_12_16; LdpcRate = Rate_5_6 } );
        ( 27uy, { Modulation = M_32APSK_4_12_16; LdpcRate = Rate_8_9 } );
        ( 28uy, { Modulation = M_32APSK_4_12_16; LdpcRate = Rate_9_10 } );
    ] |> Map.ofList

let findCodingTableEntry frameType rate =
    match frameType with
    | Long -> longCodingTable.[rate]

let findLongCodingTableEntry = findCodingTableEntry Long