using System.Collections.Generic;
using Noobot.Serverless.MessagingPipeline.Request;
using Noobot.Serverless.MessagingPipeline.Responders;
using Noobot.Serverless.MessagingPipeline.Response;

namespace Noobot.Serverless.MessagingPipeline
{
    public interface IResponder
    {
        ResponderSummary Invoke(IncomingMessage message);
        IEnumerable<CommandDescription> GetSupportedCommands();
    }
}