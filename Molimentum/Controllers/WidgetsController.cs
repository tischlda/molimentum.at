using System.Linq;
using System.Web.Mvc;
using Molimentum.Infrastructure;
using Molimentum.Infrastructure.Raven;
using Molimentum.Models;
using Molimentum.ViewModels;
using Raven.Client;

namespace Molimentum.Controllers
{
    public class WidgetsController : Controller
    {
        private readonly IDocumentSession _session;
        
        public WidgetsController(IDocumentSession session)
        {
            _session = session;
        }

        public ActionResult Categories()
        {
            var categories = _session
                .Query<Category>()
                .OrderBy(item => item.Title);

            return PartialView(categories);
        }

        public ActionResult TagCloud()
        {
            var tagClassifier = new Classifier(new ExponentialClassBuilder(6, 1.4));

            var viewModel = new TagCloudViewModel
            {
                ClassifiedTags = _session.Query<TagCount, Post_Tags_Count>()
                    .OrderBy(item => item.Tag)
                    .ToList()
                    .AsCLassified(tagClassifier, tag => tag.Count)
            };

            return PartialView(viewModel);
        }

        public ActionResult LatestPost()
        {
            var latestPost = _session.Query<Post>()
                .OrderByDescending(item => item.DateTime)
                .AsPagedResult(1, 1)
                .Items.FirstOrDefault();

            return PartialView(latestPost);
        }

        public ActionResult LatestAlbum()
        {
            var latestAlbum = _session.Query<Album>()
                    .OrderByDescending(item => item.DateTime)
                    .AsPagedResult(1, 1)
                    .Items.FirstOrDefault();

            return PartialView(latestAlbum);
        }

        public ActionResult LatestPositionReports()
        {
            var latestPositionReports =_session.Query<PositionReport>()
                    .OrderByDescending(positionReport => positionReport.DateTime)
                    .Take(10);

            return PartialView(latestPositionReports);
        }

        public ActionResult LatestComments()
        {
            var latestComments = _session.Query<CommentWithParent_ByDateTime.Result, CommentWithParent_ByDateTime>()
                .Customize(custom => custom
                    .Include<CommentWithParent_ByDateTime.Result>(item => item.ParentId))
                .OrderByDescending(c => c.DateTime)
                .Take(10)
                .ToList()
                .Select(item =>
                        new CommentWithParent
                        {
                            Comment = item.Comment,
                            Parent = _session.Load<object>(item.ParentId)
                        });

            return PartialView(latestComments);
        }
    }
}
