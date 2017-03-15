
type Standard = 
    | DvbS2
    | DvbS2x

type ``LDPC Code Identifier`` = int * int

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

let MODCODS = [
    { PlsCode = 1uy; ``LDPC Code Identifier`` = (1, 4); Modulation = M_QPSK; Standard = DvbS2 };
]
