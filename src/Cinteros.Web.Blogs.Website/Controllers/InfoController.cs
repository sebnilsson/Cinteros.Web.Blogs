using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using Blaven.Data.RavenDb2;
using Cinteros.Web.Blogs.Website.Models;
using HtmlAgilityPack;

namespace Cinteros.Web.Blogs.Website.Controllers
{
    public class InfoController : BaseController
    {
        private const string WebsiteUrl = "http://www.cinteros.se/";

        [OutputCache(Duration = DefaultCacheDuration)]
        public ActionResult Archive()
        {
            var postDates =
                this.BlogQuery.ListArchive()
                    .ToListAll()
                    .GroupBy(x => x.Date.Date)
                    .Select(x => new { Date = x.First().Date, Count = x.Sum(item => item.Count) })
                    .ToDictionary(x => x.Date, x => x.Count);

            return this.PartialView(postDates);
        }

        [OutputCache(Duration = DefaultCacheDuration)]
        public ActionResult Blogs()
        {
            var blogs = from setting in BlavenConfig.BlogSettingsLazy.Value
                        let meta = this.BlogQuery.GetBlogMeta(setting.BlogKey)
                        where meta != null
                        orderby meta.Name ascending
                        select meta;

            return this.PartialView(blogs);
        }

        [OutputCache(Duration = DefaultCacheDuration)]
        public ActionResult Tags()
        {
            var tags =
                this.BlogQuery.ListTags()
                    .ToListAll()
                    .GroupBy(x => x.Name.ToLowerInvariant())
                    .Select(
                        x => new { Name = x.Select(y => y.Name).FirstOrDefault(), Count = x.Sum(item => item.Count) })
                    .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                    .ToDictionary(x => x.Name, x => x.Count);

            return this.PartialView(tags);
        }

        public static readonly string MenuItemsCacheKey = "InfoController.MenuItems.LastUpdate";

        [OutputCache(Duration = DefaultCacheDuration)]
        public ActionResult MenuItems()
        {
            var storeMenuItems = new List<MenuItem>(0);

            var lastUpdate =
                (this.HttpContext.Cache[MenuItemsCacheKey] as DateTime?).GetValueOrDefault(DateTime.MinValue);

            if (lastUpdate.AddHours(8) < DateTime.Now)
            {
                var store = BlavenConfig.DocumentStore;

                using (var session = store.OpenSession())
                {
                    var storeHasData = session.Query<MenuItem>().Any();

                    var task = new Task(
                        () =>
                            {
                                var cinterosMenuItems = this.GetCinterosMenuItems();

                                if (cinterosMenuItems.Any())
                                {
                                    session.Query<MenuItem>().Take(int.MaxValue).ToList().ForEach(session.Delete);

                                    session.SaveChanges();
                                }

                                cinterosMenuItems.ToList()
                                    .ForEach(x => session.Store(x, Convert.ToString((uint)x.Url.GetHashCode())));

                                session.SaveChanges();
                            });
                    task.Start();

                    if (!storeHasData)
                    {
                        task.Wait();
                    }

                    storeMenuItems = session.Query<MenuItem>().Take(int.MaxValue).OrderBy(x => x.Index).ToList();
                }
            }

            this.HttpContext.Cache[MenuItemsCacheKey] = DateTime.Now;

            return this.PartialView(storeMenuItems);
        }

        private List<MenuItem> GetCinterosMenuItems()
        {
            var client = new WebClient { Encoding = System.Text.Encoding.UTF8 };
            string html = client.DownloadString("http://www.cinteros.se");

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var nodes = htmlDocument.DocumentNode.SelectNodes("//ul[@id='menu-cinteros-standard']/li");

            var menuItems = (from listItem in nodes
                             let aTag = listItem.FirstChild
                             where aTag.Name == "a"
                             let href = aTag.GetAttributeValue("href", string.Empty)
                             let linkUrl = this.GetUrl(href)
                             where !string.IsNullOrWhiteSpace(href)
                             select new MenuItem(aTag.InnerText, linkUrl)).ToList();
            for (int i = 0; i < menuItems.Count; i++)
            {
                menuItems[i].Index = i;
            }

            // Modify the blog's link (if it's present) to the active menu-item
            var blogItem = menuItems.FirstOrDefault(item => item.Url.Contains("blogs.cinteros.se"));
            if (blogItem != null)
            {
                blogItem.Class += " current_page_item";
            }

            // Change class of last item to "last"
            var lastItem = menuItems.LastOrDefault();
            if (lastItem != null)
            {
                lastItem.Class += " last";
            }

            return menuItems;
        }

        private string GetUrl(string href)
        {
            if (href.StartsWith(WebsiteUrl))
            {
                return href;
            }

            href = href.TrimStart('/');
            return (!href.StartsWith("http")) ? (WebsiteUrl + href) : href;
        }
    }
}