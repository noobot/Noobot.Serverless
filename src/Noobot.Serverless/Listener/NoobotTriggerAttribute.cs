using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.WebJobs.Description;

namespace Noobot.Serverless.Listener
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NoobotTriggerAttribute : Attribute
    {
        [Required]
        [AppSetting]
        public string SlackAccessToken { get; set; }
    }
}