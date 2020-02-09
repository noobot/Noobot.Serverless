using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Noobot.Serverless.Listener;

namespace Noobot.Serverless.MessagingPipeline
{
    public interface INoobotPipeline
    {
        Task<Summary> Execute(NoobotEvent noobotEvent, ILogger log);
    }
}