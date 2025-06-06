#!/bin/bash

# Configuraton
GitHubPAT=""
Secret=""
DefaultImage=""
OutputDir=""
DockerImage="ghcr.io/nano-dna-studios/githubselfrunner:latest"
OutputPort=8080

# Run the Container
docker run -d --name githubselfrunner-webhookserver -e GitHubPAT=$GitHubPAT -e Secret=$Secret -e DefaultImage=$DefaultImage --privileged -e DOCKER_HOST=tcp://host.docker.internal:2375 -v $OutputDir:/GitHubSelfRunner/Cache -p $OutputPort:8080 $DockerImage &

sleep 1

docker logs -f githubselfrunner-webhookserver &