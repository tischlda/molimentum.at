using Molimentum.Models;

namespace Molimentum.ViewModels
{
    public class TagIndexViewModel
    {
        public string Tag { get; set; }

        public PagedResult<Post> Posts { get; set; }
    }
}