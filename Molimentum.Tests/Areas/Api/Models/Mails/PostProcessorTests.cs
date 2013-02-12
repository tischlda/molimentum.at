using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Molimentum.Areas.Api.Models.Mails;
using Molimentum.Models;
using Raven.Client;
using Xunit;

namespace Molimentum.Tests.Areas.Api.Models.Mails
{
    public class PostProcessorTests : IDisposable
    {
        private readonly IDocumentStore _documentStore;

        public PostProcessorTests()
        {
            _documentStore = RavenDocumentStoreFactory.CreateInMemoryDocumentStore();
        }

        public void Dispose()
        {
            if (_documentStore != null) _documentStore.Dispose();
        }

        [Fact]
        public void NewPostWithNewCategoryGetsStored()
        {
            using (var session = _documentStore.OpenSession())
            {
                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "TIME", "18.12.2011 19:06" },
                    { "DATEFROM", "11.11.2011" },
                    { "DATETO", "22.11.2011" },
                    { "CATEGORY", "TestCategory" },
                    { "TAGS", "TestTag1,TestTag2" },
                    { "TITLE", "TestTitle" },
                    { "BODY", "TestBody" }
                };


                var postProcessor = new PostProcessor();
                postProcessor.Process(session, data);

                
                var posts = session.Query<Post>().Customize(c => c.WaitForNonStaleResults());

                posts.AsEnumerable().Should().HaveCount(1);
                var post = posts.First();

                var expectedPost = new Post
                {
                    DateTime = new DateTimeOffset(2011, 12, 18, 19, 6, 0, new TimeSpan(0)),
                    DateFrom = new DateTimeOffset(2011, 11, 11, 0, 0, 0, new TimeSpan(0)),
                    DateTo = new DateTimeOffset(2011, 11, 22, 0, 0, 0, new TimeSpan(0)),
                    Category = new PostCategory { Title = "TestCategory", Slug = "TestCategory".GenerateSlug() },
                    Tags = new[] { "TestTag1", "TestTag2" },
                    Title = "TestTitle",
                    Body = "TestBody",
                    Slug = "TestTitle".GenerateSlug()
                };

                post.ShouldHave().AllPropertiesBut(p => p.Id, p => p.Category).EqualTo(expectedPost);


                var categories = session.Query<Category>().Customize(c => c.WaitForNonStaleResults());

                categories.AsEnumerable().Should().HaveCount(1);
                var category = categories.First();

                var expectedCategory = new Category
                {
                    Title = "TestCategory",
                    Slug = "TestCategory".GenerateSlug()
                };

                category.ShouldHave().AllPropertiesBut(c => c.Id).EqualTo(expectedCategory);

