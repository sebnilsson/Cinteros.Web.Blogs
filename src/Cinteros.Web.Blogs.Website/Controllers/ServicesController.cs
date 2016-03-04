using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

using Blaven;
using Blaven.Data.RavenDb2;

namespace Cinteros.Web.Blogs.Website.Controllers
{
    public class ServicesController : BaseController
    {
        private const string ContentType = "application/rss+xml";

        private const string CommunityTagName = "community";

        private const int DefaultPageSize = 10;

        private const int MaxPageSize = 30;

        [OutputCache(Duration = DefaultCacheDuration)]
        public ActionResult BasicRss(int pageSize = DefaultPageSize)
        {
            return this.GetDefaultRss(pageSize, includeBlogContent: false);
        }

        [OutputCache(Duration = DefaultCacheDuration)]
        public ActionResult Rss(int pageSize = DefaultPageSize)
        {
            return this.GetDefaultRss(pageSize);
        }

        [OutputCache(Duration = DefaultCacheDuration)]
        public ActionResult CommunityRss(int? pageSize = DefaultPageSize)
        {
            int actualPageSize = Math.Min(pageSize.GetValueOrDefault(DefaultPageSize), MaxPageSize);

            var posts = this.BlogQuery.ListPostsByTag(CommunityTagName).Take(actualPageSize).ToListAll();

            return this.GetRssFeed(posts);
        }

        [OutputCache(Duration = 60)]
        public ActionResult Refresh()
        {
            var refreshResults = this.BlogSync.ForceUpdate();

            return
                this.Json(
                    refreshResults.Select(
                        x => new { Blog = x.BlogKey, Elapsed = x.Elapsed.TotalSeconds.ToString("0.###s") }),
                    JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 60)]
        public ActionResult ClearMenuItems()
        {
            this.HttpContext.Cache.Remove(InfoController.MenuItemsCacheKey);

            return this.RedirectToRoute("Empty");
        }

        private ActionResult GetDefaultRss(int pageSize, bool includeBlogContent = true)
        {
            var posts = this.BlogQuery.ListPosts().Take(pageSize).ToListAll();

            return this.GetRssFeed(posts, includeBlogContent);
        }

        private ActionResult GetRssFeed(IEnumerable<BlogPost> posts, bool includeBlogContent = true)
        {
            this.Response.ContentType = ContentType;

            var rssItems = from post in posts select GetPostXElement(post, includeBlogContent);

            string rssUrl = "http://" + this.Request.Url?.Host + this.Url.RouteUrl("Empty", null);

            var rss = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(
                    "rss",
                    new XAttribute("version", "2.0"),
                    new XElement(
                        "channel",
                        new XElement("title", "Cinteros Blogs"),
                        new XElement("link", rssUrl),
                        new XElement("description", "RSS feed containing blog-posts from bloggers at Cinteros AB."),
                        rssItems)));

            return this.Content(rss.ToString(), ContentType);
        }

        private static XElement GetPostXElement(BlogPost post, bool includeBlogContent)
        {
            var itemElements = new[]
                                   {
                                       new XElement("title", post.Title), new XElement("link", post.SourceUrl),
                                       new XElement("pubDate", post.PublishedAt?.ToString("r")),
                                       new XElement("author", post.Author.Name),
                                   };

            if (includeBlogContent)
            {
                itemElements = itemElements.Concat(new[] { new XElement("description", post.Content) }).ToArray();
            }

            return new XElement("item", itemElements.Cast<XObject>());
        }
    }
}