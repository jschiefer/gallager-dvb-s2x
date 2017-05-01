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

let s = 1.0 / sqrt(2.0)
demodulateSymbol 0.02 M_QPSK (new Complex(s, s))


/// Create a list of check nodes for bit node at index b
let checkNodes b = ()

/// Create a list of bit nodes for the check node at index c
let bitNodes c = ()

let pt = findParityTable (1, 2)
