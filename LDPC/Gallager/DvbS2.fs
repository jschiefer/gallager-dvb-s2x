module Hamstr.Ldpc.DvbS2

open System
open System.Numerics

type LdpcCode = 
    | Rate_1_4
    | Rate_1_3
    | Rate_2_5
    | Rate_1_2
    | Rate_3_5
    | Rate_2_3
    | Rate_3_4
    | Rate_4_5
    | Rate_5_6
    | Rate_8_9
    | Rate_9_10

/// Coding parameters, as per secion 5.3, tables 5
type CodingTableEntry = {
    KBch : int      // BCH Uncoded block kBch
    KLdpc : int     // BCH coded block nBch, LDPC Uncoded Block kLdpc
    BchTError : int // BCH t-error correction
    NLdpc : int     // LDPC Coded Block nLdpc
}

let longCodingTable = 
    [
        ( Rate_1_4, { KBch = 16008; KLdpc = 16200; BchTError = 12; NLdpc = 64800 } )
        ( Rate_1_3, { KBch = 21408; KLdpc = 21600; BchTError = 12; NLdpc = 64800 } )
        ( Rate_2_5, { KBch = 25728; KLdpc = 25920; BchTError = 12; NLdpc = 64800 } )
        ( Rate_1_2, { KBch = 32208; KLdpc = 32400; BchTError = 12; NLdpc = 64800 } )
        ( Rate_3_5, { KBch = 38688; KLdpc = 38880; BchTError = 12; NLdpc = 64800 } )
        ( Rate_2_3, { KBch = 43040; KLdpc = 43200; BchTError = 10; NLdpc = 64800 } )
        ( Rate_3_4, { KBch = 48408; KLdpc = 48600; BchTError = 12; NLdpc = 64800 } )
        ( Rate_4_5, { KBch = 51648; KLdpc = 51840; BchTError = 12; NLdpc = 64800 } )
        ( Rate_5_6, { KBch = 53840; KLdpc = 54000; BchTError = 10; NLdpc = 64800 } )
        ( Rate_8_9, { KBch = 57472; KLdpc = 57600; BchTError = 8; NLdpc = 64800 } )
        ( Rate_9_10, { KBch = 58192; KLdpc = 58320; BchTError = 8; NLdpc = 64800 } )
    ] |> Map.ofList

let shortCodingTable = 
    [
        ( Rate_1_4, { KBch = 3072; KLdpc = 3240; BchTError = 12; NLdpc = 16200 } )
        ( Rate_1_3, { KBch = 5232; KLdpc = 5400; BchTError = 12; NLdpc = 16200 } )
        ( Rate_2_5, { KBch = 6312; KLdpc = 6480; BchTError = 12; NLdpc = 16200 } )
        ( Rate_1_2, { KBch = 7032; KLdpc = 7200; BchTError = 12; NLdpc = 16200 } )
        ( Rate_3_5, { KBch = 9552; KLdpc = 9720; BchTError = 12; NLdpc = 16200 } )
        ( Rate_2_3, { KBch = 10632; KLdpc = 10800; BchTError = 12; NLdpc = 16200 } )
        ( Rate_3_4, { KBch = 11712; KLdpc = 11880; BchTError = 12; NLdpc = 16200 } )
        ( Rate_4_5, { KBch = 12432; KLdpc = 12600; BchTError = 12; NLdpc = 16200 } )
        ( Rate_5_6, { KBch = 13152; KLdpc = 13320; BchTError = 12; NLdpc = 16200 } )
        ( Rate_8_9, { KBch = 14232; KLdpc = 14400; BchTError = 12; NLdpc = 16200 } )
    ] |> Map.ofList

/// Frames can only have a few distinct sizes
type FECFRAME = 
    | Short of seq<(byte * float)> 
    | Long of seq<(byte * float)> 

/// This is how long frames are (in bits)
type BitsPerFrame = 
    | Short = 16200
    | Long =  64800

let (|LongFrame|ShortFrame|Invalid|) (len : int) = 
    match enum len with
    | BitsPerFrame.Short -> ShortFrame
    | BitsPerFrame.Long -> LongFrame
    | _ -> Invalid

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

