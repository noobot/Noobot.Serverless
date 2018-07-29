using System;
using System.Threading.Tasks;
using Noobot.Serverless.MessagingPipeline;
using Noobot.Serverless.MessagingPipeline.Request;
using SlackConnector;
using SlackConnector.Models;

namespace Noobot.Serverless
{
    public class ChatPackage
    {
        private readonly ISlackConnection _slackConnection;
        private readonly IncomingMessage _message;
        private readonly Pipeline _pipeline;

        public ChatPackage(
            ISlackConnection slackConnection,
            IncomingMessage message,
            Pipeline pipeline)
        {
            _slackConnection = slackConnection;
            _message = message;
            _pipeline = pipeline;
        }

        public Task<Summary> Execute()
        {
            throw new NotImplementedException();
        }
    }
}