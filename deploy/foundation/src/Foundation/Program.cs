using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            Amazon.CDK.Environment setEnv(string account = null, string region = null)
            {
                return new Amazon.CDK.Environment
                {
                    Account = account ?? System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = region ?? System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION")
                };
            }

            //Fetch context variables from cdk deploy
            var ENV = app.Node.TryGetContext("Environment");
            var STAGE = app.Node.TryGetContext("Stage");

            var _init1 = new DeployParamsStack(app, $"DeployParamsStack-{ENV}-{STAGE}", new AStackProps
            {
                Env = setEnv(),
                EnvironmentName = ENV.ToString(),
                StageName = STAGE.ToString()

            });

            //Add tags to taggable resources
            Tags.Of(_init1).Add("Environment", $"{ENV}");
            Tags.Of(_init1).Add("StageName", $"{STAGE}");
            Tags.Of(_init1).Add("CreatedBy", "AWSCDK");
            Tags.Of(_init1).Add("Application", "SampleApp");

            var _init2 = new FoundationStack(app, $"FoundationStack-{ENV}-{STAGE}", new AStackProps
            {
                Env = setEnv(),
                EnvironmentName = ENV.ToString(),
                StageName = STAGE.ToString()

            });

            //Add tags to taggable resources
            Tags.Of(_init2).Add("Environment", $"{ENV}");
            Tags.Of(_init2).Add("StageName", $"{STAGE}");
            Tags.Of(_init2).Add("CreatedBy", "AWSCDK");
            Tags.Of(_init2).Add("Application", "SampleApp");

            app.Synth();
        }
    }
}
