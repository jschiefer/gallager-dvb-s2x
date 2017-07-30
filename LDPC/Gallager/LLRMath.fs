namespace FSharp.Numerics

// How we describe a databit
type FloatLLR = 
    | FloatLLR of float

    static member Create(b : byte) = 
        match b with
        | 0uy -> FloatLLR(1.0)
        | _ -> FloatLLR(-1.0)

    static member Create(b : bool) = 
        match b with
        | false -> FloatLLR(1.0)
        | true -> FloatLLR(-1.0)

    static member Create(f : float) = FloatLLR f
        
    static member Undecided() = FloatLLR 0.0
    static member Zero = FloatLLR.Create(false)
    static member One = FloatLLR.Create(true)
    member x.ToFloat = let (FloatLLR n) = x in n

    member x.ToBool = x.ToFloat > 0.0
        
    static member (<+>) (a : FloatLLR, b: FloatLLR) = 
        let (FloatLLR a') = a
        let (FloatLLR b') = b
        FloatLLR (a' + b')
