# Welcome to your CDK C# project!

This is the CDK project to deploy ECS service per feature branch for Event Broker UI MVC application on Amazon ECS with CDK.

The `cdk.json` file tells the CDK Toolkit how to execute your app.

It uses the [.NET Core CLI](https://docs.microsoft.com/dotnet/articles/core/) to compile and execute your project.

## Useful commands

* `dotnet build src` compile this app
* `cdk deploy`       deploy this stack to your default AWS account/region
* `cdk diff`         compare deployed stack with current state
* `cdk synth`        emits the synthesized CloudFormation template

## Prerequisites 
* CDK bootstrap
   `cdk bootstrap aws://<Account-ID>/<AWS Region>` To be done once per CDK environment
* Set AWS credentials - Access Key ID, Secret Access key and AWS region as default profile or Environment variables
* Following SSM parameters should exist prior to deploying stack 

  * `/eventbroker/ebuirestsvc/vpc`                                VPC ID to use for Event Broker UI Rest Service 
  * `/eventbroker/ebuirestsvc/publicsubnets`                      Public subnets to be used by AWS CDK for Event Broker UI Rest Service 
  * `/eventbroker/ebuirestsvc/publicsubnetroutetableids`          Public subnets route table ids to be used by AWS CDK for Event Broker UI Rest Service  
  * `/eventbroker/ebuirestsvc/privatesubnets`                     Private subnets to be used by AWS CDK for Event Broker UI Rest Service 
  * `/eventbroker/ebuirestsvc/privatesubnetroutetableids`         Private subnets route table ids to be used by AWS CDK for Event Broker UI Rest Service  
  * `/eventbroker/ebuirestsvc/azs`                                AZs to be used by AWS CDK for Event Broker UI Rest Service 
  * `/eventbroker/ebuirestsvc/cert`                               ACM certificate ARN for domain (eg for filmtrack.dev)
  * `/eventbroker/ebuirestsvc/domain`                             Route53 domain name to be used by AWS CDK for Event Broker UI Rest Service 
  * `/eventbroker/ebuirestsvc/zoneid`                             Route53 hosted zone ID to be used by AWS CDK for Event Broker UI Rest Service 

* ECR image tagged with <feature name>-<build number> should be present in the ECR repository created as part of EventBroker/CodeSamples/ecs build pipeline

## Deploy stack  

Navigate to EbUIRestSvc directory

Syntax:
* `cdk deploy -c Environment=<Env name> -c Stage=<Stage name> -c FeatureName=<feature name> -c BuildNumber=<build number> --require-approval never` 

Note: build number generated should be unique per feature

Command:

For featurex with build number 1234
* `cdk deploy -c Environment=nonprod -c Stage=dev -c FeatureName=featurex -c BuildNumber=1234 --require-approval never`

For featurey with build number 5678
* `cdk deploy -c Environment=nonprod -c Stage=dev -c FeatureName=featurey -c BuildNumber=5678 --require-approval never`

For master with build number 1111
* `cdk deploy -c Environment=nonprod -c Stage=dev -c FeatureName=master -c BuildNumber=1111 --require-approval never`
..

## Test stack 

Assuming that the R53 domain=filmtrack.dev and stage name=dev,
* Navigate to your web browser and type https://<feature name>.filmtrack.dev/events to display app data
* Navigate to your web browser and type https://<feature name>.filmtrack.dev/events/health to test app health 

Replace "feature name" for the deployed service in the URL

## Destroy stack 

Navigate to EbUIRestSvc directory

Syntax:
* `cdk destroy -c Environment=<Env name> -c Stage=<Stage name> -c FeatureName=<feature name> -c BuildNumber=<build number> --force` 

Command:

For featurex with build number 1234
* `cdk destroy -c Environment=nonprod -c Stage=dev -c FeatureName=featurex -c BuildNumber=1234 --force` 

For featurey with build number 5678
* `cdk destroy -c Environment=nonprod -c Stage=dev -c FeatureName=featurey -c BuildNumber=5678 --force` 

For master with build number 1111
* `cdk destroy -c Environment=nonprod -c Stage=dev -c FeatureName=master -c BuildNumber=1111 --force` 
..