using System.Collections.Generic;

namespace Molimentum.Models
{
    public class PagedResult<TResult>
    {
        public PagedResult()
        {
        }

        public PagedResult(Pager pager, IEnumerable<TResult> items)
        {
            Pager = pager;
            Items = items;
        }

        public Pager Pager { get; set; }

        public IEnumerable<TResult> Items { get; set; }
    }
}
