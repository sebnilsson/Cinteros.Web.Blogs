using System;
using System.Web;
using System.Web.Mvc;

using Cinteros.Web.Blogs.Website.Models;

namespace Cinteros.Web.Blogs.Website.Controllers
{
    public class BlogController : BaseController
    {
        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "year;month;page",
            VaryByCustom = "RavenDbLastUpdate")]
        public ActionResult Archive(int year, int month, int? page = 1)
        {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var archiveDate = new DateTime(year, month, 1);

            var posts = this.BlogQuery.ListPostsByArchive(archiveDate);

            var model = new BlogListViewModel(posts, PageSize, pageIndex, this.ControllerContext);

            this.ViewBag.Title = $"Inlägg från {year}-{month}";

            return this.View("List", model);
        }

        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "page", VaryByCustom = "RavenDbLastUpdate")]
        public ActionResult Index(int? page = 1)
        {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            var posts = this.BlogQuery.ListPosts();

            var model = new BlogListViewModel(posts, PageSize, pageIndex, this.ControllerContext);

            this.ViewBag.Title = "Senaste inläggen";

            return this.View("List", model);
        }

        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "q;page", VaryByCustom = "RavenDbLastUpdate")]
        public ActionResult Search(string q, int? page = 1)
        {
            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            string searchTerm = HttpUtility.UrlDecode(q ?? string.Empty);

            var posts = this.BlogQuery.Search(searchTerm);

            var model = new BlogListViewModel(posts, PageSize, pageIndex, this.ControllerContext);

            this.ViewBag.Title = $"Sökresultat för '{q}'";

            return this.View("List", model);
        }

        [OutputCache(Duration = DefaultCacheDuration, VaryByParam = "t;page", VaryByCustom = "RavenDbLastUpdate")]
        public ActionResult Tag(string t, int? page = 1)
        {
            if (string.IsNullOrWhiteSpace(t))
            {
                return this.Index();
            }

            int pageIndex = page.GetValueOrDefault(1) - 1; // Given pageIndex is user-friendly, not 0-based

            string tagName = HttpUtility.UrlDecode(t);

            var posts = this.BlogQuery.ListPostsByTag(tagName);

            var model = new BlogListViewModel(posts, PageSize, pageIndex, this.ControllerContext);

            this.ViewBag.Title = $"Inlägg taggade som '{t}'";
            return this.View("List", model);
        }
    }
}