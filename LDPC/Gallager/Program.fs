module Hamstr.Gallager

open System

type Standard = 
    | DvbS2         // Defined in ETSI EN 302 307-1
    | DvbS2x        // Defined in ETSI EN 302 307-2

type LdpcRate = int * int 

type FECFRAME = 
    | Short of byte[]
    | Medium of byte[]
    | Long of byte[]

type Modulation = 
    | M_PI2BPSK             // S2x 5.4.0
    | M_QPSK                // S2 5.4.1
    | M_8PSK                // S2 5.4.2
    | M_8APSK_2_4_2         // S2x 5.4.2
    | M_16APSK_4_12         // S2 5.4.3
    | M_16APSK_8_8          // S2x 5.4.3
    | M_32APSK_4_12_16      // S2 5.4.4
    | M_32APSK_4_12_16RB    // S2x 5.4.4
    | M_32APSK_4_8_4_16     // S2 5.4.4
    | M_64APSK_16_16_16_16  // S2x 5.4.5
    | M_64APSK_8_16_20_20   // S2x 5.4.5
    | M_64APSK_4_12_20_28   // S2x 5.4.5
    | M_128APSK             // S2x 5.4.6
    | M_256APSK_1           // S2x 5.4.7
    | M_256APSK_2           // S2x 5.4.7

type Modcod = {
    PlsCode : uint8;
    LdpcRate : LdpcRate
    Modulation : Modulation
}

let DvbS2Modcods = [
    { PlsCode = 1uy; LdpcRate = (1, 4); Modulation = M_QPSK };
    { PlsCode = 2uy; LdpcRate = (1, 3); Modulation = M_QPSK };
    { PlsCode = 3uy; LdpcRate = (2, 5); Modulation = M_QPSK };
    { PlsCode = 4uy; LdpcRate = (1, 2); Modulation = M_QPSK };
    { PlsCode = 5uy; LdpcRate = (3, 5); Modulation = M_QPSK };
    { PlsCode = 6uy; LdpcRate = (2, 3); Modulation = M_QPSK };
    { PlsCode = 7uy; LdpcRate = (3, 4); Modulation = M_QPSK };
    { PlsCode = 8uy; LdpcRate = (4, 5); Modulation = M_QPSK };
    { PlsCode = 9uy; LdpcRate = (5, 6); Modulation = M_QPSK };
    { PlsCode = 10uy; LdpcRate = (8, 9); Modulation = M_QPSK };
    { PlsCode = 11uy; LdpcRate = (9, 10); Modulation = M_QPSK };
    { PlsCode = 12uy; LdpcRate = (3, 5); Modulation = M_8PSK };
    { PlsCode = 13uy; LdpcRate = (2, 3); Modulation = M_8PSK };
    { PlsCode = 14uy; LdpcRate = (3, 4); Modulation = M_8PSK };
    { PlsCode = 15uy; LdpcRate = (5, 6); Modulation = M_8PSK };
    { PlsCode = 16uy; LdpcRate = (8, 9); Modulation = M_8PSK };
    { PlsCode = 17uy; LdpcRate = (9, 10); Modulation = M_8PSK };
    { PlsCode = 18uy; LdpcRate = (2, 3); Modulation = M_16APSK_4_12 };
    { PlsCode = 19uy; LdpcRate = (3, 4); Modulation = M_16APSK_4_12 };
    { PlsCode = 20uy; LdpcRate = (4, 5); Modulation = M_16APSK_4_12 };
    { PlsCode = 21uy; LdpcRate = (5, 6); Modulation = M_16APSK_4_12 };
    { PlsCode = 22uy; LdpcRate = (8, 9); Modulation = M_16APSK_4_12 };
    { PlsCode = 23uy; LdpcRate = (9, 10); Modulation = M_16APSK_4_12 };
    { PlsCode = 24uy; LdpcRate = (3, 4); Modulation = M_32APSK_4_12_16 };
    { PlsCode = 25uy; LdpcRate = (4, 5); Modulation = M_32APSK_4_12_16 };
    { PlsCode = 26uy; LdpcRate = (5, 6); Modulation = M_32APSK_4_12_16 };
    { PlsCode = 27uy; LdpcRate = (8, 9); Modulation = M_32APSK_4_12_16 };
    { PlsCode = 28uy; LdpcRate = (9, 10); Modulation = M_32APSK_4_12_16 };
]

// DVB-S2x MODCODS, see EN 302 307-2, section 5.5.2.2
let DvbS2xModcods = [ ]


[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
