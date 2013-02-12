using Molimentum.Models;

namespace Molimentum.ViewModels
{
    public class CommentWithParentId
    {
        public string ParentId { get; set; }

        // can't name it 'Comment' or model binding won't work
        public Comment Item { get; set; }
    }
}