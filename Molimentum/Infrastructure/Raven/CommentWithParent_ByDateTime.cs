using System;
using System.Linq;
using Molimentum.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Molimentum.Infrastructure.Raven
{
    public class CommentWithParent_ByDateTime : AbstractMultiMapIndexCreationTask<CommentWithParent_ByDateTime.Result>
    {
        public CommentWithParent_ByDateTime()
        {
            AddMap<Post>(posts =>
                from post in posts
                from comment in post.Comments
                select new Result
                {
                    CommentId = comment.Id,
                    ParentId = post.Id,
                    DateTime = comment.DateTime,
                    Comment = comment
                });

            AddMap<Album>(albums =>
                from album in albums
                from comment in album.Comments
                select new Result
                {
                    CommentId = comment.Id,
                    ParentId = album.Id,
                    DateTime = comment.DateTime,
                    Comment = comment
                });

            AddMap<Comment>(comments =>
                from comment in comments
                select new Result
                {
                    CommentId = comment.Id,
                    ParentId = "",
                    DateTime = comment.DateTime,
                    Comment = comment
                });

            Reduce = results =>
                from result in results
                group result by result.CommentId into g
                select new Result
                {
                    CommentId = g.First().CommentId,
                    ParentId = g.First().ParentId,
                    DateTime = g.First().DateTime,
                    Comment = g.First().Comment
                };

            Index(result => result.DateTime, FieldIndexing.NotAnalyzed);
            Sort(result => result.DateTime, SortOptions.String);
        }

        public class Result
        {
            public string CommentId { get; set; }
            public string ParentId { get; set; }
            public DateTimeOffset DateTime { get; set; }
            public Comment Comment { get; set; }
        }
    }
}