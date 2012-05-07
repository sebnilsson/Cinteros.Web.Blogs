using System;
using System.Web;
using System.Web.Mvc;

using Cinteros.Web.Blogs.Website.Models;

namespace Cinteros.Web.Blogs.Website.Controllers {
    public class BlogController : BaseController {
        public ActionResult Archive(int year, int month, int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var service = GetBlogService();

            var selection = service.GetArchiveSelection(new DateTime(year, month, 1), pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            ViewBag.Title = string.Format("Inlägg från {0}-{1}", year, month);

            return View("List", model);
        }

        public ActionResult Index(int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var service = GetBlogService();

            var selection = service.GetSelection(pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };
            
            ViewBag.Title = "Senaste inläggen";
            return View("List", model);
        }

        public ActionResult Blog(string blogKey, int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var service = GetBlogService();

            var selection = service.GetSelection(pageIndex, blogKey);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            ViewBag.Title = string.Format("Senaste inläggen från blog '{0}'", service.GetInfo(blogKey).Title);
            return View("List", model);
        }

        public ActionResult Search(string q, int? page = 1) {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var service = GetBlogService();

            var selection = service.SearchPosts(q, pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            ViewBag.Title = string.Format("Sökresultat för '{0}'", q);
            return View("List", model);
        }

        public ActionResult Tag(string tagName, int? page = 1) {
            tagName = HttpUtility.UrlDecode(tagName);
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var service = GetBlogService();

            var selection = service.GetTagsSelection(tagName, pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            ViewBag.Title = string.Format("Inlägg taggade som '{0}'", tagName);
            return View("List", model);
        }

    }
}