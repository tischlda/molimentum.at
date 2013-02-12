using Molimentum.Areas.Admin.Models.Synchronization;
using Raven.Client;
using StructureMap;

namespace Molimentum.Infrastructure
{
    public static class StructureMapBootStrapper
    {
        public static void Execute()
        {
            ObjectFactory.Initialize(x =>
            {
                x.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                });

                // register RavenDB document store
                x.ForSingletonOf<IDocumentStore>().Use(() =>
                {
                    return RavenDocumentStoreFactory.CreateDocumentStore();
                });

                // register RavenDB document session
                x.For<IDocumentSession>().HybridHttpOrThreadLocalScoped().Use(
                    context => context.GetInstance<IDocumentStore>().OpenSession());

                x.For<System.Web.Mvc.IDependencyResolver>().Add<MvcStructureMapDependencyResolver>();
                
                x.For<IPicasaImport>().Add<PicasaImport>();
            });
        }
    }
}