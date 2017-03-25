module Hamstr.Gallager

open System

type Standard = 
    | DvbS2
    | DvbS2x

type ``LDPC Code Identifier`` = int * int // should be LDPC rate

type FECFRAME = 
    | Short of byte[]
    | Medium of byte[]
    | Long of byte[]

type Modulation = 
    | M_PI2BPSK
    | M_QPSK
    | M_8PSK
    | M_8APSK_2_4_2
    | M_16APSK_4_12
    | M_16APSK_8_8
    | M_32APSK_4_12_16
    | M_32APSK_4_12_16RB
    | M_32APSK_4_8_4_16
    | M_64APSK_16_16_16_16
    | M_64APSK_8_16_20_20
    | M_64APSK_4_12_20_28
    | M_128APSK
    | M_256APSK_1
    | M_256APSK_2

type Modcod = {
    PlsCode : uint8;
    ``LDPC Code Identifier`` : ``LDPC Code Identifier``
    Modulation : Modulation;
    Standard : Standard;
}

// MODCODS, see EN 302 307-1 (DVB-S2) or EN 302 307-2 (DVB-S2x), section 5.5.2.2
let MODCODS = [
    { PlsCode = 1uy; ``LDPC Code Identifier`` = (1, 4); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 2uy; ``LDPC Code Identifier`` = (1, 3); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 3uy; ``LDPC Code Identifier`` = (2, 5); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 4uy; ``LDPC Code Identifier`` = (1, 2); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 5uy; ``LDPC Code Identifier`` = (3, 5); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 6uy; ``LDPC Code Identifier`` = (2, 3); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 7uy; ``LDPC Code Identifier`` = (3, 4); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 8uy; ``LDPC Code Identifier`` = (4, 5); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 9uy; ``LDPC Code Identifier`` = (1, 4); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 10uy; ``LDPC Code Identifier`` = (9, 10); Modulation = M_8PSK; Standard = DvbS2 };
    { PlsCode = 11uy; ``LDPC Code Identifier`` = (2, 5); Modulation = M_16APSK_8_8; Standard = DvbS2 };
    { PlsCode = 12uy; ``LDPC Code Identifier`` = (1, 2); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 13uy; ``LDPC Code Identifier`` = (3, 5); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 14uy; ``LDPC Code Identifier`` = (2, 3); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 15uy; ``LDPC Code Identifier`` = (3, 4); Modulation = M_QPSK; Standard = DvbS2 };
    { PlsCode = 16uy; ``LDPC Code Identifier`` = (4, 5); Modulation = M_QPSK; Standard = DvbS2 };
]


[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
