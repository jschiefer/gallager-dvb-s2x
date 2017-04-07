#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open System
open System.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Demod

/// One meager test symbol
let sym = 
    [ 
        Complex(-0.640049576759338, 0.511995136737823); 
        Complex(-0.56793886423111, -0.883610665798187) 
    ]

let cons = constellation M_QPSK




