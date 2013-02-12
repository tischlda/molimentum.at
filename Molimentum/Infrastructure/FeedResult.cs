using System;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Molimentum.Infrastructure
{
    public class FeedResult : ActionResult
    {
        public FeedResult(SyndicationFeedFormatter feedFormatter)
        {
            if (feedFormatter == null) throw new ArgumentNullException("feedFormatter");

            FeedFormatter = feedFormatter;
        }

        public SyndicationFeedFormatter FeedFormatter { get; private set; }
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/rss+xml";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            using (var xmlWriter = new XmlTextWriter(response.Output))
            {
                xmlWriter.Formatting = Formatting.Indented;
                FeedFormatter.WriteTo(xmlWriter);
            }
        }
    }
}