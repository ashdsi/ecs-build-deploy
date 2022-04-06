using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.SSM;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Constructs;
using System;

namespace Foundation
{
    public class FoundationStack : Stack
    {
        internal FoundationStack(Construct scope, string id, IAStackProps props = null) : base(scope, id, props)
        {
            string stageName = props.StageName;
            string appName = "app";
            string serviceName = "ecs";

            /*----------BEGIN---------------*/
            //Prereq: Make sure the SSM parameters exist before deploying the stack
            //Import VPC ID from SSM Parameter store 
            string vpc_id = StringParameter.ValueFromLookup(this, $"/{serviceName}/{appName}/{stageName}/vpc");

            //Import public subnets from SSM Parameter store
            var public_subnets_list = StringParameter.ValueFromLookup(this, $"/{serviceName}/{appName}/{stageName}/publicsubnets");

            var public_subnets = public_subnets_list.Split(",");

            Console.WriteLine(public_subnets.Length);

            //Import private subnets from SSM Parameter store
            var private_subnets_list = StringParameter.ValueFromLookup(this, $"/{serviceName}/{appName}/{stageName}/privatesubnets");

            var private_subnets = private_subnets_list.Split(",");

            Console.WriteLine(private_subnets.Length);

            ////Import public subnet route table ids from SSM Parameter store
            //var public_subnet_routetableids_list = StringParameter.ValueFromLookup(this, $"/{serviceName}/{appName}/{stageName}/publicsubnetroutetableids");

            //var public_subnet_routetableids = public_subnet_routetableids_list.Split(",");

            //Console.WriteLine(public_subnet_routetableids.Length);


            ////Import private subnet route table ids from SSM Parameter store
            //var private_subnet_routetableids_list = StringParameter.ValueFromLookup(this, $"/{serviceName}/{appName}/{stageName}/privatesubnetroutetableids");

            //var private_subnet_routetableids = private_subnet_routetableids_list.Split(",");

            //Console.WriteLine(private_subnet_routetableids.Length);

            //Import AZs from SSM Parameter store
            string azs_list = StringParameter.ValueFromLookup(this, $"/{serviceName}/{appName}/{stageName}/azs");

            var azs = Fn.Split(",", azs_list);

            Console.WriteLine(azs.Length);

            //Reference existing VPC for DEV environment to be used for Event Broker
            //Note: SSM Parameter store values requires one subnet in each of the 3 AZs defined
            var vpc = Vpc.FromVpcAttributes(this, "vpc", new VpcAttributes
            {
                VpcId = vpc_id,
                PublicSubnetIds = public_subnets,
                PrivateSubnetIds = private_subnets,
                AvailabilityZones = azs,
                //PublicSubnetRouteTableIds = public_subnet_routetableids,
               // PrivateSubnetRouteTableIds = private_subnet_routetableids

            });

            /*-----------END-----------*/


            //Create {serviceName} cluster
            var cluster = new Cluster(this, "cluster", new ClusterProps
            {

                ClusterName = $"{appName}-{serviceName}-{stageName}",
                Vpc = vpc,
            });

            //Create {serviceName} Task Role 
            var ecsTaskRole = new Role(this, "ecstaskrole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com"),
                RoleName = $"{appName}-taskrole-{stageName}-{this.Region}",    //Region is required since the role is common to a stage in a region
                Description = "ECS Task Role created by AWS CDK"
            });

