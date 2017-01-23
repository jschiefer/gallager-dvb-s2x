# PyBOMBS

# Derive from Ubuntu
FROM ubuntu:14.04

# Set prefix variables
ENV TheUser amsat
ENV TheHomeDir /home/${TheUser}
ENV PyBOMBS_prefix myprefix
ENV PyBOMBS_init ${TheHomeDir}/pybombs

# Update apt-get
RUN apt-get update

# Install PyBOMBS dependencies
RUN apt-get install -y \
        python-pip \
        python-yaml \
        python-apt \
        python-setuptools \
        git-core

# Install PyBOMBS
RUN pip install PyBOMBS

# Create user amsat, and enable sudo
RUN groupadd -r ${TheUser} && useradd -m -r -g ${TheUser} ${TheUser} && usermod -aG sudo ${TheUser}
RUN echo 'amsat ALL=(ALL) NOPASSWD:ALL' >> /etc/sudoers
WORKDIR ${TheHomeDir}

# Add recipes to PyBOMBS
RUN pybombs recipes add gr-recipes git+https://github.com/gnuradio/gr-recipes.git
RUN pybombs recipes add gr-etcetera git+https://github.com/gnuradio/gr-etcetera.git

# Setup environment
RUN pybombs prefix init ${PyBOMBS_init} -a ${PyBOMBS_prefix}
RUN echo "source "${PyBOMBS_init}"/setup_env.sh" >> .bashrc
  
# Install packages
RUN pybombs -p ${PyBOMBS_prefix} -v install "uhd" && rm -rf ${PyBOMBS_init}/src/*
RUN pybombs -p ${PyBOMBS_prefix} -v install "rtl-sdr" && rm -rf ${PyBOMBS_init}/src/*
RUN pybombs -p ${PyBOMBS_prefix} -v install "gr-osmosdr" && rm -rf ${PyBOMBS_init}/src/*
RUN pybombs -p ${PyBOMBS_prefix} -v install --deps-only "gnuradio" && rm -rf ${PyBOMBS_init}/src/*
RUN pybombs -p ${PyBOMBS_prefix} -v install --no-deps "gnuradio" && rm -rf ${PyBOMBS_init}/src/*

# Install LXDE and VNC server
RUN DEBIAN_FRONTEND=noninteractive apt-get install -y lxde-core lxterminal tightvncserver

# Expose ports.
EXPOSE 5901

# Change directory ownwership
RUN chown ${TheUser}:${TheUser} ${PyBOMBS_init}

# Switch user
USER ${TheUser}