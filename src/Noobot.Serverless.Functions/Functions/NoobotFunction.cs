using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SlackConnector.Models;
using Noobot.Serverless.Listener;

namespace Noobot.Serverless.Functions.Functions
{
    public static class NoobotFunction
    {
        [FunctionName("SlackTriggerFunction")]
        public static void Run(
            [SlackMessageTrigger]SlackMessage message,
            TraceWriter log)
        {
            log.Info($"{message.Text} : {message.Timestamp}");
        }
    }
}