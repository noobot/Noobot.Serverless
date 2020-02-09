using System;
using System.Linq;
using System.Threading.Tasks;
using Noobot.Serverless.MessagingPipeline;
using Noobot.Serverless.MessagingPipeline.Request;
using Noobot.Serverless.MessagingPipeline.Response;
using SlackConnector;
using SlackConnector.Models;

namespace Noobot.Serverless.Listener
{
    public class NoobotEvent
    {
        public SlackMessage Message { get; }
        public ISlackConnection SlackConnection { get; }

        public NoobotEvent(
            SlackMessage message,
            ISlackConnection slackConnection)
        {
            Message = message;
            SlackConnection = slackConnection;
        }

        public async Task<ChatPackage> UseConfiguration(Func<IPipelineConfiguration> configuration)
        {
            var responder = new NoobotResponder(SlackConnection);
            var pipeline = configuration().GetPipeline(responder);

            var incomingMessage = new IncomingMessage(
                Message.User.Id,
                GetUsername(Message),
                await GetUserChannel(Message),
                Message.User.Email,
                Message.Text,
                Message.Text,
                GetTargetedText(Message),
                Message.ChatHub.Id,
                Message.ChatHub.Type == SlackChatHubType.DM ? ResponseType.DirectMessage : ResponseType.Channel,
                Message.MentionsBot,
                SlackConnection.Self.Name,
                SlackConnection.Self.Id
            );

            return new ChatPackage(
                SlackConnection,
                incomingMessage,
                pipeline
            );
        }

        private string GetUsername(SlackMessage message)
        {
            return SlackConnection.UserCache.ContainsKey(message.User.Id)
                ? SlackConnection.UserCache[message.User.Id].Name
                : string.Empty;
        }

        private string GetTargetedText(SlackMessage slackMessage)
        {
            var messageText = slackMessage.Text ?? string.Empty;
            var isOnPrivateChannel = slackMessage.ChatHub.Type == SlackChatHubType.DM;

            var myNames = new[]
            {
                SlackConnection.Self.Name + ":",
                SlackConnection.Self.Name,
                $"<@{SlackConnection.Self.Id}>:",
                $"<@{SlackConnection.Self.Id}>",
                $"@{SlackConnection.Self.Name}:",
                $"@{SlackConnection.Self.Name}",
            };

            var handle = myNames.FirstOrDefault(x => messageText.StartsWith(x, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(handle) && !isOnPrivateChannel)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(handle) && isOnPrivateChannel)
            {
                return messageText;
            }

            return messageText.Substring(handle.Length).Trim();
        }

        private async Task<string> GetUserChannel(SlackMessage message)
        {
            return (await GetUserChatHub(message.User.Id, joinChannel: false) ?? new SlackChatHub()).Id;
        }

        private async Task<SlackChatHub> GetUserChatHub(string userId, bool joinChannel = true)
        {
            SlackChatHub chatHub = null;

            if (SlackConnection.UserCache.ContainsKey(userId))
            {
                var username = "@" + SlackConnection.UserCache[userId].Name;
                chatHub = SlackConnection.ConnectedDMs().FirstOrDefault(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
            }

            if (chatHub == null && joinChannel)
            {
                chatHub = await SlackConnection.JoinDirectMessageChannel(userId);
            }

            return chatHub;
        }
    }
}