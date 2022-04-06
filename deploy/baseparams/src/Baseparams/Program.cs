using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Baseparams
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

            var init = new BaseparamsStack(app, $"BaseparamsStack-{ENV}-{STAGE}", new AStackProps
            {
                Env = setEnv(),
                EnvironmentName = ENV.ToString(),
                StageName = STAGE.ToString()

            });

            //Add tags to taggable resources
            Tags.Of(init).Add("Environment", $"{ENV}");
            Tags.Of(init).Add("StageName", $"{STAGE}");
            Tags.Of(init).Add("CreatedBy", "AWSCDK");
            Tags.Of(init).Add("Application", "SampleApp");

            app.Synth();
        }
    }
}
