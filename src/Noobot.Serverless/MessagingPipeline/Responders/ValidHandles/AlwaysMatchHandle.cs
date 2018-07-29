namespace Noobot.Serverless.MessagingPipeline.Responders.ValidHandles
{
    public class AlwaysMatchHandle : IValidHandle
    {
        public bool IsMatch(string message)
        {
            return true;
        }

        public string HandleHelpText => string.Empty;
    }
}