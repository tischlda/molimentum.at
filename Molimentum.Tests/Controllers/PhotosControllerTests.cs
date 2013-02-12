using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using FluentAssertions;
using Molimentum.Controllers;
using Molimentum.Models;
using Raven.Client;
using Xunit;

namespace Molimentum.Tests.Controllers
{
    public class PhotosControllerTests : IDisposable
    {
        private readonly IDocumentStore _documentStore;

        public PhotosControllerTests()
        {
            _documentStore = RavenDocumentStoreFactory.CreateInMemoryDocumentStore();
        }

        public void Dispose()
        {
            if (_documentStore != null) _documentStore.Dispose();
        }

        [Fact]
        public void When_Detail_For_Unknown_Album_Is_Requested_HttpNotFound_Is_Returned()
        {
            using(var session = _documentStore.OpenSession())
            {
                var controller = new PhotosController(session);

                var result = controller.Detail("undefined");

                result.Should().Match<HttpStatusCodeResult>(r => r.StatusCode == (int)HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void When_Detail_For_Existing_Album_Is_Requested_A_View_Is_Returned()
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(new Album { Slug = "slug1" });
                session.SaveChanges();
                session.WaitForNonStaleIndexes();
            }

            using (var session = _documentStore.OpenSession())
            {
                var controller = new PhotosController(session);

                var result = controller.Detail("slug1");

                result.Should().BeOfType<ViewResult>()
                    .And.Subject.As<ViewResult>().Model.Should().BeOfType<Album>();
            }
        }

        [Fact]
        public void When_Redirect_For_Unknown_Album_Is_Requested_HttpNotFound_Is_Returned()
        {
            using (var session = _documentStore.OpenSession())
            {
                var controller = new PhotosController(session);

                var result = controller.RedirectOld("undefined");

                result.Should().Match<HttpStatusCodeResult>(r => r.StatusCode == (int)HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void When_Redirect_For_Existing_Album_Is_Requested_A_Redirect_Is_Returned()
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Store(new Album { Id = "id1", Slug = "slug1" });
                session.SaveChanges();
                session.WaitForNonStaleIndexes();
            }

            using (var session = _documentStore.OpenSession())
            {
                var controller = new PhotosController(session);

                var result = controller.RedirectOld("id1");

                var expectedRouteValues = new Dictionary<string, object> { { "action", "Detail" }, { "Slug", "slug1" } };

                result.Should().BeOfType<RedirectToRouteResult>()
                    .And.Subject.As<RedirectToRouteResult>().RouteValues.Should().Equal(expectedRouteValues);
            }
        }
    }
}
