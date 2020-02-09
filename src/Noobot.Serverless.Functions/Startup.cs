using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Noobot.Serverless.Functions;
using Noobot.Serverless.Listener;
using Noobot.Serverless.MessagingPipeline;
using Noobot.Serverless.MessagingPipeline.Responders.StandardResponders;

[assembly: WebJobsStartup(typeof(Startup))]

namespace Noobot.Serverless.Functions
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var services = builder.Services;

            builder.AddExtension<NoobotExtensionConfig>();

            services
                .AddScoped<INoobotPipeline>(provider => new NoobotPipeline(
                    new NoobotPipelineConfiguration(),
                    new WelcomeResponder()));
        }
    }
}