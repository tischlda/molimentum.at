using System.Collections.Generic;
using System.Linq;
using Molimentum.Models;
using Raven.Client;
using Raven.Client.Linq;

namespace Molimentum.Infrastructure
{
    public static class LinqExtensions
    {
        public static PagedResult<TResult> AsPagedResult<TResult>(this IQueryable<TResult> source, int? page, int pageSize)
        {
            if (page == null || page <= 0) page = 1;

            Pager pager;
            IEnumerable<TResult> items = null;

            var pagedSource = source.Skip(pageSize * (page.Value - 1)).Take(pageSize);


            if (pagedSource is IRavenQueryable<TResult>)
            {
                RavenQueryStatistics statistics;

                items = ((IRavenQueryable<TResult>)pagedSource)
                    .Statistics(out statistics)
                    .ToList();

                pager = new Pager(page.Value, pageSize, statistics.TotalResults);
            }
            else
            {
                var count = source.Count();
                pager = new Pager(page.Value, pageSize, count);
            }

            if (pager.Pages == 0)
            {
                pager = new Pager(0, pageSize, 0);
            }

            else if (page > pager.Pages)
            {
                return AsPagedResult(source, pager.Pages, pageSize);
            }

            if (items == null)
            {
                items = pagedSource;
            }

            return new PagedResult<TResult>(pager, items);
        }
    }
}
