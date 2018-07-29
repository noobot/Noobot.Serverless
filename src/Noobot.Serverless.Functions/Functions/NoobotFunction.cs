using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Noobot.Serverless.Listener;
using SlackConnector.Models;

namespace Noobot.Serverless.Functions.Functions
{
    public static class NoobotFunction
    {
        [FunctionName("noobot-event")]
        public static async Task Run(
            [NoobotTrigger]NoobotEvent noobotEvent,
            TraceWriter log)
        {
            await noobotEvent.SlackConnection.Say(new BotMessage
            {
                Text = noobotEvent.Message.Text,
                ChatHub = noobotEvent.Message.ChatHub
            });

            log.Info($"{noobotEvent.Message.Text} : {noobotEvent.Message.Timestamp}");
        }
    }
}