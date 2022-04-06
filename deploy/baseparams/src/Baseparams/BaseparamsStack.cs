using Amazon.CDK;
using Constructs;
using Amazon.CDK.AWS.SSM;

namespace Baseparams
{
    public class BaseparamsStack : Stack
    {
        internal BaseparamsStack(Construct scope, string id, IAStackProps props = null) : base(scope, id, props)
        {
            string stageName = props.StageName;
            string appName = "app";
            string serviceName = "ecs";

            Item items = JsonFileReader.LoadJson();

            var ssm_vpc = _ = new StringParameter(this, "ssmvpc", new StringParameterProps
            {
                Description = "VPC",
                ParameterName = $"/{serviceName}/{appName}/{stageName}/vpc",
                StringValue = items.vpc
            });

            var ssm_publicsubnets = _ = new StringParameter(this, "ssmpublicsubnets", new StringParameterProps
            {
                Description = "Public subnets",
                ParameterName = $"/{serviceName}/{appName}/{stageName}/publicsubnets",
                StringValue = items.publicsubnets
            });

            var ssm_publicsubnetroutetableids = _ = new StringParameter(this, "ssmpublicsubnetroutetableids", new StringParameterProps
            {
                Description = "Public subnet route table ids",
                ParameterName = $"/{serviceName}/{appName}/{stageName}/publicsubnetroutetableids",
                StringValue = string.Join(",", items.publicsubnetroutetableids)
            });

            var ssm_privatesubnets = _ = new StringParameter(this, "ssmprivatesubnets", new StringParameterProps
            {
                Description = "Private subnets",
                ParameterName = $"/{serviceName}/{appName}/{stageName}/privatesubnets",
                StringValue = items.privatesubnets
            });

            var ssm_privatesubnetroutetableids = _ = new StringParameter(this, "ssmprivatesubnetroutetableids", new StringParameterProps
            {
                Description = "Private subnet route table ids",
                ParameterName = $"/{serviceName}/{appName}/{stageName}/privatesubnetroutetableids",
                StringValue = string.Join(",", items.privatesubnetroutetableids)
            });

            var ssm_azs = _ = new StringParameter(this, "ssmazs", new StringParameterProps
            {
                Description = "AZs",
                ParameterName = $"/{serviceName}/{appName}/{stageName}/azs",
                StringValue = items.azs
            });

            var ssm_cert = _ = new StringParameter(this, "ssmcert", new StringParameterProps
            {
                Description = "ACM certificate for Route53 domain",
                ParameterName = $"/{serviceName}/{appName}/{stageName}/cert",
                StringValue = items.cert
            });

            var ssm_domain = _ = new StringParameter(this, "ssmdomain", new StringParameterProps
            {
                Description = "Public Route53 domain",
                ParameterName = $"/{serviceName}/{appName}/{stageName}/domain",
                StringValue = items.domain
            });

            var ssm_zoneid = _ = new StringParameter(this, "ssmzoneid", new StringParameterProps
            {
                Description = "Public Route53 hosted zone ID",
                ParameterName = $"/{serviceName}/{appName}/{stageName}/zoneid",
                StringValue = items.zoneid
            });

            var ssm_appname = _ = new StringParameter(this, "ssmappname", new StringParameterProps
            {
                Description = "X-Ray Application Name",
                ParameterName = $"/{serviceName}/{appName}/appname",
                StringValue = items.appname
            });
        }
    }
}
