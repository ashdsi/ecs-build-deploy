# Welcome to your CDK C# project!

This is the CDK project to deploy Foundation stack for .NET application on Amazon ECS with CDK.

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

* Existing VPC with public and private subnets and configured for default routes to Internet Gateway and NAT gateway respectively.
* Route53 public hosted zone. 
* Certificate for the public Route53 domain. 

For the 'dev' environment, ensure the following SSM parameters exist prior to deploying the stack 

  * `/ecs/app/dev/vpc`                                VPC ID to be used by AWS CDK 
  * `/ecs/app/dev/publicsubnets`                      Public subnets to be used by AWS CDK (comma seperated values)
  * `/ecs/app/dev/publicsubnetroutetableids`          Public subnets route table ids to be used by AWS CDK (comma seperated values)
  * `/ecs/app/dev/privatesubnets`                     Private subnets to be used by AWS CDK (comma seperated values)
  * `/ecs/app/dev/privatesubnetroutetableids`         Private subnets route table ids to be used by AWS CDK (comma seperated values)
  * `/ecs/app/dev/azs`                                AZs to be used by AWS CDK (comma seperated values)
  * `/ecs/app/dev/cert`                               ACM certificate ARN for public Route53 domain 
  * `/ecs/app/dev/domain`                             Route53 domain name to be used by AWS CDK 
  * `/ecs/app/dev/zoneid`                             Route53 hosted zone ID to be used by AWS CDK 
  * `/ecs/app/appname`                                X-Ray Application Name to be used by AWS CDK 

Note: 
The ACM certificate should be created for the domains "domain_name" and wildcard "*.domain_name". 
If the domain is example.com, then both example.com and *.example.com should be registered domains for the ACM certificate.

## Deploy stack  

Navigate to 'foundation' directory

Syntax:
* `cdk deploy -c Environment=<Env name> -c Stage=<Stage name> --require-approval never` 

Command: 
* `cdk deploy -c Environment=nonprod -c Stage=dev --require-approval never` 

## Test stack 

Assuming that the R53 domain=example.com and stage name=dev,
Navigate to your web browser and type https://test-dev.example.com. The response should be HTTP 404 "Sorry! No services found" coming from the default ALB listener rule fixed response target.

## Destroy stack 

Navigate to 'foundation' directory

Syntax:
* `cdk destroy -c Environment=<Env name> -c Stage=<Stage name> --force` 

Command:
* `cdk destroy -c Environment=nonprod -c Stage=dev --force` 
