using System.Collections.Generic;

namespace Noobot.Serverless.MessagingPipeline.Response
{
    public class ResponseMessage
    {
        public string Text { get; }
        public string Channel { get; }
        public string UserId { get; }
        public ResponseType ResponseType { get; }
        public IEnumerable<Attachment> Attachments { get; }

        public ResponseMessage(
            string text,
            string channel,
            string userId,
            ResponseType responseType,
            IEnumerable<Attachment> attachments = null)
        {
            Text = text;
            Channel = channel;
            UserId = userId;
            ResponseType = responseType;
            Attachments = attachments;
        }

        public ResponseMessage(
            string text,
            string channel,
            ResponseType responseType,
            IEnumerable<Attachment> attachments = null)
        {
            Text = text;
            Channel = channel;
            ResponseType = responseType;
            Attachments = attachments;
        }

        public static ResponseMessage DirectUserMessage(
            string userId,
            string text)
        {
            return new ResponseMessage(
                text,
                "",
                userId,
                ResponseType.DirectMessage);
        }

        public static ResponseMessage ChannelMessage(string channel, string text, Attachment attachment)
        {
            List<Attachment> attachments = null;
            if (attachment != null)
            {
                attachments = new List<Attachment> { attachment };
            }

            return ChannelMessage(channel, text, attachments);
        }

        public static ResponseMessage ChannelMessage(string channel, string text, List<Attachment> attachments)
        {
            return new ResponseMessage(
                text,
                channel,
                ResponseType.Channel,
                attachments);
        }
    }
}