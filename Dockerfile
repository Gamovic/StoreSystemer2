# Tell the container what platform is should simulate. I'm telling it to run linux ubuntu bionic.
FROM ubuntu:bionic

# This is where you tell the container what files you want to put inside
# I just copy my entire "Build_Linux" folder into the base directory "." of the container
COPY ../SystemerProjekt/BuildsByCustomInspector/ .

# I'm running my game server on port 7777. You have to tell docker what ports you plan to use
EXPOSE 15672

# What command should it run when you start the container? 
# This is just a linux command that runs "build.x86_64" in the roLinu	ot directory "."
# Change that to whatever you named your exported build
CMD ./LinuxServer