using System.Web.Mvc;
using FluentFilters;
using FluentFilters.Criteria;

namespace Molimentum
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            var fluentFilters = FluentFiltersBuider.Filters;
            FilterProviders.Providers.Add(fluentFilters);
            fluentFilters.Add<AuthorizeAttribute>(
                c => c.Require(new AreaFilterCriteria("Admin")));
        }
    }
}