/// Parity bit accumulator table, 1/2 rate, long frames
let ldpc_1_2_l = 
    [
        [ 54; 9318; 14392; 27561; 26909; 10219; 2534; 8597]; 
        [ 55; 7263; 4635; 2530; 28130; 3033; 23830; 3651 ]; 
        [ 56; 24731; 23583; 26036; 17299; 5750; 792; 9169 ]; 
        [ 57; 5811; 26154; 18653; 11551; 15447; 13685; 16264 ]; 
        [ 58; 12610; 11347; 28768; 2792; 3174; 29371; 12997 ]; 
        [ 59; 16789; 16018; 21449; 6165; 21202; 15850; 3186 ]; 
        [ 60; 31016; 21449; 17618; 6213; 12166; 8334; 18212 ]; 
        [ 61; 22836; 14213; 11327; 5896; 718; 11727; 9308 ]; 
        [ 62; 2091; 24941; 29966; 23634; 9013; 15587; 5444 ]; 
        [ 63; 22207; 3983; 16904; 28534; 21415; 27524; 25912 ]; 
        [ 64; 25687; 4501; 22193; 14665; 14798; 16158; 5491 ]; 
        [ 65; 4520; 17094; 23397; 4264; 22370; 16941; 21526 ]; 
        [ 66; 10490; 6182; 32370; 9597; 30841; 25954; 2762 ]; 
        [ 67; 22120; 22865; 29870; 15147; 13668; 14955; 19235 ]; 
        [ 68; 6689; 18408; 18346; 9918; 25746; 5443; 20645 ]; 
        [ 69; 29982; 12529; 13858; 4746; 30370; 10023; 24828 ]; 
        [ 70; 1262; 28032; 29888; 13063; 24033; 21951; 7863 ]; 
        [ 71; 6594; 29642; 31451; 14831; 9509; 9335; 31552 ]; 
        [ 72; 1358; 6454; 16633; 20354; 24598; 624; 5265 ]; 
        [ 73; 19529; 295; 18011; 3080; 13364; 8032; 15323 ]; 
        [ 74; 11981; 1510; 7960; 21462; 9129; 11370; 25741 ]; 
        [ 75; 9276; 29656; 4543; 30699; 20646; 21921; 28050 ]; 
        [ 76; 15975; 25634; 5520; 31119; 13715; 21949; 19605 ]; 
        [ 77; 18688; 4608; 31755; 30165; 13103; 10706; 29224 ]; 
        [ 78; 21514; 23117; 12245; 26035; 31656; 25631; 30699 ]; 
        [ 79; 9674; 24966; 31285; 29908; 17042; 24588; 31857 ]; 
        [ 80; 21856; 27777; 29919; 27000; 14897; 11409; 7122 ]; 
        [ 81; 29773; 23310; 263; 4877; 28622; 20545; 22092 ]; 
        [ 82; 15605; 5651; 21864; 3967; 14419; 22757; 15896 ]; 
        [ 83; 30145; 1759; 10139; 29223; 26086; 10556; 5098 ]; 
        [ 84; 18815; 16575; 2936; 24457; 26738; 6030; 505 ]; 
        [ 85; 30326; 22298; 27562; 20131; 26390; 6247; 24791 ]; 
        [ 86; 928; 29246; 21246; 12400; 15311; 32309; 18608 ]; 
        [ 87; 20314; 6025; 26689; 16302; 2296; 3244; 19613 ]; 
        [ 88; 6237; 11943; 22851; 15642; 23857; 15112; 20947 ]; 
        [ 89; 26403; 25168; 19038; 18384; 8882; 12719; 7093 ]; 
        [ 0; 14567; 24965 ];
        [ 1; 3908; 100 ];
        [ 2; 10279; 240 ];
        [ 3; 24102; 764 ];
        [ 4; 12383; 4173 ];
        [ 5; 13861; 15918 ];
        [ 6; 21327; 1046 ];
        [ 7; 5288; 14579 ];
        [ 8; 28158; 8069 ];
        [ 9; 16583; 11098 ];
        [ 10; 16681; 28363 ];
        [ 11; 13980; 24725 ];
        [ 12; 32169; 17989 ];
        [ 13; 10907; 2767 ];
        [ 14; 21557; 3818 ];
        [ 15; 26676; 12422 ];
        [ 16; 7676; 8754 ];
        [ 17; 14905; 20232 ];
        [ 18; 15719; 24646 ];
        [ 19; 31942; 8589 ];
        [ 20; 19978; 27197 ];
        [ 21; 27060; 15071 ];
        [ 22; 6071; 26649 ];
        [ 23; 10393; 11176 ];
        [ 24; 9597; 13370 ];
        [ 25; 7081; 17677 ];
        [ 26; 1433; 19513 ];
        [ 27; 26925; 9014 ];
        [ 28; 19202; 8900 ];
        [ 29; 18152; 30647 ];
        [ 30; 20803; 1737 ];
        [ 31; 11804; 25221 ];
        [ 32; 31683; 17783 ];
        [ 33; 29694; 9345 ];
        [ 34; 12280; 26611 ];
        [ 35; 6526; 26122 ];
        [ 36; 26165; 11241 ];
        [ 37; 7666; 26962 ];
        [ 38; 16290; 8480 ];
        [ 39; 11774; 10120 ];
        [ 40; 30051; 30426 ];
        [ 41; 1335; 15424 ];
        [ 42; 6865; 17742 ];
        [ 43; 31779; 12489 ];
        [ 44; 32120; 21001 ];
        [ 45; 14508; 6996 ];
        [ 46; 979; 25024 ];
        [ 47; 4554; 21896 ];
        [ 48; 7989; 21777 ];
        [ 49; 4972; 20661 ];
        [ 50; 6612; 2730 ];
        [ 51; 12742; 4418 ];
        [ 52; 29194; 595 ];
        [ 53; 19267; 20113 ]
    ]

let findLongParityTable = function
    | Rate_1_2 -> (ldpc_1_2_l, 90)

let findShortParityTable = function
    | Rate_1_2 -> (ldpc_1_2_l, 90)

let findParityTable rate = function
    | Long(_) -> findLongParityTable rate
    | Short(_) -> findShortParityTable rate
