﻿using System;
using System.Linq;

namespace Noobot.Serverless.MessagingPipeline.Responders.ValidHandles
{
    public class StartsWithHandle : IValidHandle
    {
        private readonly string _messageStartsWith;

        public StartsWithHandle(string messageStartsWith)
        {
            _messageStartsWith = messageStartsWith ?? string.Empty;
        }

        public bool IsMatch(string message)
        {
            return (message ?? string.Empty).StartsWith(_messageStartsWith, StringComparison.OrdinalIgnoreCase);
        }

        public string HandleHelpText => _messageStartsWith;

        public static IValidHandle[] For(params string[] messagesStartsWith)
        {
            return messagesStartsWith
                .Select(x => new StartsWithHandle(x))
                .Cast<IValidHandle>()
                .ToArray();
        }
    }
}