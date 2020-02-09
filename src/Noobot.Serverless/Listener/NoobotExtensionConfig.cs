using System;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Configuration;

namespace Noobot.Serverless.Listener
{
    public class NoobotExtensionConfig : IExtensionConfigProvider
    {
        private readonly IConfiguration _configuration;

        public NoobotExtensionConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var triggerAttributeBindingRule = context.AddBindingRule<NoobotTriggerAttribute>();
            triggerAttributeBindingRule.BindToTrigger<NoobotEvent>(
                new NoobotTriggerAttributeBindingProvider(this, _configuration)
            );
        }
    }
}