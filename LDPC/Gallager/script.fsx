#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open System
open System.Numerics
open FSharp.Numerics
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.DvbS2Tables
open Hamstr.Demod
open Hamstr.Ldpc.Decoder

let a = LLR -2.1
let b = LLR 1.3

a + b

type Foo =
    | Foo of float
    member x.ToFloat = 
        let (Foo a) = x in a
    member x.ToFloat' =
        match x with
        | Foo a -> a

