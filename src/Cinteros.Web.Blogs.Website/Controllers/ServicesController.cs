using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

using Blaven;

namespace Cinteros.Web.Blogs.Website.Controllers {
    public class ServicesController : BaseController {
        private const int DefaultPageSize = 20;
        private const int MaxPageSize = 50;
        private static XNamespace _contentNs = XNamespace.Get("http://purl.org/rss/1.0/modules/content/");

        public ActionResult BasicRss(int? pageSize = DefaultPageSize) {
            return GetRss(pageSize, false);
        }

        public ActionResult Rss(int? pageSize = DefaultPageSize) {
            return GetRss(pageSize, true);
        }

        private ActionResult GetRss(int? pageSize = DefaultPageSize, bool includeBlogContent = true) {
            int actualPageSize = Math.Min(pageSize.GetValueOrDefault(DefaultPageSize), MaxPageSize);

            var service = GetBlogService();
            service.Config.PageSize = actualPageSize;

            var selection = service.GetSelection(0);

            var rssItems = from post in selection.Posts
                           select GetPostXElement(post, includeBlogContent);

            var rss = new XDocument(
                          new XDeclaration("1.0", "UTF-8", "yes"),
                          new XElement("rss",
                              new XAttribute("version", "2.0"),
                              new XAttribute(XNamespace.Xmlns + "content", _contentNs),
                              new XElement("channel",
                                  new XElement("title", "Cinteros Blogs RSS"),
                                  new XElement("link", string.Format("http://{0}/", Request.Url.Host)),
                                  new XElement("description", "RSS feed containing blog-posts from bloggers at Cinteros AB."),
                                  rssItems
                              )
                          )
                      );

            return Content(rss.ToString(), "application/rss+xml");
        }

        private static XElement GetPostXElement(BlogPost post, bool includeBlogContent) {
            var itemElements = new[] {
                new XElement("title", post.Title),
                new XElement("link", post.PermaLinkAbsolute),
                new XElement("pubDate", post.Published.ToString("r")),
            };
            if(includeBlogContent) {
                itemElements = itemElements.Concat(new[] { new XElement(_contentNs + "encoded", new XCData(post.Content)) })
                    .ToArray();
            }

            return new XElement("item",
                itemElements
            );
        }

        public ActionResult UpdateBlogs() {
            var service = GetBlogService();
            service.Update();

            return new EmptyResult();
        }
    }
}