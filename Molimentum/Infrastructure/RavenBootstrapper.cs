using System.Reflection;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Indexes;

namespace Molimentum.Infrastructure
{
    public static class RavenBootstrapper
    {
        public static void Execute()
        {
            var documentStore = DependencyResolver.Current.GetService<IDocumentStore>();
            IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), documentStore);
        }
    }
}