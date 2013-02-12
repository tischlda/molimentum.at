using System.Collections.Generic;
using Raven.Client;

namespace Molimentum.Areas.Api.Models.Mails
{
    public interface IProcessor
    {
        void Process(IDocumentSession session, IDictionary<string, string> data);
    }
}