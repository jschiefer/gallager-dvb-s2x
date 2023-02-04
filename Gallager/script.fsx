#I __SOURCE_DIRECTORY__
#r "bin/Debug/net6.0/Gallager.dll"

open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Util

let dataDir = "Data/"
let bitFileName = dataDir + "qpsk_testdata.bits"
let iqDataFileName = dataDir + "qpsk_testdata.out"
// File parameters
let frameType = { frameSize = Long; codeRate = Rate_1_2 }
let modulation = M_QPSK
let referenceFileType = BytePerBit(frameType)
let referenceFrame = readTestFile referenceFileType bitFileName 
let iqFileType = IqFile(frameType, modulation)
let frame = readTestFile iqFileType iqDataFileName
compareArrays referenceFrame.bits frame.bits
createDegreeTable()


let checkParity (frame : FECFRAME) =
    let checkParityBool (frameType : FrameType) (bits : bool[]) =
        let codingTableEntry = findCodingTableEntry frameType
        // find out how many of these bits are parity vs. user data
        let (nDataBits, nParityBits) = findSize codingTableEntry
        // Go through the parity equations that are encoded in the tables
        // and see whether they all add up to zero???

        true

    frame.bits
    |> Array.map (fun b -> b.ToBool)
    |> checkParityBool frame.frameType

checkParity frame

// Observe that the last column (the parity bits) has the wrong degree 
// (1 instead of 2) 