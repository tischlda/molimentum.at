using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Molimentum.Infrastructure;
using Molimentum.Infrastructure.Raven;
using Molimentum.Models;
using Raven.Client;

namespace Molimentum.Controllers
{
    public class FeedController : Controller
    {
        private readonly IDocumentSession _session;
        
        private const int _pageSize = 25;

        public FeedController(IDocumentSession session)
        {
            _session = session;
        }

        public ActionResult Index()
        {
            var items = _session.Query<FeedItem_ByDateTime.Result, FeedItem_ByDateTime>()
                .Customize(custom => custom.Include<FeedItem_ByDateTime.Result>(item => item.Id))
                .OrderByDescending(item => item.DateTime)
                .Take(_pageSize).ToArray();

            var feed = new SyndicationFeed("Title", "Description", new Uri(Url.Absolute(Url.Action("Index"))))
            {
                LastUpdatedTime = items.First().DateTime,
                Items = from item in items
                        let o = _session.Load<object>(item.Id)
                        select CreateSyndicationItem(o)
            };
            
            return new FeedResult(new Rss20FeedFormatter(feed));
        }

        private SyndicationItem CreateSyndicationItem(object o)
        {
            if (o is Post)
            {
                var post = (Post)o;

                var item = new SyndicationItem
                {
                    Title = new TextSyndicationContent(post.Title),
                    Content = new TextSyndicationContent(post.Body, TextSyndicationContentKind.Html),
                    Id = post.Slug,
                    PublishDate = post.DateTime,
                    LastUpdatedTime = post.DateTime
                };
                item.AddPermalink(new Uri(Url.Absolute(Url.Detail(post))));
                item.Categories.Add(new SyndicationCategory(post.Category.Title));
                foreach (var tag in post.Tags) item.Categories.Add(new SyndicationCategory(tag));

                return item;
            }

            if (o is Album)
            {
                var album = (Album)o;
                
                var item = new SyndicationItem
                {
                    Title = new TextSyndicationContent(album.Title),
                    Content = new TextSyndicationContent(CreateAlbumBody(album), TextSyndicationContentKind.Html),
                    Id = album.Slug,
                    PublishDate = album.DateTime,
                    LastUpdatedTime = album.DateTime
                };
                item.AddPermalink(new Uri(Url.Absolute(Url.Detail(album))));
                item.Categories.Add(new SyndicationCategory("Fotos"));

                return item;
            }
            
            throw new ArgumentException("Can't create syndicaiton item for type " + o.GetType() + ".");
        }

        private static string CreateAlbumBody(Album album)
        {
            var body = new StringBuilder();

            body.AppendFormat("<img src='{0}' alt='{1}' />", album.ThumbnailLinks.Fit(440, 250).Url, HttpUtility.HtmlEncode(album.Title));
            body.Append("<p>");
            body.Append(HttpUtility.HtmlEncode(album.Body));
            body.Append("</p>");
            
            return body.ToString();
        }
    }
}
