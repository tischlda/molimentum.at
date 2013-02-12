using Molimentum.Models;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace Molimentum.Tests
{
    public static class RavenDocumentStoreFactory
    {
        public static IDocumentStore CreateInMemoryDocumentStore()
        {
            var documentStore = new EmbeddableDocumentStore
            {
                RunInMemory = true,
                Conventions = {IdentityPartsSeparator = "-"}
            };

            documentStore.Initialize();

            IndexCreation.CreateIndexes(typeof(Post).Assembly, documentStore);

            return documentStore;
        }
    }
}