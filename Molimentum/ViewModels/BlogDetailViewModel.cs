using System.Collections.Generic;
using Molimentum.Models;

namespace Molimentum.ViewModels
{
    public class BlogDetailViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}