#r "bin/Debug/netcoreapp1.1/Gallager.dll"

open Hamstr.Ldpc.DvbS2Tables

let tablesToProcess =
    [
        (ldpc_23_36N, "ldpc_23_36N");
        (ldpc_25_36N, "ldpc_25_36N");
        (ldpc_13_18N, "ldpc_13_18N");
        (ldpc_7_9N, "ldpc_7_9N");
        (ldpc_90_180N, "ldpc_90_180N");
        (ldpc_96_180N, "ldpc_96_180N");
        (ldpc_100_180N, "ldpc_100_180N");
        (ldpc_104_180N, "ldpc_104_180N");
        (ldpc_116_180N, "ldpc_116_180N");
        (ldpc_124_180N, "ldpc_124_180N");
        (ldpc_128_180N, "ldpc_128_180N");
        (ldpc_132_180N, "ldpc_132_180N");
        (ldpc_135_180N, "ldpc_135_180N");
        (ldpc_140_180N, "ldpc_140_180N");
        (ldpc_154_180N, "ldpc_154_180N");
        (ldpc_18_30N, "ldpc_18_30N");
        (ldpc_20_30N, "ldpc_20_30N");
        (ldpc_22_30N, "ldpc_22_30N");
        (ldpc_1_4S, "ldpc_1_4S");
        (ldpc_1_3S, "ldpc_1_3S");
        (ldpc_2_5S, "ldpc_2_5S");
        (ldpc_1_2S, "ldpc_1_2S");
        (ldpc_3_5S, "ldpc_3_5S");
        (ldpc_2_3S, "ldpc_2_3S");
        (ldpc_3_4S, "ldpc_3_4S");
        (ldpc_4_5S, "ldpc_4_5S");
        (ldpc_5_6S, "ldpc_5_6S");
        (ldpc_8_9S, "ldpc_8_9S");
        (ldpc_11_45S, "ldpc_11_45S");
        (ldpc_4_15S, "ldpc_4_15S");
        (ldpc_14_45S, "ldpc_14_45S");
        (ldpc_7_15S, "ldpc_7_15S");
        (ldpc_8_15S, "ldpc_8_15S");
        (ldpc_26_45S, "ldpc_26_45S");
        (ldpc_32_45S, "ldpc_32_45S");
        (ldpc_1_5M, "ldpc_1_5M");
        (ldpc_11_45M, "ldpc_11_45M");
        (ldpc_1_3M, "ldpc_1_3M");
    ]

let printTable ((t : int [][]), name) = 
    printfn "let %s =\n\t[|" name
    t |> Array.iter (fun a -> 
        let lineLen = a.[0]
        let shorty = a |> Array.tail |> Array.take lineLen
        printfn "\t\t%A;" shorty
    )
    printfn "\t|]"

printTable tablesToProcess.[0]
tablesToProcess |> List.iter printTable
