using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Noobot.Serverless.Functions;
using Noobot.Serverless.Listener;

[assembly: WebJobsStartup(typeof(Startup))]

namespace Noobot.Serverless.Functions
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<NoobotExtensionConfig>();
        }
    }
}