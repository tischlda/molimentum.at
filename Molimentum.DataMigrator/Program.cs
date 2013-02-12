using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Molimentum.Areas.Admin.Models.Synchronization;
using Raven.Client.Indexes;
using CommandLine;
using System;

namespace Molimentum.DataMigrator
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();

            if (CommandLineParser.Default.ParseArguments(args, options))
            {
                using (var documentStore = Molimentum.Infrastructure.RavenDocumentStoreFactory.CreateDocumentStore())
                {
                    IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), documentStore);
                    
                    if (options.Blog || options.Categories)
                        using (var sessionBuilder = new Molimentum.Providers.NHibernate.NHibernateSessionBuilder())
                            MigrateCategories(sessionBuilder, documentStore);

                    if (options.Blog || options.Posts)
                        using (var sessionBuilder = new Molimentum.Providers.NHibernate.NHibernateSessionBuilder())
                            MigratePosts(sessionBuilder, documentStore);

                    if (options.Blog || options.Comments)
                        using (var sessionBuilder = new Molimentum.Providers.NHibernate.NHibernateSessionBuilder())
                            MigrateGuestbookComments(sessionBuilder, documentStore);

                    if (options.Blog || options.PositionReports)
                        using (var sessionBuilder = new Molimentum.Providers.NHibernate.NHibernateSessionBuilder())
                            MigratePositionReports(sessionBuilder, documentStore);

                    if (options.Picasa)
                    {
                        var picasaImport = new PicasaImport(documentStore);
                        picasaImport.ImportPicasa();
                    }
                }
            }
        }

        private static IEnumerable<T> List<T>(Molimentum.Repositories.IItemRepository<T> repository)
        {
            var page = 1;
            var pageSize = int.MaxValue;

            bool hasMorePages = true;

            do
            {
                var list = repository.List(page, pageSize);

                foreach (var item in list.Items)
                {
                    yield return item;
                }

                page++;
                hasMorePages = list.Page < list.Pages;
            }
            while (hasMorePages);
        }

        private static void MigrateCategories(Providers.NHibernate.NHibernateSessionBuilder sessionBuilder, Raven.Client.IDocumentStore documentStore)
        {
            var repository = new Molimentum.Providers.NHibernate.NHibernatePostCategoryRepository(sessionBuilder);

            foreach (var newItem in from item in List(repository)
                                    select new Molimentum.Models.Category
                                    {
                                        //item.PostListOrder
                                        Body = item.Body,
                                        Id = item.ID,
                                        Title = item.Title
                                    })
            {

                newItem.Slug = newItem.Title.GenerateSlug();

                Console.WriteLine("Category/" + newItem.Title);


                using (var session = documentStore.OpenSession())
                {
                    session.Store(newItem);
                    session.SaveChanges();
                }
            }
        }

        private static void MigratePosts(Providers.NHibernate.NHibernateSessionBuilder sessionBuilder, Raven.Client.IDocumentStore documentStore)
        {
            var repository = new Molimentum.Providers.NHibernate.NHibernatePostRepository(sessionBuilder);

            foreach (var item in List(repository).Where(item => item.IsPublished))
            {
                Console.WriteLine("Post/" + item.Title);

                var newItem = new Molimentum.Models.Post
                {
                    // item.AuthorID
                    Body = item.Body,
                    //item.Comments
                    DateFrom = Convert.FromDateTime(item.DateFrom),
                    DateTo = Convert.FromDateTime(item.DateTo),
                    Id = item.ID,
                    // item.IsPublished
                    // item.LastUpdatedTime
                    // item.Position
                    //item.PositionDateTime
                    DateTime = Convert.FromDateTime(item.PublishDate.Value).Value,
                    Tags = item.Tags.ToArray().AsEnumerable(),
                    Title = item.Title
                };

                newItem.Slug = newItem.Title.GenerateSlug();

                using (var session = documentStore.OpenSession())
                {
                    newItem.Category = session.Load<Molimentum.Models.Category>(item.Category.ID);

                    session.Store(newItem);
                    session.SaveChanges();
                }
            }
        }

        private static void MigrateGuestbookComments(Providers.NHibernate.NHibernateSessionBuilder sessionBuilder, Raven.Client.IDocumentStore documentStore)
        {
            var repository = new Molimentum.Providers.NHibernate.NHibernatePostCommentRepository(sessionBuilder);

            foreach (var item in List(repository))
            {
                Console.WriteLine("PostComment/" + item.Title);

                var newItem = new Molimentum.Models.Comment
                {
                    Author = item.Author,
                    Body = item.Body,
                    Email = item.Email,
                    Id = item.ID,
                    DateTime = Convert.FromDateTime(item.PublishDate.Value).Value,
                    Title = item.Title,
                    Website = item.Website,
                    ParentId = item.Post == null ? null : item.Post.ID
                };

                using (var session = documentStore.OpenSession())
                {
                    session.Store(newItem);
                    session.SaveChanges();
                }
            }
        }

        private static void MigratePositionReports(Providers.NHibernate.NHibernateSessionBuilder sessionBuilder, Raven.Client.IDocumentStore documentStore)
        {
            var repository = new Molimentum.Providers.NHibernate.NHibernatePositionReportRepository(sessionBuilder);

            foreach (var item in List(repository))
            {
                Console.WriteLine("PositionReport/" + item.PositionDateTime);

                var newItem = new Molimentum.Models.PositionReport
                {
                    Comment = item.Comment,
                    //item.Course
                    Id = item.ID,
                    Position = new Molimentum.Models.Position(item.Position.Latitude, item.Position.Longitude),
                    DateTime = Convert.FromDateTime(item.PositionDateTime.Value).Value
                    //item.Speed
                    //item.WindDirection
                    //item.WindSpeed
                };

                using (var session = documentStore.OpenSession())
                {
                    session.Store(newItem);
                    session.SaveChanges();
                }
            }
        }
    }
}
