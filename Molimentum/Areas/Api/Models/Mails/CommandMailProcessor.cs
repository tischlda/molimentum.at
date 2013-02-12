using System;
using System.Collections.Generic;
using Raven.Client;

namespace Molimentum.Areas.Api.Models.Mails
{
    public class CommandMailProcessor
    {
        private readonly IDictionary<string, IProcessor> _processors;

        public CommandMailProcessor()
        {
            _processors = new Dictionary<string, IProcessor>
            {
                { "POST", new PostProcessor() },
                { "POSITION REPORT", new PositionReportProcessor() },
                { "COMMENT", new CommentProcessor() }
            };
        }

        public void Process(IDocumentSession session, CommandMail commandMail)
        {
            if (!_processors.ContainsKey(commandMail.Subject))
                throw new ArgumentOutOfRangeException(
                    "commandMail.Subject",
                    commandMail.Subject,
                    "Unknown message type '" + commandMail.Subject + "'.");

            _processors[commandMail.Subject].Process(session, commandMail.Fields);
        }
    }
}
