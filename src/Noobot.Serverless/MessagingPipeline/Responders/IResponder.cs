using System.Collections.Generic;

namespace Noobot.Serverless.MessagingPipeline.Responders
{
    public interface IResponder
    {
        IEnumerable<HandlerMapping> Handlers { get; }
    }
}