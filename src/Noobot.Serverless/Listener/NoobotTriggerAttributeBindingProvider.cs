using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using SlackConnector.Models;

namespace Noobot.Serverless.Listener
{
    public class NoobotTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly NoobootExtensionConfig _extensionConfigProvider;

        public NoobotTriggerAttributeBindingProvider(NoobootExtensionConfig extensionConfigProvider)
        {
            _extensionConfigProvider = extensionConfigProvider;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var parameter = context.Parameter;
            var attribute = parameter.GetCustomAttribute<NoobotTriggerAttribute>(false);
            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            if (!IsSupportBindingType(parameter.ParameterType))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind NoobotTriggerAttribute to type '{0}'.", parameter.ParameterType));
            }

            var contextParameter = context.Parameter;
            var functionName = context.Parameter.Member.Name;
            var test = new NoobotTriggerBinding(
                contextParameter,
                _extensionConfigProvider,
                functionName);

            return Task.FromResult<ITriggerBinding>(test);
        }

        public bool IsSupportBindingType(Type t)
        {
            return t == typeof(NoobotEvent) || t == typeof(string);
        }
    }
}