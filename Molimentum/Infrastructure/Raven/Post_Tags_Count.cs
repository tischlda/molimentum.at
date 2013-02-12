using System.Linq;
using Molimentum.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Molimentum.Infrastructure.Raven
{
    public class Post_Tags_Count : AbstractIndexCreationTask<Post, TagCount>
    {
        public Post_Tags_Count()
        {
            Map = posts => from post in posts
                           from tag in post.Tags
                           select new TagCount { Tag = tag, Count = 1 };

            Reduce = results => from tagCount in results
                                group tagCount by tagCount.Tag
                                    into g
                                    select new TagCount { Tag = g.Key, Count = g.Sum(x => x.Count) };

            Sort(result => result.Tag, SortOptions.String);
        }
    }
}