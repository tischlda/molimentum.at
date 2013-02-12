using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Molimentum.Infrastructure;

namespace Molimentum
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            StructureMapBootStrapper.Execute();
            MvcBootStrapper.Execute();
            RavenBootstrapper.Execute();
        }
    }
}