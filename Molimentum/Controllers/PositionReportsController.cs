using System;
using System.Linq;
using System.Web.Mvc;
using Molimentum.Infrastructure;
using Molimentum.Models;
using Raven.Client;
using Raven.Client.Linq;

namespace Molimentum.Controllers
{
    public class PositionReportsController : Controller
    {
        private readonly IDocumentSession _session;

        private const int _defaultPageSize = 10;
        private const int _maxPageSize = 100;

        public PositionReportsController(IDocumentSession session)
        {
            _session = session;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int? page, int? pageSize)
        {
            var result = _session.Query<PositionReport>()
                .OrderByDescending(p => p.DateTime)
                .AsPagedResult(page, GetActualPageSize(pageSize));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListByPeriod(DateTimeOffset from, DateTimeOffset to, int? page, int? pageSize)
        {
            var result =
                (_session.Query<PositionReport>()
                    .Where(item => item.DateTime >= from && item.DateTime <= to.AddDays(1))
                    .OrderByDescending(p => p.DateTime))
                    .ToList()
                .Union
                (_session.Query<PositionReport>()
                    .Where(item => item.DateTime <= from.AddDays(1))
                    .OrderByDescending(p => p.DateTime)
                    .Take(1)
                    .ToList())
                .AsQueryable()
                .AsPagedResult(page, GetActualPageSize(pageSize));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static int GetActualPageSize(int? pageSize)
        {
            return
                pageSize == null || pageSize >= _maxPageSize ? _defaultPageSize : pageSize.Value;
        }
    }
}