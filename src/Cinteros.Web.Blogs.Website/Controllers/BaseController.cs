using System.Web;
using System.Web.Mvc;

using Blaven;
using Cinteros.Web.Blogs.Website;

namespace Cinteros.Web.Blogs.Website.Controllers {
    public abstract class BaseController : Controller {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext) {
            base.Initialize(requestContext);

            string requestUrl = HttpContext.Request.AppRelativeCurrentExecutionFilePath;
            if(!string.IsNullOrWhiteSpace(System.IO.Path.GetExtension(requestUrl))) {
                return;
            }

            string requestUrlLower = requestUrl.ToLowerInvariant();
            if(requestUrl == requestUrlLower) {
                return;
            }

            HttpContext.Response.Redirect(requestUrlLower, true);
        }

        public ViewResult ErrorView(object model) {
            return View("Error", model);
        }

        internal BlogService GetBlogService(bool asyncUpdate = true) {
            return MvcApplication.GetBlogService(asyncUpdate);
        }
    }
}
