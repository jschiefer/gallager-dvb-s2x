#I __SOURCE_DIRECTORY__
#r "bin/Debug/net461/Gallager.dll"
#r "Kludge/netstandard.dll"

open Hamstr.Ldpc.DvbS2
open Hamstr.Ldpc.Decoder
open Hamstr.Ldpc.Util

let dataDir = @"C:\Users\jas\Develop\p4g\gallager-dvb-s2x\"

let bitFileName = dataDir + "Data/qpsk_testdata.bits"
let iqDataFileName = dataDir + "Data/qpsk_testdata.out"

// File parameters
let frameType = { frameSize = Long; codeRate = Rate_1_2 }
let modulation = M_QPSK

let referenceFileType = BytePerBit(frameType)
let referenceFrame = readTestFile referenceFileType bitFileName 

let iqFileType = IqFile(frameType, modulation)
let frame = readTestFile iqFileType iqDataFileName

compareArrays referenceFrame.bits frame.bits

createDegreeTable()

// Observe that the last column (the parity bits) has the wrong degree (1 instead of 2) 