            //attach Policy for access to SSM parameters starting from path / {serviceName}/*
            ecsTaskRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new[] { "ssm:GetParametersByPath", "ssm:GetParameters", "ssm:GetParameter", "ssm:GetParameterHistory" },
                Resources = new[]
                {
                    Arn.Format(new ArnComponents
                    {
                        Service = "ssm",
                        Resource = "parameter",
                        ResourceName = $"{serviceName}/{appName}/*"      //Add any additional permissions as needed
                    }, this)
                }
            }));

            //attach Policy for X ray tracing
            ecsTaskRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AWSXrayWriteOnlyAccess"));
            //attach Policy to access ECR and stream Cloudwatch logs from ECS task
            ecsTaskRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AmazonECSTaskExecutionRolePolicy"));

            //Create ECR Repository
            //Note:ECR repository name should be lower case
            var ecrRepo = new Repository(this, "ecr", new RepositoryProps
            {
                RepositoryName = $"{appName}-ecr-{stageName}",
                ImageScanOnPush = true,
                RemovalPolicy = RemovalPolicy.DESTROY              //Default:Retain. Destroy:Removes ECR repo when stack is deleted or this resource undergoes update
            });

            //ALB security group to allow egress traffic on port 80 to target groups
            var albsgEgress = new SecurityGroup(this, "albsg", new SecurityGroupProps
            {
                SecurityGroupName = $"{appName}-albsg-{stageName}",
                Vpc = vpc,
                AllowAllOutbound = false,
            });

            //Egress rule for target group traffic and health check on port 80
            albsgEgress.AddEgressRule(Peer.AnyIpv4(), Port.Tcp(80), "All outbound on Port 80 for ECS target health check");

            //Create ALB
            var alb = new ApplicationLoadBalancer(this, "alb", new Amazon.CDK.AWS.ElasticLoadBalancingV2.ApplicationLoadBalancerProps
            {
                LoadBalancerName = $"{appName}-alb-{stageName}",
                Vpc = vpc,
                VpcSubnets = new SubnetSelection
                {
                    SubnetType = SubnetType.PUBLIC
                },
                InternetFacing = true,
                SecurityGroup = albsgEgress
            });

            //Fetch Certificate ARN of R53 domain from SSM parameter store
            string cert_arn = StringParameter.FromStringParameterAttributes(this, "certarn", new StringParameterAttributes
            {
                ParameterName = $"/{serviceName}/{appName}/{stageName}/cert"
            }).StringValue;

            var listener = alb.AddListener("alblistener", new BaseApplicationListenerProps
            {
                Port = 443,
                DefaultAction = ListenerAction.FixedResponse(404, new FixedResponseOptions
                { MessageBody = "Sorry! No services found" }),
                Certificates = new IListenerCertificate[] { ListenerCertificate.FromArn(cert_arn) }
            });

            //Get the list of Security Groups automatically attached to ALB
            string[] albsg = alb.LoadBalancerSecurityGroups;

            //Import R53 domain
            string domain = StringParameter.FromStringParameterAttributes(this, "domain", new StringParameterAttributes
            {
                ParameterName = $"/{serviceName}/{appName}/{stageName}/domain"
            }).StringValue;

            string zoneid = StringParameter.FromStringParameterAttributes(this, "zoneid", new StringParameterAttributes
            {
                ParameterName = $"/{serviceName}/{appName}/{stageName}/zoneid"
            }).StringValue;

            // Import R53 hosted zone
            var zone = HostedZone.FromHostedZoneAttributes(this, "hostedzone", new HostedZoneAttributes
            {
                ZoneName = domain,
                HostedZoneId = zoneid
            });

            //Create a sample R53 alias record for requests with no tenant
            var record = new
                ARecord(this, "record", new ARecordProps
                {
                    Zone = zone,
                    RecordName = $"test-{props.StageName}",
                    Target = RecordTarget.FromAlias(new LoadBalancerTarget(alb))
                    //If an alias record points to an AWS resource, you can't set the time to live (TTL); Route 53 uses the default TTL for the resource.
                });


            new CfnOutput(this, "albArn", new CfnOutputProps
            {
                Value = alb.LoadBalancerArn,
                Description = "ALB ARN",
                ExportName = $"ALBARN{props.StageName}"
            });

            new CfnOutput(this, "albName", new CfnOutputProps
            {
                Value = alb.LoadBalancerDnsName,
                Description = "ALB Name",
                ExportName = $"ALBNAME{props.StageName}"
            });

            new CfnOutput(this, "albSg", new CfnOutputProps
            {
                Value = Fn.Select(0, albsg),
                Description = "ALB Security Group",
                ExportName = $"ALBSG{props.StageName}"
            });

            new CfnOutput(this, "albCanonicalId", new CfnOutputProps
            {
                Value = alb.LoadBalancerCanonicalHostedZoneId,
                Description = "ALB Canonical Hosted Zone Id",
                ExportName = $"ALBZONEID{props.StageName}"
            });

            new CfnOutput(this, "alblistenerArn", new CfnOutputProps
            {
                Value = listener.ListenerArn,
                Description = "ALB Listener ARN",
                ExportName = $"ALBLISTENER{props.StageName}"
            });
        }
    }
}
