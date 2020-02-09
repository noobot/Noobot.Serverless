using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Noobot.Serverless.Functions.Functions
{
    /// <summary>
    /// Keeps the Azure Function app loaded into memory
    /// </summary>
    public static class Heartbeat
    {
        [FunctionName("Heartbeat")]
        public static void HeartbeatFunction([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("Keeping the ap alive...");
        }
    }
}
