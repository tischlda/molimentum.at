using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Molimentum
{
    public static class HtmlHelperExtensions
    {
        public static bool IsActiveSection(this HtmlHelper html, string sectionName)
        {
            return sectionName.Equals(html.ViewBag.Section, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string ClassForSectionLink(this HtmlHelper html, string sectionName)
        {
            return (html.IsActiveSection(sectionName)) ? "activeSection" : "inactiveSection";
        }

        public static void AddClientScript(this HtmlHelper html, IHtmlString script)
        {
            var context = GetTopActionViewContext(html.ViewContext);

            if (!context.ViewData.ContainsKey("ClientScripts")) context.ViewData["ClientScripts"] = new StringBuilder();

            ((StringBuilder)context.ViewData["ClientScripts"]).Append(script);
        }

        public static void RenderClientScripts(this HtmlHelper html)
        {
            var context = GetTopActionViewContext(html.ViewContext);

            if (context.ViewData.ContainsKey("ClientScripts"))
                html.ViewContext.Writer.Write(context.ViewData["ClientScripts"].ToString());
        }

        private static ViewContext GetTopActionViewContext(ViewContext viewContext)
        {
            if (viewContext.ParentActionViewContext == null) return viewContext;

            return GetTopActionViewContext(viewContext.ParentActionViewContext);
        }
    }
}