#!/bin/bash

#Hardcoded for now. In build pipeline, define as build variables
ACCOUNT=769749008949
ECRREPONAME=sampleapp-ecr-dev
REGION=us-east-1

#Obtained as user input for now. In build pipeline, obtain these values from default build variables
echo -e "Enter feature name"
read feature

echo -e "Enter build number"
read build

echo "=========================================================="
echo "Using ECR repo $ECRREPONAME in the region $REGION of Account $ACCOUNT" 
echo "=========================================================="
cd ecs
echo "=========================================================="
echo "Running docker build for feature $feature and build number $build"
docker build -t $feature-$build .
echo "=========================================================="
echo "Tagging docker image for account $ACCOUNT"
docker tag $feature-$build $ACCOUNT.dkr.ecr.$REGION.amazonaws.com/$ECRREPONAME:$feature-$build

#Make sure Build server is configured with AWS credentials to login to ECR repository and push image
echo "=========================================================="
echo "Login to Amazon ECR"
$(aws ecr get-login --no-include-email --region us-east-1)
echo "=========================================================="
echo "Pushing docker image to ECR"
echo "=========================================================="
docker push $ACCOUNT.dkr.ecr.$REGION.amazonaws.com/$ECRREPONAME:$feature-$build
echo "=========================================================="
echo "Removing docker images locally"
echo "=========================================================="
docker rmi $ACCOUNT.dkr.ecr.$REGION.amazonaws.com/$ECRREPONAME:$feature-$build
docker rmi $feature-$build:latest
docker rmi $(docker images -f "dangling=true" -q)
