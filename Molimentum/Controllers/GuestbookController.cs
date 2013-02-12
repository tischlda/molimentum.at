using System;
using System.Linq;
using System.Web.Mvc;
using Molimentum.Infrastructure;
using Molimentum.Infrastructure.Raven;
using Molimentum.Models;
using Molimentum.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using PoliteCaptcha;

namespace Molimentum.Controllers
{
    public class GuestbookController : Controller
    {
        private readonly IDocumentSession _session;
        
        private const int _pageSize = 10;

        public GuestbookController(IDocumentSession session)
        {
            _session = session;
        }

        public ActionResult Index(int? page)
        {
            var query = _session.Query<CommentWithParent_ByDateTime.Result, CommentWithParent_ByDateTime>()
                .Customize(custom => custom
                    .Include<CommentWithParent_ByDateTime.Result>(item => item.ParentId)
                    .WaitForNonStaleResultsAsOfLastWrite(TimeSpan.FromSeconds(10)))
                .OrderByDescending(c => c.DateTime)
                .AsPagedResult(page, _pageSize);

            var result = new PagedResult<CommentWithParent>
            {
                Items = query.Items
                    .Select(item =>
                        new CommentWithParent
                        {
                            Comment = item.Comment,
                            Parent = _session.Load<object>(item.ParentId)
                        }).ToList(),
                Pager = query.Pager
            };
            
            return View(result);
        }

        public ActionResult AddComment(string parentId)
        {
            var comment = new CommentWithParentId
            {
                ParentId = parentId,
                Item = new Comment()
            };

            return View(Request.IsAjaxRequest() ? "AddCommentAjax" : "AddComment", comment);
        }

        [HttpPost]
        [ValidateSpamPrevention]
        [ValidateInput(false)]
        public ActionResult AddComment([Bind(Exclude = "Item.Id")] CommentWithParentId comment)
        {
            ICommentable parent = null;

            if (comment.ParentId != null)
            {
                parent = _session.Load<object>(comment.ParentId) as ICommentable;

                if (parent == null)
                    throw new ArgumentOutOfRangeException("comment.ParentId", comment.ParentId, "Parent object not found.");
            }

            if (!ModelState.IsValid)
            {
                return View(Request.IsAjaxRequest() ? "AddCommentAjax" : "AddComment", comment);
            }

            comment.Item.DateTime = DateTimeOffset.Now;

            if (parent != null)
            {
                parent.AddComment(comment.Item);
            }
            else
            {
                _session.Store(comment.Item);
            }

            _session.SaveChanges();

            if (Request.IsAjaxRequest())
            {
                return View("CommentAjax", comment.Item);
            }

            if (parent == null)
            {
                return RedirectToAction("Index");
            }
            
            return RedirectToObject(parent);
        }

        protected ActionResult RedirectToObject(object o)
        {
            return Redirect(Url.Detail(o));
        }
    }
}
