using System.Collections.Generic;

namespace Molimentum.Models
{
    public interface ICommentable
    {
        void AddComment(Comment comment);

        void RemoveComment(Comment comment);

        IEnumerable<Comment> Comments { get; set; }
    }
}