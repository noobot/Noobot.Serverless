using System;
using Noobot.Serverless.MessagingPipeline.Response;

namespace Noobot.Serverless.MessagingPipeline
{
    public abstract class PipelineConfigurationBase : IPipelineConfiguration
    {
        public Pipeline GetPipeline(INoobotResponder noobotResponder)
        {
            throw new NotImplementedException();
        }
    }
}