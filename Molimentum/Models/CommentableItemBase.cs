using System;
using System.Collections.Generic;

namespace Molimentum.Models
{
    public abstract class CommentableItemBase : ItemBase, ICommentable
    {
        private List<Comment> _comments = new List<Comment>();

        public void AddComment(Comment comment)
        {
            comment.Id = Guid.NewGuid().ToString();
            _comments.Insert(0, comment);
        }

        public void RemoveComment(Comment comment)
        {
            _comments.Remove(comment);
        }

        public IEnumerable<Comment> Comments
        {
            get { return _comments; }
            set { _comments = new List<Comment>(value); }
        }
    }
}