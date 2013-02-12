using System.Linq;
using System.Web.Mvc;
using Molimentum.Infrastructure;
using Molimentum.Models;
using Raven.Client;
using Raven.Client.Linq;

namespace Molimentum.Controllers
{
    public class PhotosController : Controller
    {
        private readonly IDocumentSession _session;
        
        private const int _pageSize = 10;

        public PhotosController(IDocumentSession session)
        {
            _session = session;
        }

        public ActionResult Index(int? page)
        {
            var result = _session.Query<Album>()
                .OrderByDescending(item => item.DateTime)
                .AsPagedResult(page, _pageSize);

            return View(result);
        }

        public ActionResult Detail(string slug)
        {
            var album = _session.Query<Album>()
                .FirstOrDefault(a => a.Slug == slug);

            if (album == null) return HttpNotFound();

            return View(album);
        }

        public ActionResult RedirectOld(string id)
        {
            var album = _session.Load<Album>(id);

            if (album == null) return HttpNotFound();

            return RedirectToActionPermanent("Detail", new { album.Slug });
        }
    }
}