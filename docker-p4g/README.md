# Docker image for experimentation with Phase4Ground code

This is a based on Stefan Wunsch's https://github.com/stwunsch/docker-pybombs-gnuradio and friends.

The docker image provides a 3.7.10 version of gnu radio, the UHD driver and a special collection of Phase4Ground 
blocks. The image is based on Ubuntu 14.04.

TODO:
-----
- How does the override work for pybomb definitions?
- Add gr-iio, limesuite in new versions
- Add whatever else might be required for LimeSDR
- Upgrade to Ubuntu 16.04
- Push to Docker repository
- Run ldconfig after pybombs

Build container
-------------
To build the container, clone this repository, change directory into to `docker-p4g` and build the docker image from it:
```
$ docker build -t p4g:0.7 .
```
This will run for a while, and eventually create an image with the name p4g and the version tag 0.7, which you 
should be able to see in your image list:
```
$ docker images
REPOSITORY          TAG                 IMAGE ID            CREATED             SIZE
p4g                 0.7                 271950d2732a        8 weeks ago         2.47 GB
ubuntu              14.04               b969ab9f929b        2 months ago        188 MB

```

Run container
-------------

Now that we have this image, let's create a container from it and run it 
interactively using the `docker run` command. It is considered a best 
practice to consider docker containers ephemeral, i.e. we are not going to 
preserve any state inside the container, and destroy all evidence after 
it is done running.

There are a few more things we want to do with this:

- Map a local directory, so it is visible inside the container. This is done with
  the `-v` option.
- Allow access to external devices, e.g. your SDR hardware. This requires
  `--privileged`.
- Run a vnc server inside the container, so we can get a GUI out of it. The command
  to run this is added to the very end of our lengthy command line. 

You can adjust the mapped VNC server port with altering the argument
'-p 5901:PORT' and the screen size with changing the '-geometry WIDTHxHEIGHT'
part.

You will be asked to set a password for the access via VNC.

Start a container, using the image that we just built. For example, under Linux:

```
docker run -it --rm --privileged -p 5901:5901 -e USER=amsat -v ~/Docker:/home/amsat/Docker p4g:0.7 /bin/bash -c "vncserver :1 -geometry 1920x1080 -depth 24 && tail -F /root/.vnc/*.log"
```

Or, under Windows:
```
docker run -it --rm --privileged -p 5901:5901 -e USER=amsat -v C:/home/jan p4g:0.7 /bin/bash -c "vncserver :1 -geometry 1920x1080 -depth 24 && tail -F /root/.vnc/*.log"
```

This starts the VNC server in the image and exposes it on port 5901. Prileged access is
required in order to access external devices. As it the server starts, it will make 
you set a password.

Connect via VNC client
----------------------

I assume that you are running the container on the same machine you are
running the VNC client. Then you can connect via the ip address
'localhost:5901'. For example using vinagre:

```
$ vinagre localhost:5901
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