using System;
using Microsoft.Azure.WebJobs.Description;

namespace Noobot.Serverless.Listener
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class NoobotTriggerAttribute : Attribute
    {
    }
}