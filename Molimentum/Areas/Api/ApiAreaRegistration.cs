using System.Web.Mvc;

namespace Molimentum.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Mail",
                "Api/Mail/{action}",
                new { controller = "Mail", action = "Receive" });

            context.MapRoute(
                "Export",
                "Api/Export/{action}",
                new { controller = "Export" });

            context.MapRoute(
                "Export by id",
                "Api/Export/Items/{id}",
                new { controller = "Export", action = "Item" });

            context.MapRoute(
                "Export Post by Slug",
                "Api/Export/Posts/{slug}",
                new { controller = "Export", action = "PostBySlug" });

            context.MapRoute(
                "Export Album by Slug",
                "Api/Export/Albums/{slug}",
                new { controller = "Export", action = "AlbumBySlug" });
        }
    }
}
