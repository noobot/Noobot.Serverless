namespace Noobot.Serverless.MessagingPipeline.Response
{
    public class AttachmentField
    {
        public string Title { get; }
        public string Value { get; }
        public bool IsShort { get; }

        public AttachmentField(
            string title,
            string value,
            bool isShort)
        {
            Title = title;
            Value = value;
            IsShort = isShort;
        }
    }
}