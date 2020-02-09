namespace Noobot.Serverless.MessagingPipeline.Response
{
    public class TypingIndicatorMessage : IResponseMessage
    {
        public string Channel { get; }
        public string UserId { get; }
        public ResponseType ResponseType { get; }

        public TypingIndicatorMessage(string channel, string userId, ResponseType responseType)
        {
            Channel = channel;
            UserId = userId;
            ResponseType = responseType;
        }
    }
}