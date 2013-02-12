using System;
using System.Collections.Generic;
using System.Linq;
using Molimentum.Models;
using Raven.Client;

namespace Molimentum.Areas.Api.Models.Mails
{
    public class PostProcessor : ProcessorBase<Post>
    {
        private readonly char[] _tagSeparator = new[] { ',' };

        protected override void Save(IDocumentSession session, IDictionary<string, string> data)
        {
            Post post;

            if (data.ContainsKey("ID"))
            {
                post = session.Load<Post>(data["ID"]);
            }
            else
            {
                post = new Post { Slug = data["TITLE"].GenerateSlug() };
            }

            post.Title = data["TITLE"];
            post.DateTime = ParseDateTime(data["TIME"]);
            post.DateFrom = data.ContainsKey("DATEFROM") ? ParseDateTime(data["DATEFROM"]) : (DateTimeOffset?)null;
            post.DateTo = data.ContainsKey("DATETO") ? ParseDateTime(data["DATETO"]) : (DateTimeOffset?)null ;
            post.Tags = data.ContainsKey("TAGS") ? data["TAGS"].Split(_tagSeparator) : null;
            post.Body = data["BODY"];

            var category = session.Query<Category>()
                .Customize(c => c.WaitForNonStaleResults())
                .FirstOrDefault(c => c.Title == data["CATEGORY"]);

            if (category == null)
            {
                category = new Category
                {
                    Title = data["CATEGORY"]
                };
                category.Slug = category.Title.GenerateSlug();

                session.Store(category);
            }

            post.Category = category;

            session.Store(post);

            session.SaveChanges();
        }
    }
}