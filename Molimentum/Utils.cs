using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using HtmlAgilityPack;
using Molimentum.Models;

namespace Molimentum
{
    public static class Utils
    {
        public static string GenerateSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Substring(0, str.Length <= 55 ? str.Length : 55).Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   

            return str;
        }

        public static string RemoveAccent(this string text)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return Encoding.ASCII.GetString(bytes);
        }

        public static string GenerateSummaryFromHtmlString(this string htmlString, int maxLength)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlString);

            var text = HttpUtility.HtmlDecode(doc.DocumentNode.InnerText);

            return text.GenerateSummaryFromString(maxLength);
        }

        public static string GenerateSummaryImageFromHtmlString(this string htmlString)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlString);

            var img = doc.DocumentNode.SelectSingleNode("//img");

            if (img != null)
            {
                img.Attributes["src"].Value = img.Attributes["src"].Value.Replace("/s600/", "/s320/");
                return img.OuterHtml;
            }

            return "";
        }

        public static string GenerateSummaryFromString(this string text, int maxLength)
        {
            if (text.Length > maxLength)
            {
                var spaceIndex = text.Substring(0, maxLength).LastIndexOf(' ');

                var length = (spaceIndex < 0) ? maxLength : spaceIndex;

                var summary = text.Substring(0, length) + "...";
                
                return summary;
            }
            
            return text;
        }

        public static string Join(this IEnumerable<string> values, string separator)
        {
            return String.Join(separator, values);
        }

        public static PictureLink FittingOrSmallest(this IEnumerable<PictureLink> pictureLinks, int maximumWidth, int maximumHeight)
        {
            var pictureLink = pictureLinks.Where(l => l.Width <= maximumWidth && l.Height <= maximumHeight).OrderByDescending(l => l.Width).FirstOrDefault() ??
                              pictureLinks.OrderBy(l => l.Width).FirstOrDefault();

            return pictureLink;
        }

        public static PictureLink Fit(this IEnumerable<PictureLink> pictureLinks, int maximumWidth, int maximumHeight)
        {
            var largest = pictureLinks.OrderByDescending(l => l.Width).First();

            double relation = (double)largest.Width / (double)largest.Height;

            double targetWidth = maximumWidth;
            double targetHeight = maximumWidth / relation;

            if (targetHeight > maximumHeight)
            {
                targetWidth = targetWidth / targetHeight * maximumHeight;
                targetHeight = maximumHeight;
            }

            var pictureLink = pictureLinks.Where(l => l.Width >= targetWidth && l.Height >= targetHeight).OrderBy(l => l.Width).FirstOrDefault() ??
                              pictureLinks.OrderByDescending(l => l.Width).FirstOrDefault();

            return new PictureLink
            {
                Width = (int)targetWidth,
                Height = (int)targetHeight,
                Url = pictureLink.Url
            };
        }

        public static void InvokeAndRetryOn<T>(this Action action, int retries, int millisecondsTimeout)
            where T : Exception
        {
            if (action == null) throw new ArgumentNullException("action");

            while (retries-- > 0)
            {
                try
                {
                    action();
                    return;
                }
                catch (T)
                {
                    Thread.Sleep(millisecondsTimeout);
                }
            }

            action();
        }

        public static void InvokeAndIgnore<T>(this Action action)
            where T : Exception
        {
            if (action == null) throw new ArgumentNullException("action");

            try
            {
                action();
            }
            catch (T)
            {
            }
        }
    }
}