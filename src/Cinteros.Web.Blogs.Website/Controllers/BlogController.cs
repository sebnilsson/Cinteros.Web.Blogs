using System;
using System.Web;
using System.Web.Mvc;

using Cinteros.Web.Blogs.Website.Models;

namespace Cinteros.Web.Blogs.Website.Controllers
{
    public class BlogController : BaseController
    {
        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "year;month;page",
            VaryByCustom = "RavenDbStaleIndexes")]
        public ActionResult Archive(int year, int month, int? page = 1)
        {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var selection = this.BlogService.GetArchivePosts(new DateTime(year, month, 1), pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            this.ViewBag.Title = string.Format("Inlägg från {0}-{1}", year, month);
            return this.View("List", model);
        }

        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "page", VaryByCustom = "RavenDbStaleIndexes")]
        public ActionResult Index(int? page = 1)
        {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var selection = this.BlogService.GetPosts(pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            this.ViewBag.Title = "Senaste inläggen";
            return this.View("List", model);
        }

        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "q;page", VaryByCustom = "RavenDbStaleIndexes")]
        public ActionResult Search(string q, int? page = 1)
        {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var selection = this.BlogService.SearchPosts(q, pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            this.ViewBag.Title = string.Format("Sökresultat för '{0}'", q);
            return this.View("List", model);
        }

        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "t;page", VaryByCustom = "RavenDbStaleIndexes")]
        public ActionResult Tag(string t, int? page = 1)
        {
            t = HttpUtility.UrlDecode(t);
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var selection = this.BlogService.GetTagPosts(t, pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            this.ViewBag.Title = string.Format("Inlägg taggade som '{0}'", t);
            return this.View("List", model);
        }
    }
}