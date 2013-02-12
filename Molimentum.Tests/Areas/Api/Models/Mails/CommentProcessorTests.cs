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
    public class CommentProcessorTests : IDisposable
    {
        private readonly IDocumentStore _documentStore;

        public CommentProcessorTests()
        {
            _documentStore = RavenDocumentStoreFactory.CreateInMemoryDocumentStore();
        }

        public void Dispose()
        {
            if (_documentStore != null) _documentStore.Dispose();
        }

        [Fact]
        public void NewCommentGetsStored()
        {
            using (var session = _documentStore.OpenSession())
            {
                var testParent = new Post { Id = "TestParentId" };
                session.Store(testParent);

                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "TIME", "18.12.2011 19:06" },
                    { "AUTHOR", "TestAuthor" },
                    { "EMAIL", "test@email.com" },
                    { "WEBSITE", "http://test.website.com" },
                    { "TITLE", "TestTitle" },
                    { "BODY", "TestBody" }
                };


                var commentProcessor = new CommentProcessor();
                commentProcessor.Process(session, data);


                var comments = session.Query<Comment>().Customize(c => c.WaitForNonStaleResults());

                comments.AsEnumerable().Should().HaveCount(1);
                var comment = comments.First();

                var expectedComment = new Comment
                {
                    DateTime = new DateTimeOffset(2011, 12, 18, 19, 6, 0, new TimeSpan(0)),
                    Author = "TestAuthor",
                    Email = "test@email.com",
                    Website = "http://test.website.com",
                    Title = "TestTitle",
                    Body = "TestBody"
                };

                comment.Id.Should().NotBeNullOrEmpty();
                comment.ShouldHave().AllPropertiesBut(p => p.Id).EqualTo(expectedComment);
            }
        }

        [Fact]
        public void NewCommentWithParentGetsStored()
        {
            using (var session = _documentStore.OpenSession())
            {
                var testParent = new Post { Id = "TestParentId" };
                session.Store(testParent);

                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "TIME", "18.12.2011 19:06" },
                    { "AUTHOR", "TestAuthor" },
                    { "EMAIL", "test@email.com" },
                    { "WEBSITE", "http://test.website.com" },
                    { "PARENTID", "TestParentId" },
                    { "TITLE", "TestTitle" },
                    { "BODY", "TestBody" }
                };


                var commentProcessor = new CommentProcessor();
                commentProcessor.Process(session, data);


                var post = session.Load<Post>("TestParentId");

                post.Comments.Should().HaveCount(1);
                var comment = post.Comments.First();

                var expectedComment = new Comment
                {
                    DateTime = new DateTimeOffset(2011, 12, 18, 19, 6, 0, new TimeSpan(0)),
                    Author = "TestAuthor",
                    Email = "test@email.com",
                    Website = "http://test.website.com",
                    Title = "TestTitle",
                    Body = "TestBody"
                };

                comment.Id.Should().NotBeNullOrEmpty();
                comment.ShouldHave().AllPropertiesBut(p => p.Id).EqualTo(expectedComment);
            }
        }

        [Fact]
        public void CommentUpdateThrowsNotImplementedException()
        {
            var originalComment = new Comment
            {
                DateTime = new DateTimeOffset(2011, 12, 18, 19, 5, 0, new TimeSpan(0)),
                Title = "TestTitle0",
                Author = "TestAuthor0",
                Email = "test0@email.com",
                Website = "http://test0.website.com",
                Body = "TestBody0"
            };

            using (var session = _documentStore.OpenSession())
            {
                var testParent = new Post { Id = "TestParentId" };
                testParent.AddComment(originalComment);
                session.Store(testParent);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "ID", originalComment.Id },
                    { "TIME", "18.12.2011 19:06" },
                    { "AUTHOR", "TestAuthor" },
                    { "EMAIL", "test@email.com" },
                    { "WEBSITE", "http://test.website.com" },
                    { "TITLE", "TestTitle" },
                    { "BODY", "TestBody" }
                };


                var commentProcessor = new CommentProcessor();
                Action action = () => commentProcessor.Process(session, data);

                action.ShouldThrow<NotImplementedException>();
            }
        }

        [Fact]
        public void CommentGetsDeleted()
        {
            var originalComment = new Comment
            {
                DateTime = new DateTimeOffset(2011, 12, 18, 19, 5, 0, new TimeSpan(0)),
                Title = "TestTitle0",
                Author = "TestAuthor0",
                Email = "test0@email.com",
                Website = "http://test0.website.com",
                Body = "TestBody0"
            };

            using (var session = _documentStore.OpenSession())
            {
                session.Store(originalComment);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "ID", originalComment.Id },
                    { "COMMAND", "DELETE" }
                };


                var commentProcessor = new CommentProcessor();
                commentProcessor.Process(session, data);


                var comments = session.Query<Comment>().Customize(c => c.WaitForNonStaleResults());

                comments.AsEnumerable().Should().HaveCount(0);
            }
        }

        [Fact]
        public void CommentWithParentGetsDeleted()
        {
            var originalComment = new Comment
            {
                DateTime = new DateTimeOffset(2011, 12, 18, 19, 5, 0, new TimeSpan(0)),
                Title = "TestTitle0",
                Author = "TestAuthor0",
                Email = "test0@email.com",
                Website = "http://test0.website.com",
                Body = "TestBody0"
            };

            var testParent = new Post { Id = "TestParentId" };
            testParent.AddComment(originalComment);

            using (var session = _documentStore.OpenSession())
            {
                session.Store(testParent);
                session.SaveChanges();
                session.WaitForNonStaleIndexes();
            }

            using (var session = _documentStore.OpenSession())
            {
                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "ID", originalComment.Id },
                    { "COMMAND", "DELETE" }
                };


                var commentProcessor = new CommentProcessor();
                commentProcessor.Process(session, data);


                var post = session.Query<Post>().Customize(c => c.WaitForNonStaleResults()).First();

                post.Comments.Should().HaveCount(0);
            }
        }
    }
}