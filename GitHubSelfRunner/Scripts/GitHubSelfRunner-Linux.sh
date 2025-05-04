#!/bin/bash

# Configuraton
GitHubPAT=""
Secret=""
DefaultImage=""
OutputDir=""
DockerImage="ghcr.io/nano-dna-studios/githubselfrunner-server:latest"
OutputPort=8080

# Run the Container
docker run -d --name githubselfrunner-webhookserver -e GitHubPAT=$GitHubPAT -e Secret=$Secret -e DefaultImage=$DefaultImage --privileged -v /var/run/docker.sock:/var/run/docker.sock -v $OutputDir:/GitHubSelfRunner/Cache -p $OutputPort:8080 $DockerImage

sleep 1

docker logs -f githubselfrunner-webhookserver &