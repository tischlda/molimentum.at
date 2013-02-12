using System;
using Raven.Client;
using Raven.Client.Document;

namespace Molimentum.Infrastructure
{
    public static class RavenDocumentStoreFactory
    {
        public static IDocumentStore CreateDocumentStore()
        {
            var documentStore = new DocumentStore { ConnectionStringName = "RavenDB" };
            
            documentStore.Conventions.DocumentKeyGenerator = (c, o, s) => Guid.NewGuid().ToString();

            documentStore.Initialize();

            return documentStore;
        }
    }
}