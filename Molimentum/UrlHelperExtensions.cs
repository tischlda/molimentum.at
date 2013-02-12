using System;
using System.Web.Mvc;
using Molimentum.Models;

namespace Molimentum
{
    public static class UrlHelperExtensions
    {
        public static string Absolute(this UrlHelper url, string urlString)
        {
            var relativeOrAbsoluteUrl = new Uri(urlString, UriKind.RelativeOrAbsolute);
            if (relativeOrAbsoluteUrl.IsAbsoluteUri) return urlString;

            var requestUrl = url.RequestContext.HttpContext.Request.Url;

            string absoluteUrl = string.Format("{0}://{1}{2}",
                                                  requestUrl.Scheme,
                                                  requestUrl.Authority,
                                                  urlString);

            return absoluteUrl;
        }
        
        public static string Detail(this UrlHelper url, object o)
        {
            if (o is Post)
            {
                var post = (Post)o;

                return url.Action("Detail", "Blog", new { post.Slug });
            }
            
            if (o is Album)
            {
                var album = (Album)o;

                return url.Action("Detail", "Photos", new { album.Slug });
            }
            
            return url.Action("Index", "Guestbook");
        }
    }
}