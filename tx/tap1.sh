#!/bin/bash 

sudo ip tuntap add dev tap1 mode tap 
sudo ip link set dev tap1 address 02:00:48:55:4c:4c 
sudo ip addr add 44.0.1.1/24 broadcast 44.0.1.255 dev tap1 
sudo ip link set tap1 up sudo ip link set tap1 arp off