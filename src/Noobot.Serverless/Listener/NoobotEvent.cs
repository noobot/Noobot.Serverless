using System;
using System.Linq;
using System.Threading.Tasks;
using Noobot.Serverless.MessagingPipeline.Request;
using Noobot.Serverless.MessagingPipeline.Response;
using SlackConnector;
using SlackConnector.Models;

namespace Noobot.Serverless.Listener
{
    public class NoobotEvent
    {
        private readonly SlackMessage _message;
        public ISlackConnection SlackConnection { get; }

        public NoobotEvent(
            SlackMessage message,
            ISlackConnection slackConnection)
        {
            _message = message ?? throw new ArgumentNullException(nameof(message));
            SlackConnection = slackConnection ?? throw new ArgumentNullException(nameof(slackConnection));
        }

        public async Task<IncomingMessage> Message()
        {
            return new IncomingMessage(
                _message.User.Id,
                GetUsername(_message),
                await GetUserChannel(_message),
                _message.User.Email,
                _message.Text,
                _message.Text,
                GetTargetedText(_message),
                _message.ChatHub.Id,
                _message.ChatHub.Type == SlackChatHubType.DM ? ResponseType.DirectMessage : ResponseType.Channel,
                _message.MentionsBot,
                SlackConnection.Self.Name,
                SlackConnection.Self.Id
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