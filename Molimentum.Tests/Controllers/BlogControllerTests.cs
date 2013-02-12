using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using FluentAssertions;
using Molimentum.Controllers;
using Molimentum.Models;
using Molimentum.ViewModels;
using Raven.Client;
using Xunit;

namespace Molimentum.Tests.Controllers
{
    public class BlogControllerTests : IDisposable
    {
        private readonly IDocumentStore _documentStore;

        public BlogControllerTests()
        {
            _documentStore = RavenDocumentStoreFactory.CreateInMemoryDocumentStore();
        }

        public void Dispose()
        {
            if (_documentStore != null) _documentStore.Dispose();
        }

        [Fact]
        public void When_Detail_For_Unknown_Post_Is_Requested_HttpNotFound_Is_Returned()
        {
            using(var session = _documentStore.OpenSession())
            {
                var controller = new BlogController(session);

                var result = controller.Detail("undefined");

                result.Should().Match<HttpStatusCodeResult>(r => r.StatusCode == (int)HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void When_Detail_For_Existing_Post_Is_Requested_A_View_Is_Returned()
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(new Post { Slug = "slug1" });
                session.SaveChanges();
                session.WaitForNonStaleIndexes();
            }

            using (var session = _documentStore.OpenSession())
            {
                var controller = new BlogController(session);

                var result = controller.Detail("slug1");

                result.Should().BeOfType<ViewResult>()
                    .And.Subject.As<ViewResult>().Model.Should().BeOfType<Post>();
            }
        }

        [Fact]
        public void When_Detail_For_Unknown_Category_Is_Requested_HttpNotFound_Is_Returned()
        {
            using (var session = _documentStore.OpenSession())
            {
                var controller = new BlogController(session);

                var result = controller.CategoryDetail("undefined", null);

                result.Should().Match<HttpStatusCodeResult>(r => r.StatusCode == (int)HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void When_Detail_For_Existing_Category_Is_Requested_A_View_Is_Returned()
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(new Category { Slug = "slug1" });
                session.SaveChanges();
                session.WaitForNonStaleIndexes();
            }

            using (var session = _documentStore.OpenSession())
            {
                var controller = new BlogController(session);

                var result = controller.CategoryDetail("slug1", null);

                result.Should().BeOfType<ViewResult>()
                    .And.Subject.As<ViewResult>().Model.Should().BeOfType<CategoryDetailViewModel>();
            }
        }

        [Fact]
        public void When_Index_For_Unknown_Tag_Is_Requested_HttpNotFound_Is_Returned()
        {
            using (var session = _documentStore.OpenSession())
            {
                var controller = new BlogController(session);

                var result = controller.TagIndex("undefined", null);

                result.Should().Match<HttpStatusCodeResult>(r => r.StatusCode == (int)HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void When_Index_For_Existing_Tag_Is_Requested_A_View_Is_Returned()
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(new Post { Tags = new [] { "tag1" } });
                session.SaveChanges();
                session.WaitForNonStaleIndexes();
            }

            using (var session = _documentStore.OpenSession())
            {
                var controller = new BlogController(session);

                var result = controller.TagIndex("tag1", null);

                result.Should().BeOfType<ViewResult>()
                    .And.Subject.As<ViewResult>().Model.Should().BeOfType<TagIndexViewModel>();
            }
        }

        [Fact]
        public void When_Redirect_For_Unknown_Post_Is_Requested_HttpNotFound_Is_Returned()
        {
            using (var session = _documentStore.OpenSession())
            {
                var controller = new BlogController(session);

                var result = controller.RedirectOld("undefined");

                result.Should().Match<HttpStatusCodeResult>(r => r.StatusCode == (int)HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void When_Redirect_For_Existing_Post_Is_Requested_A_Redirect_Is_Returned()
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(new Post { Id = "id1", Slug = "slug1" });
                session.SaveChanges();
                session.WaitForNonStaleIndexes();
            }

            using (var session = _documentStore.OpenSession())
            {
                var controller = new BlogController(session);

                var result = controller.RedirectOld("id1");

                var expectedRouteValues = new Dictionary<string, object> { { "action", "Detail" },  { "Slug", "slug1" } };

                result.Should().BeOfType<RedirectToRouteResult>()
                    .And.Subject.As<RedirectToRouteResult>().RouteValues.Should().Equal(expectedRouteValues);
            }
        }
    }
}
