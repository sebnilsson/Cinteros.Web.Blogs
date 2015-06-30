using System.Web.Mvc;

using Blaven;

namespace Cinteros.Web.Blogs.Website.Controllers
{
    public abstract class BaseController : Controller
    {
        public const int DefaultCacheDuration = 300;

        public BlogService BlogService { get; protected set; }

        public ViewResult ErrorView(object model)
        {
            return this.View("Error", model);
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            string requestUrl = HttpContext.Request.AppRelativeCurrentExecutionFilePath;
            if (!string.IsNullOrWhiteSpace(System.IO.Path.GetExtension(requestUrl)))
            {
                return;
            }

            string requestUrlLower = requestUrl.ToLowerInvariant();
            if (requestUrl == requestUrlLower)
            {
                return;
            }

            this.HttpContext.Response.Redirect(requestUrlLower, true);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.BlogService = this.BlogService ?? new BlogService(MvcApplication.DocumentStore);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                return;
            }

            if (BlogService != null)
            {
                this.BlogService.Dispose();
            }
        }
    }
}