module Hamstr.Ldpc.DvbS2

open System.Numerics
open FSharp.Numerics
open DvbS2Tables

type FrameSize =
    | Short | Medium | Long 

type CodeRate = 
    | Rate_1_4 | Rate_1_3 | Rate_2_5 | Rate_1_2 | Rate_3_5 | Rate_2_3 
    | Rate_3_4 | Rate_4_5 | Rate_5_6 | Rate_8_9 | Rate_9_10

type FrameType = {
    frameSize : FrameSize
    codeRate : CodeRate
}

type FECFRAME = {
    frameType : FrameType
    bits : LLR[]
}

type Standard =
    | DvbS2
    | DvbS2x

/// Coding parameters, as per secion 5.3, tables 5
type CodingTableEntry = {
    /// Which standard is this
    Std : Standard
    /// BCH Uncoded block kBch
    KBch : int      
    /// BCH coded block nBch, LDPC Uncoded Block kLdpc
    KLdpc : int     
    /// BCH t-error correction
    BchTError : int 
    /// LDPC Coded Block nLdpc
    NLdpc : int     
    /// Accumulator table
    AccTable : int [] []
    q : int         
}

/// Lookup table to find the right numbers of bits for each frame type
let codingTable = 
    [
        ( (Long, Rate_1_4), { Std = DvbS2; KBch = 16008; KLdpc = 16200; BchTError = 12; NLdpc = 64800; q = 135; AccTable = ldpc_1_4N } )
        ( (Long, Rate_1_3), { Std = DvbS2; KBch = 21408; KLdpc = 21600; BchTError = 12; NLdpc = 64800; q = 120; AccTable = ldpc_1_3N } )
        ( (Long, Rate_2_5), { Std = DvbS2; KBch = 25728; KLdpc = 25920; BchTError = 12; NLdpc = 64800; q = 108; AccTable = ldpc_2_5N } )
        ( (Long, Rate_1_2), { Std = DvbS2; KBch = 32208; KLdpc = 32400; BchTError = 12; NLdpc = 64800; q = 90; AccTable = ldpc_1_2N } )
        ( (Long, Rate_3_5), { Std = DvbS2; KBch = 38688; KLdpc = 38880; BchTError = 12; NLdpc = 64800; q = 72; AccTable = ldpc_3_5N } )
        ( (Long, Rate_2_3), { Std = DvbS2; KBch = 43040; KLdpc = 43200; BchTError = 10; NLdpc = 64800; q = 60; AccTable = ldpc_2_3N } )
        ( (Long, Rate_3_4), { Std = DvbS2; KBch = 48408; KLdpc = 48600; BchTError = 12; NLdpc = 64800; q = 45; AccTable = ldpc_3_4N } )
        ( (Long, Rate_4_5), { Std = DvbS2; KBch = 51648; KLdpc = 51840; BchTError = 12; NLdpc = 64800; q = 36; AccTable = ldpc_4_5N } )
        ( (Long, Rate_5_6), { Std = DvbS2; KBch = 53840; KLdpc = 54000; BchTError = 10; NLdpc = 64800; q = 30; AccTable = ldpc_5_6N } )
        ( (Long, Rate_8_9), { Std = DvbS2; KBch = 57472; KLdpc = 57600; BchTError = 8; NLdpc = 64800; q = 20 ; AccTable = ldpc_8_9N } )
        ( (Long, Rate_9_10), { Std = DvbS2; KBch = 58192; KLdpc = 58320; BchTError = 8; NLdpc = 64800; q = 18; AccTable = ldpc_9_10N } )
        ( (Short, Rate_1_4), { Std = DvbS2; KBch = 3072; KLdpc = 3240; BchTError = 12; NLdpc = 16200; q = 36; AccTable = ldpc_1_4S } )
        ( (Short, Rate_1_3), { Std = DvbS2; KBch = 5232; KLdpc = 5400; BchTError = 12; NLdpc = 16200; q = 30; AccTable = ldpc_1_3S } )
        ( (Short, Rate_2_5), { Std = DvbS2; KBch = 6312; KLdpc = 6480; BchTError = 12; NLdpc = 16200; q = 27; AccTable = ldpc_2_5S } )
        ( (Short, Rate_1_2), { Std = DvbS2; KBch = 7032; KLdpc = 7200; BchTError = 12; NLdpc = 16200; q = 25; AccTable = ldpc_1_2S } )
        ( (Short, Rate_3_5), { Std = DvbS2; KBch = 9552; KLdpc = 9720; BchTError = 12; NLdpc = 16200; q = 18; AccTable = ldpc_3_5S } )
        ( (Short, Rate_2_3), { Std = DvbS2; KBch = 10632; KLdpc = 10800; BchTError = 12; NLdpc = 16200; q = 15; AccTable = ldpc_2_3S } )
        ( (Short, Rate_3_4), { Std = DvbS2; KBch = 11712; KLdpc = 11880; BchTError = 12; NLdpc = 16200; q = 12; AccTable = ldpc_3_4S } )
        ( (Short, Rate_4_5), { Std = DvbS2; KBch = 12432; KLdpc = 12600; BchTError = 12; NLdpc = 16200; q = 10; AccTable = ldpc_4_5S } )
        ( (Short, Rate_5_6), { Std = DvbS2; KBch = 13152; KLdpc = 13320; BchTError = 12; NLdpc = 16200; q = 8; AccTable = ldpc_5_6S } )
        ( (Short, Rate_8_9), { Std = DvbS2; KBch = 14232; KLdpc = 14400; BchTError = 12; NLdpc = 16200; q = 5; AccTable = ldpc_8_9S } )
    ] |> Map.ofList

