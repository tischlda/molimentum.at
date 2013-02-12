using System.Linq;
using Molimentum.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Molimentum.Infrastructure.Raven
{
    public class Picture_ByDateTime : AbstractIndexCreationTask<Album, Picture>
    {
        public Picture_ByDateTime()
        {
            Map = albums => from album in albums
                            from picture in album.Pictures
                            select new Picture
                            {
                                Id = picture.Id,
                                Body = picture.Body,
                                DateTime = picture.DateTime,
                                Links = picture.Links,
                                Title = picture.Title,
                                Position = picture.Position
                            };

            Reduce = results => from result in results
                                group result by result.Id into g
                                select new Picture
                                {
                                    Id = g.Select(x => x.Id).First(),
                                    Body = g.Select(x => x.Body).First(),
                                    DateTime = g.Select(x => x.DateTime).First(),
                                    Links = g.Select(x => x.Links).First(),
                                    Title = g.Select(x => x.Title).First(),
                                    Position = g.Select(x => x.Position).First()
                                };

            Sort(result => result.DateTime, SortOptions.String);
        }
    }
}