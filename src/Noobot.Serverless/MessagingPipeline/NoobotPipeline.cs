using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Noobot.Serverless.Listener;
using Noobot.Serverless.MessagingPipeline.Request;
using Noobot.Serverless.MessagingPipeline.Responders;
using Noobot.Serverless.MessagingPipeline.Response;
using SlackConnector;
using SlackConnector.Models;

namespace Noobot.Serverless.MessagingPipeline
{
    public class NoobotPipeline : INoobotPipeline
    {
        private readonly NoobotPipelineConfiguration _configuration;
        private readonly IResponder[] _responders;

        public NoobotPipeline(
            NoobotPipelineConfiguration configuration,
            params IResponder[] responders)
        {
            _configuration = configuration;
            _responders = responders;
        }

        public async Task<Summary> Execute(
            NoobotEvent noobotEvent,
            ILogger log)
        {
            var message = await noobotEvent.Message();
            log.LogInformation($"Processing message '{message.FullText}' from '{message.Username}' in '{message.Channel}'");

            foreach (var responder in _responders)
            {
                if (responder == null)
                {
                    continue;
                }

                await ProcessResponder(
                    responder,
                    message,
                    noobotEvent.SlackConnection, 
                    log);
            }

            return new Summary();
        }

        private async Task ProcessResponder(
            IResponder responder,
            IncomingMessage message,
            ISlackConnection slackConnection,
            ILogger log)
        {
            foreach (var handler in responder.Handlers)
            {
                var messageText = message.FullText;
                if (handler.MessageShouldTargetBot)
                {
                    messageText = message.TargetedText;
                }

                await ProcessHandles(
                    message,
                    handler,
                    messageText,
                    slackConnection,
                    log);
            }
        }

        private async Task ProcessHandles(
            IncomingMessage message,
            HandlerMapping handler,
            string messageText,
            ISlackConnection slackConnection,
            ILogger log)
        {
            foreach (var handle in handler.ValidHandles)
            {
                if (!handle.IsMatch(messageText))
                {
                    continue;
                }

                await foreach (var responseMessage in handler.EvaluatorFunc(message, handle))
                {
                    await SendMessage(
                        responseMessage,
                        slackConnection,
                        log);
                }
            }
        }

        private async Task SendMessage(
            IResponseMessage responseMessage,
            ISlackConnection slackConnection,
            ILogger log)
        {
            var chatHub = await GetChatHub(responseMessage, slackConnection);
            if (chatHub == null)
            {
                throw new ArgumentNullException(nameof(chatHub));
            }

            if (responseMessage is TypingIndicatorMessage)
            {
                await slackConnection.IndicateTyping(chatHub);
                return;
            }

            if (!(responseMessage is ResponseMessage message))
            {
                throw new Exception("Unknown message type");
            }

            var botMessage = new BotMessage
            {
                ChatHub = chatHub,
                Text = message.Text,
                Attachments = GetAttachments(message.Attachments)
            };

            var textTrimmed = botMessage.Text.Length > 50 
                ? botMessage.Text.Substring(0, 50) + "..."
                : botMessage.Text;

            log.LogInformation($"Sending message '{textTrimmed}'");

            await slackConnection.Say(botMessage);
        }

        private async Task<SlackChatHub> GetChatHub(
            IResponseMessage responseMessage,
            ISlackConnection slackConnection)
        {
            SlackChatHub chatHub = null;

            if (responseMessage.ResponseType == ResponseType.Channel)
            {
                chatHub = new SlackChatHub { Id = responseMessage.Channel };
            }
            else if (responseMessage.ResponseType == ResponseType.DirectMessage)
            {
                if (string.IsNullOrEmpty(responseMessage.Channel))
                {
                    chatHub = await GetUserChatHub(
                        responseMessage.UserId,
                        slackConnection);
                }
                else
                {
                    chatHub = new SlackChatHub { Id = responseMessage.Channel };
                }
            }

            return chatHub;
        }

        private async Task<SlackChatHub> GetUserChatHub(
            string userId,
            ISlackConnection slackConnection,
            bool joinChannel = true)
        {
            SlackChatHub chatHub = null;

            if (slackConnection.UserCache.ContainsKey(userId))
            {
                string username = "@" + slackConnection.UserCache[userId].Name;
                chatHub = slackConnection.ConnectedDMs().FirstOrDefault(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
            }

            if (chatHub == null && joinChannel)
            {
                chatHub = await slackConnection.JoinDirectMessageChannel(userId);
            }

            return chatHub;
        }

        private IList<SlackAttachment> GetAttachments(IEnumerable<Attachment> attachments)
        {
            var slackAttachments = new List<SlackAttachment>();

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    slackAttachments.Add(new SlackAttachment
                    {
                        Text = attachment.Text,
                        Title = attachment.Title,
                        Fallback = attachment.Fallback,
                        ImageUrl = attachment.ImageUrl,
                        ThumbUrl = attachment.ThumbUrl,
                        AuthorName = attachment.AuthorName,
                        ColorHex = attachment.Color,
                        Fields = GetAttachmentFields(attachment)
                    });
                }
            }

            return slackAttachments;
        }

        private IList<SlackAttachmentField> GetAttachmentFields(Attachment attachment)
        {
            var attachmentFields = new List<SlackAttachmentField>();

            if (attachment?.AttachmentFields != null)
            {
                foreach (var attachmentField in attachment.AttachmentFields)
                {
                    attachmentFields.Add(new SlackAttachmentField
                    {
                        Title = attachmentField.Title,
                        Value = attachmentField.Value,
                        IsShort = attachmentField.IsShort
                    });
                }
            }

            return attachmentFields;
        }
    }
}