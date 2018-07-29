using SlackConnector;
using SlackConnector.Models;

namespace Noobot.Serverless.Listener
{
    public class NoobotEvent
    {
        public SlackMessage Message { get; }
        public ISlackConnection SlackConnection { get; }

        public NoobotEvent(
            SlackMessage message,
            ISlackConnection slackConnection)
        {
            Message = message;
            SlackConnection = slackConnection;
        }
    }
}