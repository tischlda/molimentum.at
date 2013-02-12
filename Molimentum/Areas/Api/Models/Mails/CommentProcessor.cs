using System;
using System.Collections.Generic;
using System.Linq;
using Molimentum.Infrastructure.Raven;
using Molimentum.Models;
using Raven.Client;

namespace Molimentum.Areas.Api.Models.Mails
{
    public class CommentProcessor : ProcessorBase<Comment>
    {
        protected override void Save(IDocumentSession session, IDictionary<string, string> data)
        {
            if (data.ContainsKey("ID"))
            {
                throw new NotImplementedException("Comment updating not supported.");
            }
            
            var comment = new Comment
            {
                Title = data["TITLE"],
                DateTime = ParseDateTime(data["TIME"]),
                Author = data["AUTHOR"],
                Email = data["EMAIL"],
                Website = data.ContainsKey("WEBSITE") ? data["WEBSITE"] : null,
                Body = data["BODY"]
            };

            if (data.ContainsKey("PARENTID"))
            {
                var parent = session.Load<object>(data["PARENTID"]) as ICommentable;
                parent.AddComment(comment);
            }
            else
            {
                session.Store(comment);
            }

            session.SaveChanges();
        }

        protected override void Delete(IDocumentSession session, IDictionary<string, string> data)
        {
            var comment = session.Load<Comment>(data["ID"]);

            if (comment != null)
            {
                session.Delete(comment);
            }
            else
            {
                var commentWithParent = session.Query<CommentWithParent_ByDateTime.Result, CommentWithParent_ByDateTime>()
                .Customize(custom => custom
                    .Include<CommentWithParent_ByDateTime.Result>(item => item.ParentId))
                .FirstOrDefault(item => item.CommentId == data["ID"]);

                if (commentWithParent != null)
                {
                    var parent = session.Load<ICommentable>(commentWithParent.ParentId);
                    comment = parent.Comments.FirstOrDefault(item => item.Id == data["ID"]);
                    parent.RemoveComment(comment);
                }
            }

            session.SaveChanges();
        }
    }
}