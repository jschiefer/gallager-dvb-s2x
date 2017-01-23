# Docker image for experimentation with Phase4Ground code

This is a based on Stefan Wunsch's https://github.com/stwunsch/docker-pybombs-gnuradio and friends.

The docker image provides a 3.7.10 version of gnu radio, the UHD driver and a special collection of Phase4Ground 
blocks. The image is based on Ubuntu 14.04.

Build container
-------------
To build the container, clone this repository and build a docker image from it:
```
$ docker build -t p4g --squash .
```
This will create the image, which you should be able to see in your image list using
```
$ docker images
```

Run container
-------------

You can adjust the mapped VNC server port with altering the argument
'-p 5901:PORT' and the screen size with changing the '-geometry WIDTHxHEIGHT'
part.

You will be asked to set a password for the access via VNC.

Start a container, using the image that we just built:

```
$ docker run -it --rm --privileged -p 5901:5901 -e USER=amsat p4g /bin/bash -c \
    "vncserver :1 -geometry 1280x800 -depth 24 && tail -F /root/.vnc/*.log"
```

This starts the VNC server in the image and exposes it on port 5901. Prileged access is
required in order to access external devices.

Connect via VNC client
----------------------

I assume that you are running the container on the same machine you are
running the VNC client. Then you can connect via the ip address
'localhost:5901'. For example using vncviewer:

```
$ vncviewer localhost:5901
```

Run GNU Radio Companion
-----------------------

Simply open a terminal with the appropriate button in Start/System Tools/XTerm
and run 'gnuradio-companion' from the terminal.

Install new GNU Radio Out-of-Tree modules
-----------------------------------------

The image contains a full PyBOMBS installation. So feel free to run PyBOMBS
and install new packages. For example:

```
$ pybombs install gr-radar
```