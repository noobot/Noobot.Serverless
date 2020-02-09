using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using SlackConnector;
using SlackConnector.Models;

namespace Noobot.Serverless.Listener
{
    public class NoobotListener : IListener
    {
        private ISlackConnection _connection;
        private readonly NoobotTriggerAttribute _attribute;
        public ITriggeredFunctionExecutor Executor { get; }

        public NoobotListener(
            ITriggeredFunctionExecutor executor,
            NoobotTriggerAttribute attribute)
        {
            Executor = executor;
            _attribute = attribute;
        }

        public void Cancel() { }

        public void Dispose() { }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var connector = new SlackConnector.SlackConnector();
            var slackAccessToken = _attribute.SlackAccessToken;

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

        }

        private async Task ConnectionOnOnMessageReceived(SlackMessage message)
        {
            var triggerData = new TriggeredFunctionData
            {
                TriggerValue = new NoobotEvent(message, _connection)
            };

            await Executor.TryExecuteAsync(triggerData, CancellationToken.None);
        }
    }
}