# Welcome to your .NET C# project!

This is the .NET project to deploy Sample .NET 6 Web API Application for Event Broker UI on Amazon ECS 

## What does the App do?

The application has two HTTP GET API endpoints 

  * `/events`                             Custom app logic to return response with OS, Runtime etc and feature information 
  * `/events/health`                      Returns status code 200 OK with status message "Healthy" for ALB health check 

The application is configured to fetch all parameters from AWS Systems Manager from path `/eventbroker/ebuirestsv` at startup . The `appname` parameter in that hierarchy will be used by AWS X ray service map for tracing API calls.

## Prerequisites 
* Foundation CDK stack "EbFoundOne" should be deployed prior to deploying this stack
* Following AWS SSM parameter should exist prior to deploying application to ECS

  * `/eventbroker/ebuirestsvc/appname`                            This contains a string value for appName used by Xray for generating service map 

## ECS configuration (refer EventBroker/EbUIRestSvc CDK stack code)
* This configuration includes X ray daemon to be deployed as a sidecar with this application
* ECS task role includes permissions for AWS Systems Manager Parameter store access and AWS X ray write access
* ECS task container and X ray container definition includes environment variable for AWS_REGION where this task will be deployed

## Build application locally

* ACCOUNT,ECRREPONAME,REGION are hardcoded values in Bash script EventBroker/CodeSamples/ecs-bamboo-build-steps.sh right now. 
* Run the bash script: sh ecs-bamboo-build-steps.sh
* Input feature 
* Input build
* Script runs, builds image, tags it, login to the ECR repo and pushes image with tag "feature name-build number"

## Changes to include in bash script (Bamboo build)
* Change the hardcoded local variable values for ACCOUNT,ECRREPONAME,REGION in Bash script ecs-bamboo-build-steps.sh to read from Bamboo variables
* Change inputs "feature" and "build" to read from Bamboo default variables. Ensure build number generated and passed to the script is unique for every build of the feature.

## Test 
Image with tag "feature name-build number" is pushed to ECR