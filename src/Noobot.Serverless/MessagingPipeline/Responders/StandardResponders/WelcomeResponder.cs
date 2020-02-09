using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Noobot.Serverless.MessagingPipeline.Request;
using Noobot.Serverless.MessagingPipeline.Responders.ValidHandles;
using Noobot.Serverless.MessagingPipeline.Response;

namespace Noobot.Serverless.MessagingPipeline.Responders.StandardResponders
{
    public class WelcomeResponder : IResponder
    {
        public IEnumerable<HandlerMapping> Handlers { get; }

        public WelcomeResponder()
        {
            Handlers = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = ExactMatchHandle.For("hello", "hi"),
                    Description = "",
                    EvaluatorFunc = HandleMessage
                }
            };
        }

        private static async IAsyncEnumerable<IResponseMessage> HandleMessage(IncomingMessage message, IValidHandle matchedHandle)
        {
            yield return message.ReplyToChannel($"Hey @{message.Username}, how you doing?");

            await Task.Delay(TimeSpan.FromSeconds(1));

            yield return message.IndicateTypingOnDirectMessage();

            await Task.Delay(TimeSpan.FromSeconds(5));

            yield return message.ReplyDirectlyToUser("I know where you live...");
        }
    }
}