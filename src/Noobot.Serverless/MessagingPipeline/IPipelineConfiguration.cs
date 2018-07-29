using Noobot.Serverless.MessagingPipeline;
using Noobot.Serverless.MessagingPipeline.Response;

namespace Noobot.Serverless
{
    public interface IPipelineConfiguration
    {
        Pipeline GetPipeline(INoobotResponder noobotResponder);
    }
}