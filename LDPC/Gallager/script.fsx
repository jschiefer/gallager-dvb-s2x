(*
#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open System
open System.Numerics
open Hamstr.Ldpc.Math
open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.DvbS2Tables
open Hamstr.Demod
open Hamstr.Ldpc.Decoder
*)

// TODO:
// - Implement AsBoolean
// - Properly implement the modulo-2 addition

type FloatLLR = 
    | LLR of float

    static member (<+>) (a : FloatLLR, b: FloatLLR) = a 
    static member Create(b : byte) = 
        match b with
        | 0uy -> LLR(-1.0)
        | _ -> LLR(1.0)
    static member Create(b : bool) = 
        match b with
        | false -> LLR(-1.0)
        | true -> LLR(1.0)
    static member Create(f : float) = LLR(f)
    // static member AsBoolean(a : FloatLLR) = a.LLR > 0.0;
        

 let a = LLR(-2.0)
