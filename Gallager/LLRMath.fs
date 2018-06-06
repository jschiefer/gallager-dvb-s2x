namespace FSharp.Numerics

// The Log-Likelihood Ratio (LLR) is a real number that represents a "soft" bit,
// typically the output of a demodulator.  It is computed by taking the natural 
// logarithm of the ratio of probability that the bit is 1, divided by the probability
// that the bit bit is zero. The sign of the value represents the 'hard' decision,
// and the magnitude represents the reliability of the decision.

// This type defines the implementation of an algebra for LLRs, implemented in 
// double-precision floating point. This is typically way too inefficient for a 
// real-time implementation, but useful for experimentation, and to understand
// the algorithms.

// Other than creation of values and conversions to other data types, we will need
// the addition operator, often written as "box-plus". Two different implementations
// are available, an exact one (+) and an approximated one (<+). Adding 
// LLR(a) + LLR(b) is equivalent to the LLR of the modulo-2 sum of a and b.

// cf. Moon, Todd K.: Error Correction Coding: Mathematical Methods  and Algorithms

// More useful links on understanding LLR algebra:
// http://onlinelibrary.wiley.com/doi/10.1002/0471739219.app1/pdf 
// http://elib.dlr.de/48687/1/main.pdf 
// http://wireless.ece.ufl.edu/eel6550/lit/sklar_primer.pdf 

type LLR = 
    | LLR of float
    with
        /// Create an LLR value from a boolean. Our confidence is infinite.
        static member Create(b : bool) = 
            match b with
            | true -> LLR(-infinity)
            | false -> LLR(infinity)
        static member Create b = LLR.Create(b <> 0uy)
        static member Create(f : float) = LLR f
        static member Undecided = LLR 0.0
        static member Zero = LLR.Create(false)
        static member One = LLR.Create(true)
        member x.ToFloat = let (LLR n) = x in n
        member x.ToBool = x.ToFloat > 0.0

        /// Addition - approximation.
        static member (<+>) (a : LLR, b : LLR) = 
            let (LLR a') = a
            let (LLR b') = b
            LLR.Create(float (sign a' * sign b') * (min (abs a') (abs b')))
        
        /// Addition - exact algorithm
        static member (+) (a : LLR, b : LLR) =
            let (LLR a') = a
            let (LLR b') = b
            let atanh x = (log(1.0 + x) - log(1.0 - x)) / 2.0
            LLR.Create(2.0 * atanh(tanh(a' / 2.0) * tanh(b' / 2.0)))
