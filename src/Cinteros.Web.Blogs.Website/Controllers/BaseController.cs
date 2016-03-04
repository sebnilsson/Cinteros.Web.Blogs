using System;
using System.Web.Mvc;

using Blaven;
using Blaven.Synchronization;

namespace Cinteros.Web.Blogs.Website.Controllers
{
    public abstract class BaseController : Controller
    {
        public const int DefaultCacheDuration = 300;

        public const int PageSize = 10;

        private readonly Lazy<BlogQueryService> blogQueryLazy =
            new Lazy<BlogQueryService>(BlavenConfig.BlogQueryServiceFactory);

        private readonly Lazy<BlogSyncService> blogSyncLazy =
            new Lazy<BlogSyncService>(BlavenConfig.BlogSyncServiceFactory);

        public BlogQueryService BlogQuery => this.blogQueryLazy.Value;

        public BlogSyncService BlogSync => this.blogSyncLazy.Value;

        public ViewResult ErrorView(object model)
        {
            return this.View("Error", model);
        }
    }
}