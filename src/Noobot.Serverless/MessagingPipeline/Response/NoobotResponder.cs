using System;
using System.Threading.Tasks;
using Noobot.Serverless.MessagingPipeline.Request;
using Noobot.Serverless.MessagingPipeline.Response;
using SlackConnector;

namespace Noobot.Serverless.Listener
{
    public class NoobotResponder : INoobotResponder
    {
        private readonly ISlackConnection _slackConnection;

        public NoobotResponder(ISlackConnection slackConnection)
        {
            _slackConnection = slackConnection;
        }

        public Task SendResponse(ResponseMessage responseMessage)
        {
            throw new NotImplementedException();
        }

        public Task IndicateTypingOnChannel(IncomingMessage incomingMessage)
        {
            throw new NotImplementedException();
        }
    }
}