using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

using Blaven;

namespace Cinteros.Web.Blogs.Website.Controllers {
    public class ServicesController : BaseController {
        private const string ContentType = "text/xml";
        private const int DefaultPageSize = 20;
        private const int MaxPageSize = 50;

        public ActionResult BasicRss(int? pageSize = DefaultPageSize) {
            Response.ContentType = ContentType;
            return GetRss(pageSize, false);
        }

        public ActionResult Rss(int? pageSize = DefaultPageSize) {
            Response.ContentType = ContentType;
            return GetRss(pageSize, true);
        }

        private ActionResult GetRss(int? pageSize = DefaultPageSize, bool includeBlogContent = true) {
            int actualPageSize = Math.Min(pageSize.GetValueOrDefault(DefaultPageSize), MaxPageSize);

            this.BlogService.Config.PageSize = actualPageSize;

            var selection = this.BlogService.GetSelection(0);

            var rssItems = from post in selection.Posts
                           select GetPostXElement(post, includeBlogContent);

            string rssUrl = "http://" + Request.Url.Host + Url.RouteUrl("RSS", null);

            var rss = new XDocument(
                          new XDeclaration("1.0", "UTF-8", "yes"),
                          new XElement("rss",
                              new XAttribute("version", "2.0"),
                              new XElement("channel",
                                  new XElement("title", "Cinteros Blogs RSS"),
                                  new XElement("link", rssUrl),
                                  new XElement("description", "RSS feed containing blog-posts from bloggers at Cinteros AB."),
                                  rssItems
                              )
                          )
                      );

            return Content(rss.ToString(), ContentType);
        }

        private static XElement GetPostXElement(BlogPost post, bool includeBlogContent) {
            var itemElements = new[] {
                new XElement("title", post.Title),
                new XElement("link", post.PermaLinkAbsolute),
                new XElement("pubDate", post.Published.ToString("r")),
                new XElement("author", post.Author.Name),
            };
            if(includeBlogContent) {
                itemElements = itemElements.Concat(new[] {
                    new XElement("description", post.Content),
                }).ToArray();
            }

            return new XElement("item",
                itemElements
            );
        }

        public ActionResult RefreshBlogs() {
            var timer = new System.Diagnostics.Stopwatch();

            timer.Start();
            var refreshResults = this.BlogService.ForceRefresh();
            timer.Stop();

            var result = new { TimeElapsed = timer.Elapsed.TotalSeconds.ToString("#.###s"), Blogs = refreshResults.Select(x => x.Item1) };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClearMenuItems() {
            this.HttpContext.Cache.Remove(InfoController.MenuItemsCacheKey);

            return this.RedirectToRoute("Empty");
        }
    }
}