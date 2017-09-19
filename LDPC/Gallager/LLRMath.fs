namespace FSharp.Numerics

// cf. Moon, Todd K.: Error Correction Coding: Mathematical Methods  and Algorithms

type LLR = 
    | LLR of float

    static member Create(b : byte) = 
        match b with
        | 0uy -> LLR(1.0)
        | _ -> LLR(-1.0)

    static member Create(b : bool) = 
        match b with
        | false -> LLR(1.0)
        | true -> LLR(-1.0)

    static member Create(f : float) = LLR f
        
    static member Undecided() = LLR 0.0
    static member Zero = LLR.Create(false)
    static member One = LLR.Create(true)
    member x.ToFloat = let (LLR n) = x in n

    member x.ToBool = x.ToFloat > 0.0
        
    static member (+) (a : LLR, b: LLR) = 
        let (LLR a') = a
        let (LLR b') = b
        LLR (a' + b')
