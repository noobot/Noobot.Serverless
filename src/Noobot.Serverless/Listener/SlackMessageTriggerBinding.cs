using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackConnector.Models;

namespace Noobot.Serverless.Listener
{
    public class SlackMessageTriggerBinding : ITriggerBinding
    {
        private readonly Dictionary<string, Type> _bindingContract;
        private readonly string _functionName;
        private readonly ParameterInfo _parameter;
        private SlackMessageExtentionConfig _listenersStore;

        public SlackMessageTriggerBinding(
            ParameterInfo parameter,
            SlackMessageExtentionConfig listenersStore,
            string functionName)
        {
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
            if (!(value is SlackMessage))
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
            var attribute = GetResolvedAttribute<SlackMessageTriggerAttribute>(_parameter);
            return Task.FromResult<IListener>(new SlackMessageListener(context.Executor, attribute));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new SlackMessageTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "SlackMessage",
                    Description = "SlackMessage trigger fired",
                    DefaultValue = "Sample"
                }
            };
        }

        public Type TriggerValueType => typeof(SlackMessage);

        public IReadOnlyDictionary<string, Type> BindingDataContract => _bindingContract;

        internal static TAttribute GetResolvedAttribute<TAttribute>(ParameterInfo parameter)
            where TAttribute : Attribute
        {
            var attribute = parameter.GetCustomAttribute<TAttribute>(true);

            var attributeConnectionProvider = attribute as IConnectionProvider;
            if (attributeConnectionProvider != null && string.IsNullOrEmpty(attributeConnectionProvider.Connection))
            {
                var connectionProviderAttribute =
                    attribute.GetType().GetCustomAttribute<ConnectionProviderAttribute>();
                if (connectionProviderAttribute?.ProviderType != null)
                {
                    var connectionOverrideProvider =
                        GetHierarchicalAttributeOrNull(parameter, connectionProviderAttribute.ProviderType) as IConnectionProvider;
                    if (connectionOverrideProvider != null &&
                        !string.IsNullOrEmpty(connectionOverrideProvider.Connection))
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

        private class SlackMessageValueBinder : ValueBinder, IDisposable
        {
            private readonly object _value;
            private List<IDisposable> _disposables;

            public SlackMessageValueBinder(ParameterInfo parameter, object value,
                List<IDisposable> disposables = null)
                : base(parameter.ParameterType)
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

            public override Task<object> GetValueAsync()
            {
                return Task.FromResult(_value);
            }

            public override string ToInvokeString()
            {
                // TODO: Customize your Dashboard invoke string
                return $"{_value}";
            }
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