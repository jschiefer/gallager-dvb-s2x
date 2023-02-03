# Gallager-DVB-S2x
This is an experimental reference implementation for a DVB-S2x modem, written in F#. It is
named after [Robert G. Gallager, who invented LDPC codes in 1963 and described them in his Ph.D. thesis. 
Implmentation of LDPC codes wasn't practical at the time, so this work was forgotten.

In the mid-nineties, people were looking for alternatives to the powerful, but very heavily patent-encumbered Turbo codes, and 
David J.C. McKay et. al. rediscovered Gallager's work. Since then, LDPC codes have been used in a variety of applications, from 
hard drives to deep space networks. 

The code in this repository implements the LDPC codes for the DVB-S2 and DVB-S2x digital satellite TV standards. The 
purpose of this is to provide a starting point to understand the algorithms and issues, in the hope of furthering
the cause of an Open Source implementation of a ground station for satellite ham radio use.

This code has been developed using .Net Core (currently net6.0), mostly on Linux. The choice of implementation
language was driven by the desire to take advantage of the exceptional clarity and brevity of F#. Similar results could 
be expected with other OCaml-derived languages.
