using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using SlackConnector;
using SlackConnector.Models;

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
            var accessToken = AmbientConnectionStringProvider.Instance.GetConnectionString(_attribute.AccessToken);
            _connection = await connector.Connect(accessToken);
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