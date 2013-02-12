using Molimentum.Models;

namespace Molimentum.ViewModels
{
    public class CategoryDetailViewModel
    {
        public Category Category { get; set; }

        public PagedResult<Post> Posts { get; set; }
    }
}