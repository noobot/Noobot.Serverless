namespace Noobot.Serverless.MessagingPipeline.Responders.ValidHandles
{
    public interface IValidHandle
    {
        bool IsMatch(string message);
        string HandleHelpText { get; }
    }
}