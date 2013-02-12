using System;
using System.Collections.Generic;
using System.Globalization;
using Raven.Client;

namespace Molimentum.Areas.Api.Models.Mails
{
    public abstract class ProcessorBase<T> : IProcessor
    {
        public void Process(IDocumentSession session, IDictionary<string, string> data)
        {
            if (data.ContainsKey("COMMAND") && data["COMMAND"].Equals("DELETE", StringComparison.InvariantCultureIgnoreCase))
            {
                Delete(session, data);
            }
            else
            {
                Save(session, data);
            }
        }

        protected abstract void Save(IDocumentSession session, IDictionary<string, string> data);

        protected virtual void Delete(IDocumentSession session, IDictionary<string, string> data)
        {
            var item = session.Load<T>(data["ID"]);

            session.Delete(item);

            session.SaveChanges();
        }

        protected DateTimeOffset ParseDateTime(string dateTimeString)
        {
            return DateTimeOffset.Parse(dateTimeString, new CultureInfo("DE-AT", false).DateTimeFormat, DateTimeStyles.AssumeUniversal);
        }
    }
}
