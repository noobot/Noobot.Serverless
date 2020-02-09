using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Noobot.Serverless.Listener
{
    public class NoobotTriggerBinding : ITriggerBinding
    {
        private readonly Dictionary<string, Type> _bindingContract;
        private readonly string _functionName;
        private readonly IConfiguration _configuration;
        private readonly ParameterInfo _parameter;
        private NoobotExtensionConfig _listenersStore;

        public NoobotTriggerBinding(
            IConfiguration configuration,
            ParameterInfo parameter,
            NoobotExtensionConfig listenersStore,
            string functionName)
        {
            _configuration = configuration;
            _parameter = parameter;
            _listenersStore = listenersStore;
            _functionName = functionName;
            _bindingContract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                {"data", typeof(JObject)}
            };
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            if (!(value is NoobotEvent))
            {
                throw new Exception();
            }

            var bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                {"data", value}
            };

            var argument = _parameter.ParameterType == typeof(string) 
                ? JsonConvert.SerializeObject(value, Formatting.Indented) 
                : value;

            var valueBinder = new SlackMessageValueBinder(_parameter, argument);
            return Task.FromResult<ITriggerData>(new TriggerData(valueBinder, bindingData));
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            var attribute = GetResolvedAttribute<NoobotTriggerAttribute>(_parameter);
            return Task.FromResult<IListener>(new NoobotListener(context.Executor, attribute));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new SlackMessageTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "NoobotEvent",
                    Description = "NoobotEvent trigger fired",
                    DefaultValue = "Sample"
                }
            };
        }

        public Type TriggerValueType => typeof(NoobotEvent);

        public IReadOnlyDictionary<string, Type> BindingDataContract => _bindingContract;

        internal TAttribute GetResolvedAttribute<TAttribute>(ParameterInfo parameter)
            where TAttribute : Attribute
        {
            var attribute = parameter.GetCustomAttribute<TAttribute>(true);

            if (attribute is NoobotTriggerAttribute trigger)
            {
                if (string.IsNullOrEmpty(trigger.SlackAccessToken))
                {
                    trigger.SlackAccessToken = _configuration.GetConnectionStringOrSetting("SlackAccessToken");
                    return attribute;
                }
            }

            if (attribute is IConnectionProvider attributeConnectionProvider && string.IsNullOrEmpty(attributeConnectionProvider.Connection))
            {
                var connectionProviderAttribute =
                    attribute.GetType().GetCustomAttribute<ConnectionProviderAttribute>();

                if (connectionProviderAttribute?.ProviderType == null)
                {
                    return attribute;
                }

                if (GetHierarchicalAttributeOrNull(parameter, connectionProviderAttribute.ProviderType) is IConnectionProvider connectionOverrideProvider
                    && !string.IsNullOrEmpty(connectionOverrideProvider.Connection))
                {
                    attributeConnectionProvider.Connection = connectionOverrideProvider.Connection;
                }
            }

            return attribute;
        }
        
        internal static Attribute GetHierarchicalAttributeOrNull(ParameterInfo parameter, Type attributeType)
        {
            if (parameter == null)
            {
                return null;
            }

            var attribute = parameter.GetCustomAttribute(attributeType);
            if (attribute != null)
            {
                return attribute;
            }

            var method = parameter.Member as MethodInfo;
            return method == null 
                ? null 
                : GetHierarchicalAttributeOrNull(method, attributeType);
        }

        internal static Attribute GetHierarchicalAttributeOrNull(MethodInfo method, Type type)
        {
            var attribute = method.GetCustomAttribute(type);
            if (attribute != null)
            {
                return attribute;
            }

            attribute = method.DeclaringType.GetCustomAttribute(type);
            return attribute;
        }

        private class SlackMessageValueBinder : IValueProvider, IDisposable
        {
            private readonly object _value;
            private List<IDisposable> _disposables;

            public SlackMessageValueBinder(
                ParameterInfo parameter,
                object value,
                List<IDisposable> disposables = null)
            {
                _value = value;
                _disposables = disposables;
            }

            public void Dispose()
            {
                if (_disposables != null)
                {
                    foreach (var d in _disposables)
                    {
                        d.Dispose();
                    }
                    _disposables = null;
                }
            }

            public Task<object> GetValueAsync()
            {
                return Task.FromResult(_value);
            }

            public string ToInvokeString()
            {
                // TODO: Customize your Dashboard invoke string
                return $"{_value}";
            }

            public Type Type { get; }
        }

        private class SlackMessageTriggerParameterDescriptor : TriggerParameterDescriptor
        {
            public override string GetTriggerReason(IDictionary<string, string> arguments)
            {
                // TODO: Customize your Dashboard display string
                return $"SlackMessage trigger fired at {DateTime.Now:o}";
            }
        }
    }
}