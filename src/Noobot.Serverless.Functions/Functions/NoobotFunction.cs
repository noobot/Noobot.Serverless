using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Noobot.Serverless.Listener;
using Noobot.Serverless.MessagingPipeline;

namespace Noobot.Serverless.Functions.Functions
{
    public class NoobotFunction
    {
        private readonly INoobotPipeline _noobotPipeline;

        public NoobotFunction(INoobotPipeline noobotPipeline)
        {
            _noobotPipeline = noobotPipeline;
        }

        [FunctionName("noobot-event")]
        public async Task HandleNoobotEvent(
            [NoobotTrigger] NoobotEvent noobotEvent,
            ILogger log)
        {
            await _noobotPipeline.Execute(noobotEvent, log);
        }
    }
}