let findCodingTableEntry f = 
    codingTable.[(f.frameSize, f.codeRate)]

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

/// A MODCOD is the combination of the MODulation type and the CODe rate. 
type ModCod = {
    Modulation : Modulation
    CodeRate : CodeRate
}

/// The PLS code is a tag for the MODCOD used in a frame. 
let ModCodLookup = 
    [
        ( 1uy, { Modulation = M_QPSK; CodeRate = Rate_1_4 } );
        ( 2uy, { Modulation = M_QPSK; CodeRate = Rate_1_3 } );
        ( 3uy, { Modulation = M_QPSK; CodeRate = Rate_2_5 } );
        ( 4uy, { Modulation = M_QPSK; CodeRate = Rate_1_2 } );
        ( 5uy, { Modulation = M_QPSK; CodeRate = Rate_3_5 } );
        ( 6uy, { Modulation = M_QPSK; CodeRate = Rate_2_3 } );
        ( 7uy, { Modulation = M_QPSK; CodeRate = Rate_3_4 } );
        ( 8uy, { Modulation = M_QPSK; CodeRate = Rate_4_5 } );
        ( 9uy, { Modulation = M_QPSK; CodeRate = Rate_5_6 } );
        ( 10uy, { Modulation = M_QPSK; CodeRate = Rate_8_9 } );
        ( 11uy, { Modulation = M_QPSK; CodeRate = Rate_9_10 } );
        ( 12uy, { Modulation = M_8PSK; CodeRate = Rate_3_5 } );
        ( 13uy, { Modulation = M_8PSK; CodeRate = Rate_2_3 } );
        ( 14uy, { Modulation = M_8PSK; CodeRate = Rate_3_4 } );
        ( 15uy, { Modulation = M_8PSK; CodeRate = Rate_5_6 } );
        ( 16uy, { Modulation = M_8PSK; CodeRate = Rate_8_9 } );
        ( 17uy, { Modulation = M_8PSK; CodeRate = Rate_9_10 } );
        ( 18uy, { Modulation = M_16APSK_4_12; CodeRate = Rate_2_3 } );
        ( 19uy, { Modulation = M_16APSK_4_12; CodeRate = Rate_3_4 } );
        ( 20uy, { Modulation = M_16APSK_4_12; CodeRate = Rate_4_5 } );
        ( 21uy, { Modulation = M_16APSK_4_12; CodeRate = Rate_5_6 } );
        ( 22uy, { Modulation = M_16APSK_4_12; CodeRate = Rate_8_9 } );
        ( 23uy, { Modulation = M_16APSK_4_12; CodeRate = Rate_9_10 } );
        ( 24uy, { Modulation = M_32APSK_4_12_16; CodeRate = Rate_3_4 } );
        ( 25uy, { Modulation = M_32APSK_4_12_16; CodeRate = Rate_4_5 } );
        ( 26uy, { Modulation = M_32APSK_4_12_16; CodeRate = Rate_5_6 } );
        ( 27uy, { Modulation = M_32APSK_4_12_16; CodeRate = Rate_8_9 } );
        ( 28uy, { Modulation = M_32APSK_4_12_16; CodeRate = Rate_9_10 } );
    ] |> Map.ofList
