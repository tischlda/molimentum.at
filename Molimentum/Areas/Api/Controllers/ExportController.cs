using System.Linq;
using System.Web.Mvc;
using Molimentum.Infrastructure;
using Molimentum.Models;
using Raven.Client;
using Raven.Client.Linq;

namespace Molimentum.Areas.Api.Controllers
{
    public class ExportController : Controller
    {
        private readonly IDocumentSession _session;
        private const int _defaultPageSize = 10;
        
        public ExportController(IDocumentSession session)
        {
            _session = session;
        }

        [HttpGet]
        public ActionResult Item(string id)
        {
            var item = _session.Load<object>(id);

            if (item == null) return HttpNotFound();

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Posts(int? page, int? pageSize, bool condensed = true)
        {
            var baseQuery = _session.Query<Post>()
                .OrderByDescending(item => item.DateTime);

            object result;

            if (condensed)
            {
                result = baseQuery
                    .Select(item => new { item.Id, item.DateTime, item.Slug, item.Title })
                    .AsPagedResult(page, GetActualPageSize(pageSize));
            }
            else
            {
                result = baseQuery
                    .AsPagedResult(page, GetActualPageSize(pageSize));
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PostBySlug(string slug)
        {
            var post = _session.Query<Post>().SingleOrDefault(item => item.Slug == slug);

            if (post == null) return HttpNotFound();

            return Json(post, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PositionReports(int? page, int? pageSize, bool condensed = true)
        {
            var baseQuery = _session.Query<PositionReport>()
                .OrderByDescending(item => item.DateTime);

            object result;

            if (condensed)
            {
                result = baseQuery
                    .Select(item => new { item.Id, item.DateTime, item.Position, item.Comment })
                    .AsPagedResult(page, GetActualPageSize(pageSize));
            }
            else
            {
                result = baseQuery
                    .AsPagedResult(page, GetActualPageSize(pageSize));
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Comments(int? page, int? pageSize, bool condensed = true)
        {
            var baseQuery = _session.Query<Comment>()
                .OrderByDescending(item => item.DateTime);

            object result;

            if (condensed)
            {
                result = baseQuery
                    .Select(item => new { item.Id, item.DateTime, item.Author, item.Title })
                    .AsPagedResult(page, GetActualPageSize(pageSize));
            }
            else
            {
                result = baseQuery
                    .AsPagedResult(page, GetActualPageSize(pageSize));
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Albums(int? page, int? pageSize, bool condensed = true)
        {
            var baseQuery = _session.Query<Album>()
                .OrderByDescending(item => item.DateTime);

            object result;

            if (condensed)
            {
                result = baseQuery
                    .Select(item => new { item.Id, item.DateTime, item.Slug, item.Title })
                    .AsPagedResult(page, GetActualPageSize(pageSize));
            }
            else
            {
                result = baseQuery
                    .AsPagedResult(page, GetActualPageSize(pageSize));
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AlbumBySlug(string slug)
        {
            var album = _session.Query<Album>().SingleOrDefault(item => item.Slug == slug);

            if (album == null) return HttpNotFound();

            return Json(album, JsonRequestBehavior.AllowGet);
        }

        private static int GetActualPageSize(int? pageSize)
        {
            return pageSize != null ? pageSize.Value : _defaultPageSize;
        }
    }
}
