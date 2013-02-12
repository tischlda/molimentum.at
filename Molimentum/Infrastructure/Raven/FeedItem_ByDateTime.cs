using System;
using System.Linq;
using Molimentum.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Molimentum.Infrastructure.Raven
{
    public class FeedItem_ByDateTime : AbstractMultiMapIndexCreationTask<FeedItem_ByDateTime.Result>
    {
        public FeedItem_ByDateTime()
        {
            AddMap<Post>(posts => from post in posts
                         select new Result { Id = post.Id, DateTime = post.DateTime });

            AddMap<Album>(albums => from album in albums
                                    select new Result { Id = album.Id, DateTime = album.DateTime });

            Reduce = results => from result in results
                                group result by result.Id into g
                                  select new Result { Id = g.Key, DateTime = g.First().DateTime };

            Sort(album => album.DateTime, SortOptions.String);
        }

        public class Result
        {
            public string Id { get; set; }
            public DateTimeOffset DateTime { get; set; }
        }
    }
}