module Hamstr.Ldpc.Math

// How we describe a databit (LLR, effectively)
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
    // TODO: Boolean conversion
    // static member AsBoolean(a : FloatLLR) = a.LLR > 0.0;
        