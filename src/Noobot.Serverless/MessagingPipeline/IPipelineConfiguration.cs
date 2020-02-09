using Noobot.Serverless.MessagingPipeline.Response;

namespace Noobot.Serverless.MessagingPipeline
{
    public interface IPipelineConfiguration
    {
        Pipeline GetPipeline(INoobotResponder noobotResponder);
    }
}