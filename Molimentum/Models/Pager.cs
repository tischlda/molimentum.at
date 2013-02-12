using System;

namespace Molimentum.Models
{
    public class Pager
    {
        public Pager(int page, int pageSize, int itemCount)
        {
            Pages = (int)Math.Ceiling((double)itemCount / (double)pageSize);
            Page = page;
            PageSize = pageSize;
        }

        public int Page { get; private set; }

        public int Pages { get; private set; }

        public int PageSize { get; private set; }
    }
}