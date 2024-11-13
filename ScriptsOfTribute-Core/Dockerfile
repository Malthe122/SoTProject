FROM ubuntu:22.04

# Set non-interactive mode for apt-get
RUN export DEBIAN_FRONTEND=noninteractive

# Update and install necessary packages
RUN apt-get update && \
    apt-get install -y \
    git \
    vim \
    nano \
    wget \
    python3 \
    python3-pip \
    dotnet-sdk-7.0 \
    aspnetcore-runtime-7.0

# Copy project files
COPY . /sot
