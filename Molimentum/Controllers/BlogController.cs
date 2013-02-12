using System.Linq;
using System.Web.Mvc;
using Molimentum.Infrastructure;
using Molimentum.Infrastructure.Raven;
using Molimentum.Models;
using Molimentum.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace Molimentum.Controllers
{
    public class BlogController : Controller
    {
        private readonly IDocumentSession _session;
        
        private const int _pageSize = 5;

        public BlogController(IDocumentSession session)
        {
            _session = session;
        }

        public ActionResult Index(int? page)
        {
            var result = _session.Query<Post>()
                .OrderByDescending(item => item.DateTime)
                .AsPagedResult(page, _pageSize);

            return View(result);
        }

        public ActionResult Detail(string slug)
        {
            var post = _session.Query<Post>()
                .SingleOrDefault(item => item.Slug == slug);

            if (post == null) return HttpNotFound();

            return View(post);
        }

        public ActionResult CategoryDetail(string slug, int? page)
        {
            var category = _session.Query<Category>()
                .SingleOrDefault(c => c.Slug == slug);

            if (category == null) return HttpNotFound();

            var result = _session.Query<Post>()
                .Where(item => item.Category.Id == category.Id)
                .OrderByDescending(item => item.DateTime)
                .AsPagedResult(page, _pageSize);

            var viewModel = new CategoryDetailViewModel
            {
                Category = category,
                Posts = result
            };

            return View(viewModel);
        }

        public ActionResult TagIndex(string tag, int? page)
        {
            var tagCount = _session.Query<TagCount, Post_Tags_Count>()
                .FirstOrDefault(item => item.Tag == tag);

            if (tagCount == null) return HttpNotFound();

            var result = _session.Query<Post>()
                .Where(item => item.Tags.Any(t => t == tag))
                .OrderByDescending(item => item.DateTime)
                .AsPagedResult(page, _pageSize);

            var viewModel = new TagIndexViewModel
            {
                Tag = tagCount.Tag,
                Posts = result
            };

            return View(viewModel);
        }

        public ActionResult RedirectOld(string id)
        {
            var post = _session.Load<Post>(id);

            if (post == null) return HttpNotFound();

            return RedirectToActionPermanent("Detail", new { post.Slug });
        }
    }
}
