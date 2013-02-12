using Molimentum.Models;

namespace Molimentum.ViewModels
{
    public class CommentWithParent
    {
        public object Parent { get; set; }

        public Comment Comment { get; set; }
    }
}