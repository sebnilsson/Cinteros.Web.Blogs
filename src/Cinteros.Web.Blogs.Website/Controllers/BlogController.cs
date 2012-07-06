using System;
using System.Web;
using System.Web.Mvc;

using Cinteros.Web.Blogs.Website.Models;

namespace Cinteros.Web.Blogs.Website.Controllers {
    public class BlogController : BaseController {
        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "year,month,page")]
        public ActionResult Archive(int year, int month, int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based
            
            var selection = this.BlogService.GetArchiveSelection(new DateTime(year, month, 1), pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            ViewBag.Title = string.Format("Inlägg från {0}-{1}", year, month);
            return View("List", model);
        }

        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "page")]
        public ActionResult Index(int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var selection = this.BlogService.GetSelection(pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            ViewBag.Title = "Senaste inläggen";
            return View("List", model);
        }

        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "q,page")]
        public ActionResult Search(string q, int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var selection = this.BlogService.SearchPosts(q, pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            ViewBag.Title = string.Format("Sökresultat för '{0}'", q);
            return View("List", model);
        }

        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "t,page")]
        public ActionResult Tag(string t, int? page = 1) {
            t = HttpUtility.UrlDecode(t);
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var selection = this.BlogService.GetTagsSelection(t, pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            ViewBag.Title = string.Format("Inlägg taggade som '{0}'", t);
            return View("List", model);
        }

    }
}