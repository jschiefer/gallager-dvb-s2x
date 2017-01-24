& docker run -it --rm --privileged -p 5901:5901 -e USER=amsat p4g:0.3 /bin/bash -c "vncserver :1 -geometry 1280x800 -depth 24 && tail -F /root/.vnc/*.log"
