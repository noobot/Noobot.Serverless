namespace Noobot.Serverless.MessagingPipeline.Response
{
    public interface IResponseMessage
    {
        public string Channel { get; }
        public string UserId { get; }
        public ResponseType ResponseType { get; }
    }
}