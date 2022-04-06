using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation
{
    internal interface IAStackProps : IStackProps
    {
        public string EnvironmentName { get; set; }

        public string StageName { get; set; }

    }
}
