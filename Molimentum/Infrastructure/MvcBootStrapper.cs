using System.Web.Mvc;
using StructureMap;

namespace Molimentum.Infrastructure
{
    public static class MvcBootStrapper
    {
        public static void Execute()
        {
            DependencyResolver.SetResolver(ObjectFactory.GetInstance<IDependencyResolver>());
        }
    }
}