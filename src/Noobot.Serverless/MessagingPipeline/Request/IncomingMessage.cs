using System.Collections.Generic;
using Noobot.Serverless.MessagingPipeline.Response;

namespace Noobot.Serverless.MessagingPipeline.Request
{
    public class IncomingMessage
    {
        /// <summary>
        /// The Slack UserId of whoever sent the message
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Username of whoever sent the mssage
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// The channel used to send a DirectMessage back to the user who sent the message. 
        /// Note: this might be empty if the Bot hasn't talked to them privately before, but Noobot will join the DM automatically if required.
        /// </summary>
        public string UserChannel { get; }

        /// <summary>
        /// The email of the user that sent the message
        /// </summary>
        public string UserEmail { get; }

        /// <summary>
        /// Contains the untainted raw Text that comes in from Slack. This hasn't been URL decoded.
        /// </summary>
        public string RawText { get; }

        /// <summary>
        /// Contains the URL decoded text from the message.
        /// </summary>
        public string FullText { get; }

        /// <summary>
        /// Contains the text minus any Bot targeting text (e.g. "@Noobot: {blah}" turns into "{blah}")
        /// </summary>
        public string TargetedText { get; }

        /// <summary>
        /// The 'channel' the message occured on. This might be a DirectMessage channel.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// The type of channel the message arrived on
        /// </summary>
        public ResponseType ChannelType { get; }

        /// <summary>
        /// Detects if the bot's name is mentioned anywhere in the text
        /// </summary>
        public bool BotIsMentioned { get; }

        /// <summary>
        /// The Bot's Slack name - this is configurable in Slack
        /// </summary>
        public string BotName { get; }

        /// <summary>
        /// The Bot's UserId
        /// </summary>
        public string BotId { get; }

        public IncomingMessage(
            string userId,
            string username,
            string userChannel,
            string userEmail,
            string rawText,
            string fullText,
            string targetedText,
            string channel,
            ResponseType channelType,
            bool botIsMentioned,
            string botName,
            string botId)
        {
            UserId = userId;
            Username = username;
            UserChannel = userChannel;
            UserEmail = userEmail;
            RawText = rawText;
            FullText = fullText;
            TargetedText = targetedText;
            Channel = channel;
            ChannelType = channelType;
            BotIsMentioned = botIsMentioned;
            BotName = botName;
            BotId = botId;
        }


        /// <summary>
        /// Will generate a message to be sent the current channel the message arrived from
        /// </summary>
        public ResponseMessage ReplyToChannel(string text, Attachment attachment = null)
        {
            if (attachment == null)
                return ResponseMessage.ChannelMessage(Channel, text, attachments: null);

            var attachments = new List<Attachment> { attachment };
            return ReplyToChannel(text, attachments);
        }

        /// <summary>
        /// Will generate a message to be sent the current channel the message arrived from
        /// </summary>
        public ResponseMessage ReplyToChannel(string text, List<Attachment> attachments)
        {
            return ResponseMessage.ChannelMessage(Channel, text, attachments);
        }

        /// <summary>
        /// Will send a DirectMessage reply to the use who sent the message
        /// </summary>
        public ResponseMessage ReplyDirectlyToUser(string text)
        {
            return ResponseMessage.DirectUserMessage(UserId, text);
        }
    }
}