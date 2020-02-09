using System.Collections.Generic;
using Noobot.Serverless.MessagingPipeline.Request;

namespace Noobot.Serverless.MessagingPipeline.Responders
{
    public interface IResponder
    {
        ResponderSummary Invoke(IncomingMessage message);
        IEnumerable<CommandDescription> GetSupportedCommands();
    }
}