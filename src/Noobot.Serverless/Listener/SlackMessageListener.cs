using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using SlackConnector;
using SlackConnector.Models;
using System.Configuration;

namespace Noobot.Serverless.Listener
{
    public class SlackMessageListener : IListener
    {
        private ISlackConnection _connection;
        private readonly SlackMessageTriggerAttribute _attribute;
        public ITriggeredFunctionExecutor Executor { get; }

        public SlackMessageListener(ITriggeredFunctionExecutor executor, SlackMessageTriggerAttribute attribute)
        {
            Executor = executor;
            _attribute = attribute;
        }

        public void Cancel() { }

        public void Dispose() { }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var connector = new SlackConnector.SlackConnector();
            var slackAccessToken = AmbientConnectionStringProvider.Instance.GetConnectionString("SlackAccessToken");

            _connection = await connector.Connect(slackAccessToken);
            _connection.OnMessageReceived += ConnectionOnOnMessageReceived;
            _connection.OnDisconnect += ConnectionOnOnDisconnect;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.OnMessageReceived -= ConnectionOnOnMessageReceived;
            _connection.OnDisconnect -= ConnectionOnOnDisconnect;
            await _connection.Close();
        }

        private void ConnectionOnOnDisconnect()
        {
            throw new Exception("Slack Disconnect");
        }

        private async Task ConnectionOnOnMessageReceived(SlackMessage message)
        {
            var triggerData = new TriggeredFunctionData
            {
                TriggerValue = message
            };

            await Executor.TryExecuteAsync(triggerData, CancellationToken.None);
        }
    }
}