                post.Category.ShouldHave().AllProperties().EqualTo(category);
            }
        }

        [Fact]
        public void PostUpdateGetStored()
        {
            var originalCategory = new Category
            {
                Title = "TestCategory",
                Slug = "TestCategory".GenerateSlug()
            };

            var originalPost = new Post
            {
                DateTime = new DateTimeOffset(2011, 12, 18, 19, 5, 0, new TimeSpan(0)),
                DateFrom = new DateTimeOffset(2011, 11, 10, 0, 0, 0, new TimeSpan(0)),
                DateTo = new DateTimeOffset(2011, 11, 21, 0, 0, 0, new TimeSpan(0)),
                Category = originalCategory,
                Tags = new[] { "TestTag0", "TestTag1" },
                Title = "TestTitle0",
                Body = "TestBody0",
                Slug = "TestTitle0".GenerateSlug()
            };

            using (var session = _documentStore.OpenSession())
            {
                session.Store(originalCategory);
                session.Store(originalPost);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "ID", originalPost.Id },
                    { "TIME", "18.12.2011 19:06" },
                    { "DATEFROM", "11.11.2011" },
                    { "DATETO", "22.11.2011" },
                    { "CATEGORY", "TestCategory" },
                    { "TAGS", "TestTag1,TestTag2" },
                    { "TITLE", "TestTitle" },
                    { "BODY", "TestBody" }
                };


                var postProcessor = new PostProcessor();
                postProcessor.Process(session, data);


                var posts = session.Query<Post>().Customize(c => c.WaitForNonStaleResults());

                posts.AsEnumerable().Should().HaveCount(1);
                var post = posts.First();

                var expectedPost = new Post
                {
                    DateTime = new DateTimeOffset(2011, 12, 18, 19, 6, 0, new TimeSpan(0)),
                    DateFrom = new DateTimeOffset(2011, 11, 11, 0, 0, 0, new TimeSpan(0)),
                    DateTo = new DateTimeOffset(2011, 11, 22, 0, 0, 0, new TimeSpan(0)),
                    Category = new PostCategory { Title = "TestCategory", Slug = "TestCategory".GenerateSlug() },
                    Tags = new[] { "TestTag1", "TestTag2" },
                    Title = "TestTitle",
                    Body = "TestBody",
                    Slug = "TestTitle0".GenerateSlug()
                };

                post.ShouldHave().AllPropertiesBut(p => p.Id, p => p.Category).EqualTo(expectedPost);
                post.Id.Should().Be(originalPost.Id);


                var categories = session.Query<Category>().Customize(c => c.WaitForNonStaleResults());

                categories.AsEnumerable().Should().HaveCount(1);

                post.Category.ShouldHave().AllPropertiesBut(c => c.Id).EqualTo(expectedPost.Category);
            }
        }

        [Fact]
        public void PostUpdateWithCategoryUpdateGetStored()
        {
            var originalCategory = new Category
            {
                Title = "TestCategory0",
                Slug = "TestCategory0".GenerateSlug()
            };

            var originalPost = new Post
            {
                DateTime = new DateTimeOffset(2011, 12, 18, 19, 5, 0, new TimeSpan(0)),
                DateFrom = new DateTimeOffset(2011, 11, 10, 0, 0, 0, new TimeSpan(0)),
                DateTo = new DateTimeOffset(2011, 11, 21, 0, 0, 0, new TimeSpan(0)),
                Category = originalCategory,
                Tags = new [] { "TestTag0", "TestTag1" },
                Title = "TestTitle0",
                Body = "TestBody0",
                Slug = "TestTitle0".GenerateSlug()
            };

            using (var session = _documentStore.OpenSession())
            {
                session.Store(originalCategory);
                session.Store(originalPost);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "ID", originalPost.Id },
                    { "TIME", "18.12.2011 19:06" },
                    { "DATEFROM", "11.11.2011" },
                    { "DATETO", "22.11.2011" },
                    { "CATEGORY", "TestCategory" },
                    { "TAGS", "TestTag1,TestTag2" },
                    { "TITLE", "TestTitle" },
                    { "BODY", "TestBody" }
                };


                var postProcessor = new PostProcessor();
                postProcessor.Process(session, data);


                var posts = session.Query<Post>().Customize(c => c.WaitForNonStaleResults());

                posts.AsEnumerable().Should().HaveCount(1);
                var post = posts.First();

                var expectedPost = new Post
                {
                    DateTime = new DateTimeOffset(2011, 12, 18, 19, 6, 0, new TimeSpan(0)),
                    DateFrom = new DateTimeOffset(2011, 11, 11, 0, 0, 0, new TimeSpan(0)),
                    DateTo = new DateTimeOffset(2011, 11, 22, 0, 0, 0, new TimeSpan(0)),
                    Category = new PostCategory { Title = "TestCategory", Slug = "TestCategory".GenerateSlug() },
                    Tags = new[] { "TestTag1", "TestTag2" },
                    Title = "TestTitle",
                    Body = "TestBody",
                    Slug = "TestTitle0".GenerateSlug()
                };

                post.ShouldHave().AllPropertiesBut(p => p.Id, p => p.Category).EqualTo(expectedPost);
                post.Id.Should().Be(originalPost.Id);


                var categories = session.Query<Category>().Customize(c => c.WaitForNonStaleResults());

                categories.AsEnumerable().Should().HaveCount(2);

                post.Category.ShouldHave().AllPropertiesBut(c => c.Id).EqualTo(expectedPost.Category);
            }
        }

        [Fact]
        public void NewPostWithExistingCategoryGetsStored()
        {
            var category = new Category
            {
                Title = "TestCategory",
                Slug = "TestCategory".GenerateSlug()
            };

            using (var session = _documentStore.OpenSession())
            {
                session.Store(category);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "TIME", "18.12.2011 19:06" },
                    { "DATEFROM", "11.11.2011" },
                    { "DATETO", "22.11.2011" },
                    { "CATEGORY", "TestCategory" },
                    { "TAGS", "TestTag1,TestTag2" },
                    { "TITLE", "TestTitle" },
                    { "BODY", "TestBody" }
                };

                var postProcessor = new PostProcessor();
                postProcessor.Process(session, data);


                var posts = session.Query<Post>().Customize(c => c.WaitForNonStaleResults());

                posts.AsEnumerable().Should().HaveCount(1);
                var post = posts.First();

                var expectedPost = new Post
                {
                    DateTime = new DateTimeOffset(2011, 12, 18, 19, 6, 0, new TimeSpan(0)),
                    DateFrom = new DateTimeOffset(2011, 11, 11, 0, 0, 0, new TimeSpan(0)),
                    DateTo = new DateTimeOffset(2011, 11, 22, 0, 0, 0, new TimeSpan(0)),
                    Category = new PostCategory { Title = "TestCategory", Slug = "TestCategory".GenerateSlug() },
                    Tags = new [] { "TestTag1", "TestTag2" },
                    Title = "TestTitle",
                    Body = "TestBody",
                    Slug = "TestTitle".GenerateSlug()
                };

                post.ShouldHave().AllPropertiesBut(p => p.Id, p => p.Category).EqualTo(expectedPost);


                post.Category.ShouldHave().AllProperties().EqualTo(category);
            }
        }

        [Fact]
        public void PostGetsDeleted()
        {
            var originalCategory = new Category
            {
                Title = "TestCategory",
                Slug = "TestCategory".GenerateSlug()
            };

            var originalPost = new Post
            {
                DateTime = new DateTimeOffset(2011, 12, 18, 19, 5, 0, new TimeSpan(0)),
                DateFrom = new DateTimeOffset(2011, 11, 10, 0, 0, 0, new TimeSpan(0)),
                DateTo = new DateTimeOffset(2011, 11, 21, 0, 0, 0, new TimeSpan(0)),
                Category = originalCategory,
                Tags = new [] { "TestTag0", "TestTag1" },
                Title = "TestTitle0",
                Body = "TestBody0",
                Slug = "TestTitle0".GenerateSlug()
            };

            using (var session = _documentStore.OpenSession())
            {
                session.Store(originalCategory);
                session.Store(originalPost);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "ID", originalPost.Id },
                    { "COMMAND", "DELETE" }
                };


                var postProcessor = new PostProcessor();
                postProcessor.Process(session, data);


                var posts = session.Query<Post>().Customize(c => c.WaitForNonStaleResults());

                posts.AsEnumerable().Should().HaveCount(0);
            }
        }
    }
}