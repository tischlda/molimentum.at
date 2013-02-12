using System.Web.Mvc;
using System.Web.Routing;

namespace Molimentum
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Error",
                url: "Errors/{action}",
                defaults: new { controller = "Errors" }
            );

            routes.MapRoute(
                name: "Post Redirect",
                url: "Posts/Detail/{id}",
                defaults: new { controller = "Blog", action = "RedirectOld" }
            );

            routes.MapRoute(
                name: "Photos Redirect",
                url: "Albums/Detail/{id}",
                defaults: new { controller = "Photos", action = "RedirectOld" }
            );

            routes.MapRoute(
                name: "Guestbook Index",
                url: "guestbook",
                defaults: new { controller = "Guestbook", action = "Index" }
            );

            routes.MapRoute(
                name: "Blog Category",
                url: "blog/category/{slug}",
                defaults: new { controller = "Blog", action = "CategoryDetail" }
            );

            routes.MapRoute(
                name: "Blog Tag",
                url: "blog/tag/{tag}",
                defaults: new { controller = "Blog", action = "TagIndex" }
            );

            routes.MapRoute(
                name: "Blog Detail",
                url: "blog/{slug}",
                defaults: new { controller = "Blog", action = "Detail" }
            );

            routes.MapRoute(
                name: "Blog Index",
                url: "blog",
                defaults: new { controller = "Blog", action = "Index" }
            );

            routes.MapRoute(
                name: "Photos Detail",
                url: "photos/{slug}",
                defaults: new { controller = "Photos", action = "Detail" }
            );

            routes.MapRoute(
                name: "Photos Index",
                url: "photos",
                defaults: new { controller = "Photos", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}