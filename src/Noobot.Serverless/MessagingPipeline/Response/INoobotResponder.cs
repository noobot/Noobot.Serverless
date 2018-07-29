using System.Threading.Tasks;
using Noobot.Serverless.MessagingPipeline.Request;

namespace Noobot.Serverless.MessagingPipeline.Response
{
    public interface INoobotResponder
    {
        Task SendResponse(ResponseMessage responseMessage);
        Task IndicateTypingOnChannel(IncomingMessage incomingMessage);
    }
}