using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Caching;
using System.Web.Mvc;

using HtmlAgilityPack;

namespace Cinteros.Web.Blogs.Website.Controllers {
    public class InfoController : BaseController {
        public ActionResult Archive() {
            var service = GetBlogService();

            var postDates = service.GetArchiveCount();

            return PartialView(postDates);
        }

        public ActionResult Blogs() {
            var service = GetBlogService();

            var blogs = from setting in service.Config.BloggerSettings
                        let info = service.GetInfo(setting.BlogKey)
                        orderby info.Title ascending
                        select info;

            return PartialView(blogs);
        }

        public ActionResult Tags() {
            var service = GetBlogService();

            var tags = service.GetTagsCount();

            return PartialView(tags);
        }

        public ActionResult MenuItems() {
            string cacheKey = "Cinteros.Web.Blogs.Website.Controllers.InfoController.MenuItems";

            var menuItems = this.HttpContext.Cache[cacheKey] as List<Tuple<string, string, string>>;
            if(menuItems == null) {
                try {
                    var client = new WebClient() {
                        Encoding = System.Text.Encoding.UTF8
                    };
                    string html = client.DownloadString("http://www.cinteros.se");

                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);

                    var nodes = htmlDocument.DocumentNode.SelectNodes("//ul[@id='menu-cinteros-standard']/li");

                    menuItems = (from listItem in nodes
                                 let aTag = listItem.FirstChild
                                 where aTag.Name == "a"
                                 let href = aTag.GetAttributeValue("href", string.Empty)
                                 let linkUrl = GetUrl(href)
                                 where !string.IsNullOrWhiteSpace(href)
                                 select new Tuple<string, string, string>(aTag.InnerText, linkUrl, null)).ToList();

                    // Modify the blog's link (if it's present) to the active menu-item
                    var blogItem = menuItems.FirstOrDefault(item => item.Item2.Contains("blogs.cinteros.se"));
                    if(blogItem != null) {
                        int blogItemIndex = menuItems.IndexOf(blogItem);
                        var updatedBlogItem = new Tuple<string, string, string>(blogItem.Item1, blogItem.Item2, (blogItem.Item3 + " current_page_item").Trim());

                        menuItems.RemoveAt(blogItemIndex);
                        menuItems.Insert(blogItemIndex, updatedBlogItem);
                    }

                    // Change class of last item to "last"
                    var lastItem = menuItems.Last();
                    var updatedLastItem = new Tuple<string, string, string>(lastItem.Item1, lastItem.Item2, (lastItem.Item3 + " last").Trim());

                    menuItems.Remove(lastItem);
                    menuItems.Add(updatedLastItem);
                    
                    this.HttpContext.Cache.Add(cacheKey, menuItems, null, DateTime.Now.AddHours(8), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                }
                catch(Exception) {
                    // Silten fail
                }
            }
            
            return PartialView(menuItems ?? new List<Tuple<string, string, string>>(0));
        }

        private static readonly string WebsiteUrl = "http://www.cinteros.se/";

        private string GetUrl(string href) {
            if(href.StartsWith(WebsiteUrl)) {
                return href;
            }
            
            href = href.TrimStart('/');
            return WebsiteUrl + href;
        }
    }
}