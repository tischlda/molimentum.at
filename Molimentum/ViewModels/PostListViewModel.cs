using System.Collections.Generic;
using Molimentum.Models;

namespace Molimentum.ViewModels
{
    public class PostListViewModel
    {
        public PagedResult<Post> Posts { get; set; }

        public IEnumerable<CommentCount> CommentCounts { get; set; }
    }
}