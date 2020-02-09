﻿using System;
using System.Collections.Generic;
using Noobot.Serverless.MessagingPipeline.Request;
using Noobot.Serverless.MessagingPipeline.Responders.ValidHandles;
using Noobot.Serverless.MessagingPipeline.Response;

namespace Noobot.Serverless.MessagingPipeline.Responders
{
    public class HandlerMapping
    {
        /// <summary>
        /// Valid handles to match on for incoming text, e.g. match exactly on "Hello"
        /// </summary>
        public IValidHandle[] ValidHandles { get; set; } = new IValidHandle[0];

        /// <summary>
        /// Description of what this handle does. This appears in the "help" function.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The function that is evaluated on a matched handle
        /// </summary>
        public Func<IncomingMessage, IValidHandle, IAsyncEnumerable<IResponseMessage>> EvaluatorFunc { get; set; }

        /// <summary>
        /// Defaults to "False". If set to True then the pipeline isn't interrupted if a match occurs here.
        /// </summary>
        public bool ShouldContinueProcessing { get; set; }

        /// <summary>
        /// Defaults to "True". If set to false then any message is considered, even if it isn't targeted at the bot. e.g. @noobot or a private channel
        /// </summary>
        public bool MessageShouldTargetBot { get; set; } = true;

        /// <summary>
        /// Defaults to "True". Set to false to hide these commands in the help command.
        /// </summary>
        public bool VisibleInHelp { get; set; } = true;
    }
}