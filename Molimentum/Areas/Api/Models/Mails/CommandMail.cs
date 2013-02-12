using System.Collections.Generic;
using System.IO;

namespace Molimentum.Areas.Api.Models.Mails
{
    public class CommandMail
    {
        protected CommandMail()
        {

        }

        private static readonly char[] _tokenSeparator = new[] { ':' };

        public static CommandMail Parse(Mail mail)
        {
            var commandMail = new CommandMail
            {
                Subject = mail.Subject,
                From = mail.From,
                Fields = new Dictionary<string, string>()
            };

            var stringReader = new StringReader(mail.Plain);
            
            string line;
            bool headerRead = false;
            
            while(!headerRead && null != (line = stringReader.ReadLine()))
            {
                var parts = line.Split(_tokenSeparator, 2);
                
                if(parts.Length == 2)
                    commandMail.Fields.Add(parts[0].Trim().ToUpper(), parts[1].Trim());
                else
                    headerRead = true;
            }

            commandMail.Body = stringReader.ReadToEnd();
            commandMail.Fields.Add("BODY", commandMail.Body);

            return commandMail;
        }

        public IDictionary<string, string> Fields { get; private set; }

        public string Subject { get; private set; }
        public string From { get; private set; }
        public string Body { get; private set; }
    }
}