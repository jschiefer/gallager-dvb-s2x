#!/bin/bash

sudo ip tuntap add dev tap0 mode tap
sudo ip link set dev tap0 address 02:00:48:55:4c:4b
sudo ip addr add 44.0.0.1/24 broadcast 44.0.0.255 dev tap0
sudo ip link set tap0 up sudo ip link set tap0 arp off
sudo sysctl -w net.ipv4.conf.all.rp_filter=0
sudo sysctl -w net.ipv4.conf.enp0s25.rp_filter=0

