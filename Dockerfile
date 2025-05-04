FROM ubuntu:22.04

# Install docker
RUN apt-get update && apt-get install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    wget \
    software-properties-common && \
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | apt-key add - && \
    add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu focal stable" && \
    apt-get update && \
    apt-get install -y docker-ce docker-ce-cli containerd.io

# Install .NET Runtime
RUN apt-get update && \
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
dpkg -i packages-microsoft-prod.deb && \
apt-get update && \
apt-get install -y aspnetcore-runtime-8.0

# Copy your published app
COPY ./GitHubSelfRunner/bin/Release/net8.0/linux-x64/publish /GitHubSelfRunner

# Make sure OutputLogs exists
RUN mkdir /GitHubSelfRunner/OutputLogs

# Expose the 8080 Port for the Webhook Server
EXPOSE 8080

# Final command to run
CMD ["/bin/bash", "-c", "\
/GitHubSelfRunner/GitHubSelfRunner registerserver \"$GitHubPAT\" \"$Secret\" \"$DefaultImage\" 8080 /GitHubSelfRunner/OutputLogs && \
/GitHubSelfRunner/GitHubSelfRunner startserver"]
